using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Entities.Entities.Orders?> GetByIdAsync(int orderId);
        Task AddAsync(Entities.Entities.Orders order);
        void Update(Entities.Entities.Orders order);
    }
}