using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace EngConnect.Repositories.Repositories.Outbox
{
    public class OutboxMessageRepository : GenericRepository<OutboxMessage>, IOutboxMessageRepository
    {
        private const int LastErrorMaxLength = 2000;

        public OutboxMessageRepository(EngConnectContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<OutboxMessage>> ClaimPendingAsync(
            int batchSize,
            TimeSpan processingTimeout,
            CancellationToken cancellationToken = default)
        {
            if (batchSize <= 0)
            {
                return Array.Empty<OutboxMessage>();
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var messages = await _dbSet
                    .FromSqlInterpolated($@"
                        SELECT
                            outbox_message_id,
                            type,
                            payload,
                            status,
                            attempt_count,
                            max_attempts,
                            available_at,
                            locked_until,
                            last_error,
                            processed_at,
                            created_at,
                            update_date
                        FROM outbox_message
                        WHERE (status = {OutboxMessageStatuses.Pending} AND available_at <= NOW())
                           OR (status = {OutboxMessageStatuses.Processing} AND locked_until IS NOT NULL AND locked_until < NOW())
                        ORDER BY outbox_message_id
                        LIMIT {batchSize}
                        FOR UPDATE SKIP LOCKED")
                    .ToListAsync(cancellationToken);

                if (messages.Count == 0)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return messages;
                }

                var now = DateTime.UtcNow;
                var lockedUntil = now.Add(processingTimeout);

                foreach (var message in messages)
                {
                    message.Status = OutboxMessageStatuses.Processing;
                    message.AttemptCount += 1;
                    message.LockedUntil = lockedUntil;
                    message.UpdateDate = now;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return messages;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task MarkProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            message.Status = OutboxMessageStatuses.Processed;
            message.ProcessedAt = now;
            message.LockedUntil = null;
            message.LastError = null;
            message.UpdateDate = now;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkRetryAsync(
            OutboxMessage message,
            DateTime availableAt,
            string? lastError,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            message.Status = OutboxMessageStatuses.Pending;
            message.AvailableAt = DateTime.SpecifyKind(availableAt, DateTimeKind.Utc);
            message.LockedUntil = null;
            message.LastError = TruncateError(lastError);
            message.UpdateDate = now;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkDeadLetteredAsync(
            OutboxMessage message,
            string? lastError,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            message.Status = OutboxMessageStatuses.DeadLettered;
            message.LockedUntil = null;
            message.LastError = TruncateError(lastError);
            message.UpdateDate = now;
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static string? TruncateError(string? lastError)
        {
            if (string.IsNullOrEmpty(lastError))
            {
                return lastError;
            }

            return lastError.Length <= LastErrorMaxLength
                ? lastError
                : lastError[..LastErrorMaxLength];
        }
    }
}
