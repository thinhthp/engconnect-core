using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngConnect.Repositories.Common;
using OrderEntity = EngConnect.Entities.Entities.Orders;

namespace EngConnect.Repositories.Repositories.Orders
{
    public interface IOrderRepository : IGenericRepository<OrderEntity>
    {
        Task<OrderEntity?> GetByIdAsync(int orderId);
        Task<OrderEntity?> GetByIdWithItemsAndCoursesAsync(int orderId);
    }
}