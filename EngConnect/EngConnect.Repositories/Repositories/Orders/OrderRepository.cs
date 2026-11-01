using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using OrderEntity = EngConnect.Entities.Entities.Orders;

namespace EngConnect.Repositories.Repositories.Orders
{
    public class OrderRepository : GenericRepository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<OrderEntity?> GetByIdAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.Orderitems).ThenInclude(oi => oi.Course)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<OrderEntity?> GetByIdWithItemsAndCoursesAsync(int orderId)
        {
            // for payment webhook
            return await _dbSet
                .AsSplitQuery()
                .Include(o => o.Orderitems).ThenInclude(oi => oi.Course)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}