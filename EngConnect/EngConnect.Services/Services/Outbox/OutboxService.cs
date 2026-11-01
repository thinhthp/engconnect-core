using EngConnect.Entities.Entities;
using EngConnect.Repositories.Common;
using EngConnect.Repositories.Repositories.Outbox;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EngConnect.Services.Services.Outbox
{
    // Save outbox msgs to the db.
    public class OutboxService : IOutboxService
    {
        private readonly IOutboxMessageRepository _outboxMessageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly OutboxOptions _options;
        private readonly ILogger<OutboxService> _logger;

        public OutboxService(
            IOutboxMessageRepository outboxMessageRepository,
            IUnitOfWork unitOfWork,
            IOptions<OutboxOptions> options,
            ILogger<OutboxService> logger)
        {
            _outboxMessageRepository = outboxMessageRepository;
            _unitOfWork = unitOfWork;
            _options = options.Value;
            _logger = logger;
        }

        public async Task EnqueueAsync(string type, string payload, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Outbox message type is required.", nameof(type));
            }

            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new ArgumentException("Outbox message payload is required.", nameof(payload));
            }

            var now = DateTime.UtcNow;
            var message = new OutboxMessage
            {
                Type = type.Trim(),
                Payload = payload,
                Status = OutboxMessageStatuses.Pending,
                AttemptCount = 0,
                MaxAttempts = Math.Max(_options.MaxAttempts, 1),
                AvailableAt = now,
                CreatedAt = now
            };

            await _outboxMessageRepository.AddAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Enqueued outbox message {OutboxMessageId} of type {OutboxType}.",
                message.OutboxMessageId,
                message.Type);
        }

        public Task EnqueueEmailAsync(string toEmail, string subject, string htmlContent, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Email subject is required.", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException("Email body is required.", nameof(htmlContent));
            }

            var payload = JsonSerializer.Serialize(new EmailOutboxPayload
            {
                ToEmail = toEmail.Trim(),
                Subject = subject,
                HtmlContent = htmlContent
            });

            return EnqueueAsync(OutboxMessageTypes.Email, payload, cancellationToken);
        }
    }
}
