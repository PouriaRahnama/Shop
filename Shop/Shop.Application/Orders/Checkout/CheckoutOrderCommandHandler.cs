using Common.Application;
using Shop.Domain.OrderAgg;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.OrderAgg.ValueObjects;
using Shop.Domain.SellerAgg.Repository;
using Shop.Domain.SiteEntities.Repositories;

namespace Shop.Application.Orders.Checkout
{
    public class CheckoutOrderCommandHandler : IBaseCommandHandler<CheckoutOrderCommand>
    {
        private static readonly SemaphoreSlim _inventoryLock = new SemaphoreSlim(1, 1);
        private readonly IOrderRepository _repository;
        private readonly ISellerRepository _sellerRepository;
        private IShippingMethodRepository _shippingMethodRepository;
        public CheckoutOrderCommandHandler(IOrderRepository repository,
            IShippingMethodRepository shippingMethodRepository, ISellerRepository sellerRepository)
        {
            _repository = repository;
            _shippingMethodRepository = shippingMethodRepository;
            _sellerRepository = sellerRepository;
        }

        public async Task<OperationResult> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var currentOrder = await _repository.GetCurrentUserOrder(request.UserId);
            if (currentOrder == null)
                return OperationResult.NotFound();

            var address = new OrderAddress(request.Shire, request.City, request.PostalCode,
                request.PostalAddress, request.PhoneNumber, request.Name,
                request.Family, request.NationalCode);

            var shippingMethod = await _shippingMethodRepository.GetAsync(request.ShippingMethodId);
            if (shippingMethod == null)
                return OperationResult.Error();


            currentOrder.Checkout(address, new OrderShippingMethod(shippingMethod.Title, shippingMethod.Cost));
            await _inventoryLock.WaitAsync();
            try
            {
                // رزرو موجودی برای هر آیتم سفارش
                foreach (var item in currentOrder.Items)
                {
                    var inventory = await _sellerRepository.GetInventoryById(item.InventoryId);
                    if (inventory == null)
                        return OperationResult.Error("موجودی کالا یافت نشد.");

                    var seller = await _sellerRepository.GetTracking(inventory.SellerId);
                    if (seller == null) return OperationResult.Error("فروشنده کالا یافت نشد."); ;

                    seller.Reserve(inventory.Id, item.Count);  // رزرو انجام میشه
                }
            }
            finally
            {
                _inventoryLock.Release();
            }
            await _repository.Save();
            return OperationResult.Success();
        }
    }
}