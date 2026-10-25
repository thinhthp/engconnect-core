using EngConnect.Entities.Entities;
using EngConnect.Repositories.Repositories.Outbox;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EngConnect.Services.Services.Outbox
{
    // Scans the outbox table for msgs and processes them.
    public class OutboxProcessorService : IOutboxProcessorService
    {
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly IReadOnlyDictionary<string, IOutboxMessageHandler> _handlers; // Key: EventType, Value: Handler
        private readonly OutboxOptions _options;
        private readonly ILogger<OutboxProcessorService> _logger;

        public OutboxProcessorService(
            IOutboxMessageRepository outboxMessageRepository,
            IEnumerable<IOutboxMessageHandler> handlers,
            IOptions<OutboxOptions> options,
            ILogger<OutboxProcessorService> logger)
        {
            _outboxMessageRepository = outboxMessageRepository;
            _handlers = handlers
                .GroupBy(h => h.EventType, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Last(), StringComparer.OrdinalIgnoreCase);
            _options = options.Value;
            _logger = logger;
        }

        public async Task ProcessPendingAsync(CancellationToken cancellationToken = default)
        {
            var batchSize = Math.Max(_options.BatchSize, 1);
            var processingTimeout = TimeSpan.FromSeconds(Math.Max(_options.ProcessingTimeoutSeconds, 1)); // Time to lock messages for processing

            IReadOnlyList<OutboxMessage> messages;
            try
            {
                messages = await _outboxMessageRepository.ClaimPendingAsync(batchSize, processingTimeout, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to claim pending outbox messages.");
                throw;
            }

            if (messages.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Claimed {Count} outbox message(s) for processing.", messages.Count);

            foreach (var message in messages)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await ProcessMessageAsync(message, cancellationToken);
            }
        }


        // Pick a handler based on the message type and process the message.
        private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            if (!_handlers.TryGetValue(message.Type, out var handler))
            {
                var error = $"No outbox handler registered for type '{message.Type}'.";
                _logger.LogError(
                    "Dead-lettering outbox message {OutboxMessageId}: {Error}",
                    message.OutboxMessageId,
                    error);
                await _outboxMessageRepository.MarkDeadLetteredAsync(message, error, CancellationToken.None);
                return;
            }

            try
            {
                await handler.HandleAsync(message.Payload, cancellationToken);
                await _outboxMessageRepository.MarkProcessedAsync(message, CancellationToken.None);
                _logger.LogInformation(
                    "Processed outbox message {OutboxMessageId} of type {OutboxType} on attempt {AttemptCount}.",
                    message.OutboxMessageId,
                    message.Type,
                    message.AttemptCount);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    "Outbox message {OutboxMessageId} processing was cancelled on attempt {AttemptCount}; scheduling immediate retry.",
                    message.OutboxMessageId,
                    message.AttemptCount);
                await _outboxMessageRepository.MarkRetryAsync(
                    message,
                    DateTime.UtcNow,
                    "Processing cancelled.",
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                if (message.AttemptCount >= message.MaxAttempts)
                {
                    _logger.LogError(
                        ex,
                        "Dead-lettering outbox message {OutboxMessageId} of type {OutboxType} after {AttemptCount} attempt(s).",
                        message.OutboxMessageId,
                        message.Type,
                        message.AttemptCount);
                    await _outboxMessageRepository.MarkDeadLetteredAsync(message, error, CancellationToken.None);
                    return;
                }

                var delaySeconds = Math.Min(Math.Pow(2, message.AttemptCount), 300);
                var availableAt = DateTime.UtcNow.AddSeconds(delaySeconds);
                _logger.LogWarning(
                    ex,
                    "Outbox message {OutboxMessageId} of type {OutboxType} failed on attempt {AttemptCount}; retrying at {AvailableAt:u}.",
                    message.OutboxMessageId,
                    message.Type,
                    message.AttemptCount,
                    availableAt);
                await _outboxMessageRepository.MarkRetryAsync(message, availableAt, error, CancellationToken.None);
            }
        }
    }
}
