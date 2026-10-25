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
    public class GmailEmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSsl;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public GmailEmailService(IConfiguration configuration)
        {
            _host = configuration["Smtp:Host"]!;
            _port = int.Parse(configuration["Smtp:Port"]!);
            _enableSsl = bool.Parse(configuration["Smtp:EnableSsl"]!);
            _userName = configuration["Smtp:UserName"]!;
            _password = configuration["Smtp:Password"]!;
            _fromEmail = configuration["Smtp:FromEmail"]!;
            _fromName = configuration["Smtp:FromName"] ?? "EngConnect";
        }

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