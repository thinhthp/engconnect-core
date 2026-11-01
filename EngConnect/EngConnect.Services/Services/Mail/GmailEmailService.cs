using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace EngConnect.Services.Services.Mail
{
    public class GmailEmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly ILogger<GmailEmailService> _logger;

        public GmailEmailService(IConfiguration configuration, ILogger<GmailEmailService> logger)
        {
            _host = configuration["Smtp:Host"]!;
            _port = int.Parse(configuration["Smtp:Port"]!);
            _enableSsl = bool.Parse(configuration["Smtp:EnableSsl"]!);
            _userName = configuration["Smtp:UserName"]!;
            _password = configuration["Smtp:Password"]!;
            _fromEmail = configuration["Smtp:FromEmail"]!;
            _fromName = configuration["Smtp:FromName"] ?? "EngConnect";
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));
            }

            try
            {
                using var smtpClient = new SmtpClient(_host, _port)
                {
                    EnableSsl = _enableSsl,
                    Credentials = new NetworkCredential(_userName, _password)
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(toEmail));

                await smtpClient.SendMailAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email via SMTP host {SmtpHost}:{SmtpPort} to {ToEmail} with subject {Subject}.",
                    _host,
                    _port,
                    toEmail,
                    subject);
                throw;
            }
        }
    }
}
