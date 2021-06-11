using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Ordering.Infrastrucuture.Mail
{
    public class EmailService : IEmailService
    {

        public EmailsSettings _emailsSettings { get; }
        public ILogger<EmailService> _logger { get; }

        public EmailService(IOptions<EmailsSettings> emailsSettings, ILogger<EmailService> logger)
        {
            _emailsSettings = emailsSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmail(Email email)
        {
            var client = new SendGridClient(_emailsSettings.ApiKey);

            var subject = email.Subject;
            var to = new EmailAddress(email.To);
            var emailBody = email.Body;

            var from = new EmailAddress
            {
                Email = _emailsSettings.FromAddress,
                Name = _emailsSettings.FromName
            };

            var sendgridMessage = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);
            var response = await client.SendEmailAsync(sendgridMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;

            _logger.LogError("Email sendig failed.");
            return false;
        }
    }
}
