using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailService.Models;

namespace EmailService.EmailProviders
{
    public class EmailProviderPipeline
    {
        private readonly List<EmailProviderStatus> _emailProviderStatuses = new();

        public async Task SendAsync(Email email)
        {
            var emailSent = false;
            while (!emailSent)
            {
                var status = _emailProviderStatuses.FirstOrDefault(status => status.IsEnabled);
                if (status == null)
                    throw new Exception("No email providers are available to handle the request.");

                var provider = status.Provider;
                try
                {
                    await provider.SendAsync(email);
                    emailSent = true;
                }
                catch (Exception)
                {
                    status.IsEnabled = false;
                }
            }
        }

        public void AddEmailProvider<T>(T emailProvider) where T : IEmailProvider
        {
            var status = new EmailProviderStatus(emailProvider);
            _emailProviderStatuses.Add(status);
        }

        private class EmailProviderStatus
        {
            public IEmailProvider Provider { get; }
            public bool IsEnabled { get; set; } = true;

            public EmailProviderStatus(IEmailProvider provider)
            {
                Provider = provider;
            }
        }
    }
}
