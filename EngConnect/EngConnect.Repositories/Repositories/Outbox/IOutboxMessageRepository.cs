using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;

namespace EngConnect.Repositories.Repositories.Outbox
{
    public interface IOutboxMessageRepository : IGenericRepository<OutboxMessage>
    {
        Task<IReadOnlyList<OutboxMessage>> ClaimPendingAsync(
            int batchSize,
            TimeSpan processingTimeout,
            CancellationToken cancellationToken = default);

        Task MarkProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        Task MarkRetryAsync(
            OutboxMessage message,
            DateTime availableAt,
            string? lastError,
            CancellationToken cancellationToken = default);

        Task MarkDeadLetteredAsync(
            OutboxMessage message,
            string? lastError,
            CancellationToken cancellationToken = default);
    }
}
