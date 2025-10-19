using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EngConnectContext _context;

        public OrderRepository(EngConnectContext context)
        {
            _context = context;
        }

        public async Task<Entities.Entities.Orders?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Orderitems).ThenInclude(oi => oi.Course)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Entities.Entities.Orders?> GetByIdWithItemsAndCoursesAsync(int orderId)
        {
            // for payment webhook
            return await _context.Orders
                .AsSplitQuery()
                .Include(o => o.Orderitems).ThenInclude(oi => oi.Course)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task AddAsync(Entities.Entities.Orders order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void Update(Entities.Entities.Orders order)
        {
            _context.Orders.Update(order);
        }
    }
}