using Common.Application;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg.Repository;

namespace Shop.Application.Orders.IncreaseItemCount
{
    public class IncreaseOrderItemCountCommandHandler : IBaseCommandHandler<IncreaseOrderItemCountCommand>
    {
        private readonly IOrderRepository _repository;
        private readonly ISellerRepository _sellerRepository;

        public IncreaseOrderItemCountCommandHandler(IOrderRepository repository, ISellerRepository sellerRepository)
        {
            _repository = repository;
            _sellerRepository = sellerRepository;
        }

        public async Task<OperationResult> Handle(IncreaseOrderItemCountCommand request, CancellationToken cancellationToken)
        {
            var currentOrder = await _repository.GetCurrentUserOrder(request.UserId);
            if (currentOrder == null)
                return OperationResult.NotFound();


            var item = currentOrder.Items.Where(i => i.Id == request.ItemId).FirstOrDefault();

            var Inventory = await _sellerRepository.GetInventoryById(item.InventoryId);

            if (Inventory.Count <= item.Count)
                return OperationResult.Error("موجودی نسبت به درخواست شما کم می باشد.");

            currentOrder.IncreaseItemCount(request.ItemId, request.Count);
            await _repository.Save();
            return OperationResult.Success();
        }
    }
}