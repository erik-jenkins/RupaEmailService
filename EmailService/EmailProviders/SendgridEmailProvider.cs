using System.Text.Json;
using System.Threading.Tasks;
using EmailService.Models;
using Microsoft.Extensions.Logging;

namespace EmailService.EmailProviders
{
    public class SendgridEmailProvider : IEmailProvider
    {
        private readonly ILogger<SendgridEmailProvider> _logger;

        public SendgridEmailProvider(ILogger<SendgridEmailProvider> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(Email email)
        {
            var emailJson = JsonSerializer.Serialize(email);
            _logger.LogInformation("Sending email with Sendgrid: {Email}", emailJson);
            return Task.CompletedTask;
        }
    }
}
