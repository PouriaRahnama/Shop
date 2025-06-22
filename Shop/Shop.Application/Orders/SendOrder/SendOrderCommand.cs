using Common.Application;
using Shop.Domain.OrderAgg;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg.Repository;

namespace Shop.Application.Orders.SendOrder;

public class SendOrderCommand : IBaseCommand
{
    public SendOrderCommand(long orderId)
    {
        OrderId = orderId;
    }

    public long OrderId { get; private set; }
}

public class SendOrderCommandHandler : IBaseCommandHandler<SendOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISellerRepository _sellerRepository;

    public SendOrderCommandHandler(IOrderRepository orderRepository, ISellerRepository sellerRepository)
    {
        _orderRepository = orderRepository;
        _sellerRepository = sellerRepository;
    }

    public async Task<OperationResult> Handle(SendOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetTracking(request.OrderId);
        if (order == null)
            return OperationResult.NotFound();

        order.ChangeStatus(OrderStatus.Shipping);
        await _orderRepository.Save();

        #region 
        //foreach (var item in order.Items)
        //{
        //    var Inventory = await _sellerRepository.GetInventoryById(item.InventoryId);

        //    var seller = await _sellerRepository.GetTracking(Inventory.SellerId);

        //    var count = Inventory.Count - item.Count;
        //    if (count <= 0)
        //        count = 0;

        //    seller.EditInventory(Inventory.Id, count, Inventory.Price, Inventory.DiscountPercentage);
        //    await _sellerRepository.Save();
        //}
        #endregion

        return OperationResult.Success();
    }
}