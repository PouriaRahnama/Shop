using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shop.Domain.OrderAgg;
using Shop.Domain.OrderAgg.Repository;
using Shop.Infrastructure._Utilities;
using System.Data;

namespace Shop.Infrastructure.Persistent.Ef.OrderAgg
{
    internal class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private IDbContextTransaction _currentTransaction;

        public OrderRepository(ShopContext context) : base(context)
        {
        }
        public async Task<Order?> GetCurrentUserOrder(long userId)
        {
            return await Context.Orders
                .AsTracking()
                .FirstOrDefaultAsync(f => f.UserId == userId &&
                (f.Status == OrderStatus.Pending || f.Status == OrderStatus.CheckedOut));
        }

        public async Task<int> ReserveInventory(long inventoryId, int count)
        {
            return await Context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE [seller].Inventories
                SET ReservedCount = ReservedCount + {count}
                WHERE Id = {inventoryId}
                  AND (Count - ReservedCount) >= {count}
            ");
        }
        public async Task BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            _currentTransaction = await Context.Database.BeginTransactionAsync(isolationLevel);
        }

        public async Task CommitTransactionAsync()
        {
            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task<int> DecreaseInventoryAtomic(long inventoryId, int count)
        {
            // UPDATE شرطی، فقط وقتی موجودی کافی هست
            return await Context.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE [seller].Inventories
            SET ReservedCount = ReservedCount - {count}, Count = Count - {count}
            WHERE Id = {inventoryId} AND ReservedCount >= {count}
        ");

        }
    }
}