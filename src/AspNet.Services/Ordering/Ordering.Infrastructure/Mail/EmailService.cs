using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Mail
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings { get; }
        public ILogger<EmailService> _logger { get; }

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendEmail(Email email)
        {
            BaseClient client = new SendGridClient(_emailSettings.ApiKey);

            EmailAddress to = new EmailAddress{ Email = email.To };

            EmailAddress from = new EmailAddress
            {
                Email = _emailSettings.FromAddress,
                Name = _emailSettings.FromName
            };

            SendGridMessage emailMessage = MailHelper.CreateSingleEmail(from, to, email.Subject, email.Body, email.Body);

            Response response = await client.SendEmailAsync(emailMessage);

            _logger.LogInformation("Email sent.");

            if (!response.IsSuccessStatusCode) _logger.LogError("There was an error sending the email.");

            return response.IsSuccessStatusCode;
        }
    }
}
