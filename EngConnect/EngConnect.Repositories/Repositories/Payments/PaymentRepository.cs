using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using EngConnect.Repositories.Repositories.Payments.Filters;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Payments
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            return await _dbSet
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<Payment?> GetByTransactionCodeAsync(string transactionCode)
        {
            return await _dbSet
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionCode == transactionCode);
        }

        public async Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetForAdminAsync(
            PaymentQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(p => p.Order)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                var status = parameters.Status.Trim().ToLower();
                query = query.Where(p => p.Status.ToLower() == status);
            }

            if (parameters.FromDate.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(parameters.FromDate.Value, DateTimeKind.Utc);
                query = query.Where(p => p.CreatedAt >= fromUtc);
            }

            if (parameters.ToDate.HasValue)
            {
                // inclusive ToDate (till end of day)
                var toUtc = DateTime.SpecifyKind(parameters.ToDate.Value, DateTimeKind.Utc);
                var inclusiveTo = toUtc.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.CreatedAt <= inclusiveTo);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var search = parameters.Search.Trim();
                var isOrderId = int.TryParse(search, out var orderId);
                query = query.Where(p =>
                    (p.TransactionCode != null && EF.Functions.Like(p.TransactionCode, $"%{search}%"))
                    || (isOrderId && p.OrderId == orderId));
            }

            // Sorting
            var sortBy = (parameters.SortBy ?? "createdAt").Trim().ToLowerInvariant();
            var desc = string.Equals(parameters.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

            query = (sortBy) switch
            {
                "transactiondate" => desc ? query.OrderByDescending(p => p.TransactionDate).ThenByDescending(p => p.PaymentId)
                                          : query.OrderBy(p => p.TransactionDate).ThenBy(p => p.PaymentId),
                "amount" => desc ? query.OrderByDescending(p => p.Amount).ThenByDescending(p => p.PaymentId)
                                          : query.OrderBy(p => p.Amount).ThenBy(p => p.PaymentId),
                "status" => desc ? query.OrderByDescending(p => p.Status).ThenByDescending(p => p.PaymentId)
                                          : query.OrderBy(p => p.Status).ThenBy(p => p.PaymentId),
                "orderid" => desc ? query.OrderByDescending(p => p.OrderId).ThenByDescending(p => p.PaymentId)
                                          : query.OrderBy(p => p.OrderId).ThenBy(p => p.PaymentId),
                _ => desc ? query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.PaymentId)
                                          : query.OrderBy(p => p.CreatedAt).ThenBy(p => p.PaymentId),
            };

            // Pagination
            var page = Math.Max(parameters.PageNumber, 1);
            var size = Math.Clamp(parameters.PageSize, 1, 200);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);

            return (items, total);
        }
    }
}