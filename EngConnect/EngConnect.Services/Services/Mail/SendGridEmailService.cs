using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Services.Services.Mail
{
    public class SendGridEmailService : IEmailService
    {
        //private readonly string _apiKey;
        //private readonly string _fromEmail;
        //private readonly string _fromName;

        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailService(IConfiguration configuration)
        {
            //_apiKey = configuration["SendGrid:ApiKey"]!;
            //_fromEmail = configuration["SendGrid:FromEmail"]!;
            //_fromName = configuration["SendGrid:FromName"] ?? "EngConnect";

            _host = configuration["Smtp:Host"]!;
            _port = int.Parse(configuration["Smtp:Port"]!);
            _enableSsl = bool.Parse(configuration["Smtp:EnableSsl"]!);
            _userName = configuration["Smtp:UserName"]!;
            _password = configuration["Smtp:Password"]!;
            _fromEmail = configuration["Smtp:FromEmail"]!;
            _fromName = configuration["Smtp:FromName"] ?? "EngConnect";
        }

        //public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        //{
        //    var client = new SendGridClient(_apiKey);
        //    var from = new EmailAddress(_fromEmail, _fromName);
        //    var to = new EmailAddress(toEmail);
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);
        //    var response = await client.SendEmailAsync(msg);

        //    // Handle non-success responses (logging, retries, etc.)
        //}

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
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

            await smtpClient.SendMailAsync(message);
        }
    }
}