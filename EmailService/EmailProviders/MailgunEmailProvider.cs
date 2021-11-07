using System;
using System.Text.Json;
using System.Threading.Tasks;
using EmailService.Models;
using Microsoft.Extensions.Logging;

namespace EmailService.EmailProviders
{
    public class MailgunEmailProvider : IEmailProvider
    {
        private readonly ILogger<MailgunEmailProvider> _logger;
        private static int _callCount;

        public MailgunEmailProvider(ILogger<MailgunEmailProvider> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(Email email)
        {
            _callCount++;
            if (_callCount == 1 || _callCount > 20)
                throw new Exception("testing");

            _logger.LogInformation("Sending email with Mailgun: {Email}", JsonSerializer.Serialize(email));
            return Task.CompletedTask;
        }
    }
}
