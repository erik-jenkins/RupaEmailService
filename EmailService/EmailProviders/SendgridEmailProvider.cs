using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EmailService.Configuration;
using EmailService.Dtos;
using EmailService.Exceptions;
using EmailService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmailService.EmailProviders
{
    public class SendgridEmailProvider : IEmailProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SendgridEmailProvider> _logger;
        private readonly string _uri;
        private readonly string _apiKey;

        public SendgridEmailProvider(
            IOptionsMonitor<EmailProviderSettings> optionsMonitor,
            IHttpClientFactory httpClientFactory,
            ILogger<SendgridEmailProvider> logger)
        {
            var sendgridOptions = optionsMonitor.Get(EmailProviderSettings.Sendgrid) ?? throw new ArgumentException("Failed to load options");
            _uri = sendgridOptions.Uri ?? throw new ArgumentException("Sendgrid settings did not contain URI");
            _apiKey = sendgridOptions.ApiKey ?? throw new ArgumentException("Sendgrid settings did not contain API key");
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task SendAsync(Email email)
        {
            var request = BuildRequest(email);
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            try
            {
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Sent email using Sendgrid.");
            }
            catch (HttpRequestException ex)
            {
                throw new FailedToSendEmailException("Failed to send email using Sendgrid", ex);
            }
        }

        private HttpRequestMessage BuildRequest(Email email)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.RequestUri = new Uri(_uri);

            var sendgridRequest = BuildSendgridRequest(email);
            var json = JsonSerializer.Serialize(sendgridRequest);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return request;
        }

        private SendgridRequest BuildSendgridRequest(Email email)
        {
            var request = new SendgridRequest
            {
                Personalizations = new()
                {
                    new SendgridRequestPersonalization
                    {
                        To = new() {new () {Email = email.To, Name = email.ToName}},
                        Subject = email.Subject
                    }
                },
                Content = new()
                {
                    new() {Type = "text/plain", Value = email.Body}
                },
                From = new() {Email = email.From, Name = email.FromName},
                ReplyTo = new() {Email = email.From, Name = email.FromName}
            };

            return request;
        }
    }
}
