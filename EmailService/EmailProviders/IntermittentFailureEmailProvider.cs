using System;
using System.Text.Json;
using System.Threading.Tasks;
using EmailService.Exceptions;
using EmailService.Models;
using Microsoft.Extensions.Logging;

namespace EmailService.EmailProviders
{
    public class IntermittentFailureEmailProvider : IEmailProvider
    {
        private readonly ILogger<IntermittentFailureEmailProvider> _logger;
        private static int _callCount;

        public IntermittentFailureEmailProvider(ILogger<IntermittentFailureEmailProvider> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(Email email)
        {
            _callCount++;
            if (_callCount % 5 == 0)
                throw new FailedToSendEmailException();

            _logger.LogInformation("Sending email with IntermittentFailureEmailProvider: {Email}", JsonSerializer.Serialize(email));
            return Task.CompletedTask;
        }
    }
}
