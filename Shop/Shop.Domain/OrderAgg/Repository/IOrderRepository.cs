using Common.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.OrderAgg.Repository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task BeginTransactionAsync(IsolationLevel isolationLevel);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<Order> GetCurrentUserOrder(long userId);
        Task<int> ReserveInventory(long inventoryId, int count);
        Task<int> DecreaseInventoryAtomic(long inventoryId, int count);
    }
}