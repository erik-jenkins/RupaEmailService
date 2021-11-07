using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailService.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace EmailService.EmailProviders
{
    public interface IEmailProviderPipeline
    {
        Task SendAsync(Email email);
    }

    public class EmailProviderPipeline : IEmailProviderPipeline
    {
        private readonly List<EmailProviderStatus> _emailProviderStatuses = new();
        private readonly ILogger<EmailProviderPipeline> _logger;

        public EmailProviderPipeline(ILogger<EmailProviderPipeline> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(Email email)
        {
            var emailSent = false;
            while (!emailSent)
            {
                var status = _emailProviderStatuses.FirstOrDefault(status => status.IsEnabled());
                if (status == null)
                    throw new Exception("No email providers are available to handle the request.");

                try
                {
                    await status.CircuitBreaker.ExecuteAsync(async () => await status.Provider.SendAsync(email));
                    emailSent = true;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void AddEmailProvider<T>(T emailProvider) where T : IEmailProvider
        {
            var status = new EmailProviderStatus(emailProvider, _logger);
            _emailProviderStatuses.Add(status);
        }

        private class EmailProviderStatus
        {
            public IEmailProvider Provider { get; }
            public AsyncCircuitBreakerPolicy CircuitBreaker { get; }

            public EmailProviderStatus(IEmailProvider provider, ILogger logger)
            {
                Provider = provider;

                Action<Exception, TimeSpan> onBreak = (_, duration) =>
                {
                    logger.LogInformation("Error threshold for provider {Provider} exceeded. Failing over to next option.", Provider);
                };

                Action onReset = () =>
                {
                    logger.LogInformation("Email was successfully sent using Provider {Provider}.", Provider);
                };

                Action onHalfOpen = () =>
                {
                    logger.LogInformation("Provider {Provider} circuit breaker duration exceeded. Attempting to send email.", provider);
                };

                CircuitBreaker = Policy
                    .Handle<Exception>()
                    .CircuitBreakerAsync(
                        1,
                        TimeSpan.FromSeconds(10),
                        onBreak,
                        onReset,
                        onHalfOpen);
            }

            public bool IsEnabled() => CircuitBreaker.CircuitState is CircuitState.Closed or CircuitState.HalfOpen;
        }
    }
}
