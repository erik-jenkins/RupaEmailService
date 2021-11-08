using System;
using System.Collections.Generic;
using System.Linq;
using EmailService.Configuration;
using EmailService.EmailProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmailService
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailProviders(this IServiceCollection services, IConfiguration emailConfigurationSection)
        {
            var providerSettings = services.GetProviderSettingsFromConfiguration(emailConfigurationSection);
            services.AddEmailProviderPipeline(providerSettings);
        }

        private static Dictionary<SupportedEmailProvider, EmailProviderSettings> GetProviderSettingsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var providers = new Dictionary<SupportedEmailProvider, EmailProviderSettings>();

            foreach (var child in configuration.GetChildren())
            {
                if (!Enum.TryParse<SupportedEmailProvider>(child.Key, out var provider))
                    throw new ArgumentException($"The configured provider {child.Key} is not supported.");

                services.Configure<EmailProviderSettings>(provider.ToString(), child);

                var providerSettings = child.Get<EmailProviderSettings>();
                providers.Add(provider, providerSettings);
            }

            return providers;
        }

        private static void AddEmailProviderPipeline(this IServiceCollection services, Dictionary<SupportedEmailProvider, EmailProviderSettings> providerSettingsMap)
        {
            foreach (var (provider, _) in providerSettingsMap.OrderBy(kvp => kvp.Value.Rank))
            {
                var providerType = GetEmailProviderType(provider);
                services.AddSingleton(providerType);
            }

            services.AddSingleton<IEmailProviderPipeline>(sp =>
            {
                var logger = sp.GetService<ILogger<EmailProviderPipeline>>() ?? throw new Exception("Failed to build pipeline logger");
                var pipeline = new EmailProviderPipeline(logger);

                foreach (var (provider, _) in providerSettingsMap.OrderBy(kvp => kvp.Value.Rank))
                {
                    var providerType = GetEmailProviderType(provider) ?? throw new Exception($"Failed to get provider type for provider {provider}");
                    var providerDependency = (IEmailProvider) sp.GetService(providerType)!;
                    pipeline.AddEmailProvider(providerDependency);
                }

                return pipeline;
            });
        }

        private static Type GetEmailProviderType(SupportedEmailProvider provider)
        {
            return provider switch
            {
                SupportedEmailProvider.Mailgun => typeof(MailgunEmailProvider),
                SupportedEmailProvider.Sendgrid => typeof(SendgridEmailProvider),
                SupportedEmailProvider.IntermittentFailure => typeof(IntermittentFailureEmailProvider),
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }
    }

    public enum SupportedEmailProvider
    {
        Mailgun,
        Sendgrid,
        IntermittentFailure
    }
}
