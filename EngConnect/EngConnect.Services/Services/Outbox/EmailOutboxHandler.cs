using EngConnect.Entities.Entities;
using EngConnect.Services.Services.Mail;
using System.Text.Json;

namespace EngConnect.Services.Services.Outbox
{
    // Handles the processing of email outbox messages (deserializing the payload, validating it, and sending the email using the IEmailService.
    public class EmailOutboxHandler : IOutboxMessageHandler
    {
        private readonly IEmailService _emailService;

        public EmailOutboxHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public string EventType => OutboxMessageTypes.Email;

        public async Task HandleAsync(string payload, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new InvalidOperationException("Email outbox payload is empty.");
            }

            EmailOutboxPayload? email;
            try
            {
                email = JsonSerializer.Deserialize<EmailOutboxPayload>(payload);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Email outbox payload is not valid JSON.", ex);
            }

            if (email == null
                || string.IsNullOrWhiteSpace(email.ToEmail)
                || string.IsNullOrWhiteSpace(email.Subject)
                || string.IsNullOrWhiteSpace(email.HtmlContent))
            {
                throw new InvalidOperationException("Email outbox payload is missing toEmail, subject, or htmlContent.");
            }

            await _emailService.SendEmailAsync(email.ToEmail, email.Subject, email.HtmlContent, cancellationToken);
        }
    }
}
