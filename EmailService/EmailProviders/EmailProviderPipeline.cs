using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailService.Exceptions;
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
        private readonly List<EmailProviderHandle> _emailProviderHandles = new();
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
                var handle = _emailProviderHandles.FirstOrDefault(handle => handle.IsEnabled());
                if (handle == null)
                    throw new Exception("No email providers are available to handle the request.");

                try
                {
                    await handle.SendAsync(email);
                    emailSent = true;
                }
                catch (FailedToSendEmailException ex)
                {
                    _logger.LogError(ex, "Failed to send email with handler {HandlerType}", handle.Provider.GetType());
                }
            }
        }

        public void AddEmailProvider<T>(T emailProvider) where T : IEmailProvider
        {
            var handle = new EmailProviderHandle(emailProvider, _logger);
            _emailProviderHandles.Add(handle);
        }

        /// <summary>
        /// A convenience class for packaging the provider along with the circuit breaker policy
        /// that will allow
        /// </summary>
        private class EmailProviderHandle
        {
            public IEmailProvider Provider { get; }
            private AsyncCircuitBreakerPolicy CircuitBreaker { get; }

            public EmailProviderHandle(IEmailProvider provider, ILogger logger)
            {
                Provider = provider;

                Action<Exception, TimeSpan> onBreak = (_, _) =>
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
                    .Handle<FailedToSendEmailException>()
                    .CircuitBreakerAsync(
                        1,
                        TimeSpan.FromSeconds(10),
                        onBreak,
                        onReset,
                        onHalfOpen);
            }

            public async Task SendAsync(Email email)
            {
                await CircuitBreaker.ExecuteAsync(async () => await Provider.SendAsync(email));
            }

            public bool IsEnabled() => CircuitBreaker.CircuitState is CircuitState.Closed or CircuitState.HalfOpen;
        }
    }
}
