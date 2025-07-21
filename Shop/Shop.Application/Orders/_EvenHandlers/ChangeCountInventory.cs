using MediatR;
using Shop.Domain.OrderAgg.Events;
using Shop.Domain.OrderAgg.Repository;
using Shop.Domain.SellerAgg.Repository;
using System.Threading;

namespace Shop.Application.Orders._EventHandlers;

public class ChangeCountInventory : INotificationHandler<OrderFinalized>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISellerRepository _sellerRepository;

    // ✅ یک قفل سراسری استاتیک
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public ChangeCountInventory(IOrderRepository orderRepository, ISellerRepository sellerRepository)
    {
        _orderRepository = orderRepository;
        _sellerRepository = sellerRepository;
    }

    public async Task Handle(OrderFinalized notification, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken); // 🚦 قفل گرفتن

        try
        {
            Console.WriteLine("ChangeCountInventory");

            var order = await _orderRepository.GetTracking(notification.OrderId);
            if (order == null) return;

            foreach (var item in order.Items)
            {
                var inventory = await _sellerRepository.GetInventoryById(item.InventoryId);
                if (inventory == null) continue;

                var seller = await _sellerRepository.GetTracking(inventory.SellerId);
                if (seller == null) continue;

                seller.DecreaseCount(inventory.Id,item.Count);
            }

            await _sellerRepository.Save();
        }
        finally
        {
            _semaphore.Release(); // 🔓 آزاد کردن قفل
        }
    }
}
