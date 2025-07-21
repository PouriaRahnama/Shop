using Common.Application;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg.Repository;
public record CancelOrderCommand(long OrderId) : IBaseCommand;

public class CancelOrderCommandHandler : IBaseCommandHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISellerRepository _sellerRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, ISellerRepository sellerRepository)
    {
        _orderRepository = orderRepository;
        _sellerRepository = sellerRepository;
    }

    public async Task<OperationResult> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetTracking(request.OrderId);
        if (order == null)
            return OperationResult.NotFound();

        // آزادسازی موجودی رزرو شده
        foreach (var item in order.Items)
        {
            var inventory = await _sellerRepository.GetInventoryById(item.InventoryId);
            if (inventory == null)
                return OperationResult.Error($"موجودی آیتم با شناسه {item.InventoryId} یافت نشد.");

            var seller = await _sellerRepository.GetTracking(inventory.SellerId);
            if (seller == null) return OperationResult.Error($"فروشنده یافت نشد");
            seller.ReleaseReserved(inventory.Id,item.Count);
        }

        // لغو سفارش
        order.ChangeStatus(Shop.Domain.OrderAgg.OrderStatus.Rejected);

        await _sellerRepository.Save();
        await _orderRepository.Save();

        return OperationResult.Success();
    }
}
