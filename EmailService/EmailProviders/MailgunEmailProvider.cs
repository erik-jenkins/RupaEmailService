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

        public MailgunEmailProvider(ILogger<MailgunEmailProvider> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(Email email)
        {
            _logger.LogInformation("Sending email with Mailgun: {Email}", JsonSerializer.Serialize(email));
            return Task.CompletedTask;
        }
    }
}
