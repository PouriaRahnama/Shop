using Common.Application;
using MediatR;
using Shop.Domain.OrderAgg;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg;
using Shop.Domain.SellerAgg.Repository;
using System.Data;
using System.Threading;

namespace Shop.Application.Orders.Finally;

public class OrderFinallyCommandHandler : IBaseCommandHandler<OrderFinallyCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISellerRepository _sellerRepository;

    // ✅ یک قفل سراسری استاتیک
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    public OrderFinallyCommandHandler(IOrderRepository orderRepository, ISellerRepository sellerRepository)
    {
        _orderRepository = orderRepository;
        _sellerRepository = sellerRepository;
    }

    public async Task<OperationResult> Handle(OrderFinallyCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetTracking(request.OrderId);
        if (order == null)
            return OperationResult.NotFound();

        await _orderRepository.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
        try
        {

            // کاهش موجودی
            foreach (var item in order.Items)
            {
                var inventory = await _sellerRepository.GetInventoryById(item.InventoryId);
                if (inventory == null)
                {
                    Console.WriteLine($"⚠️ Inventory not found: {item.InventoryId}");
                    continue;
                }

                var seller = await _sellerRepository.GetTracking(inventory.SellerId);
                if (seller == null)
                {
                    Console.WriteLine($"⚠️ Seller not found: {inventory.SellerId}");
                    continue;
                }

                var affectedRows = await _orderRepository.DecreaseInventoryAtomic(item.InventoryId, item.Count);
                if (affectedRows == 0)
                {
                    throw new Exception("موجودی کافی نیست");
                }
                seller.DecreaseCount(inventory.Id, item.Count);

            }
            // تغییر وضعیت سفارش
            order.Finally();
            await _sellerRepository.Save();
            await _orderRepository.CommitTransactionAsync();
            return OperationResult.Success();
        }
        catch
        {
            await _orderRepository.RollbackTransactionAsync();
            throw;
        }

    }

}