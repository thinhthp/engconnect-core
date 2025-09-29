using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Mail
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailService(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:ApiKey"]!;
            _fromEmail = configuration["SendGrid:FromEmail"]!;
            _fromName = configuration["SendGrid:FromName"] ?? "EngConnect";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);
            var response = await client.SendEmailAsync(msg);

            // Handle non-success responses (logging, retries, etc.)
        }
    }
}