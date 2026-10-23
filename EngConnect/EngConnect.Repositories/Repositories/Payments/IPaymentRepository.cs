using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Payments.Filters;

namespace EngConnect.Repositories.Repositories.Payments
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByIdAsync(int paymentId);
        Task<Payment?> GetByTransactionCodeAsync(string transactionCode);
        Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetForAdminAsync(PaymentQueryParameters parameters, CancellationToken cancellationToken = default);
    }
}