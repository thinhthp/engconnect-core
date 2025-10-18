using EngConnect.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int paymentId);
        Task<Payment?> GetByTransactionCodeAsync(string transactionCode);
        Task AddAsync(Payment payment);
        void Update(Payment payment);
    }
}