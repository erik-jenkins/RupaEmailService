using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using EmailService.Configuration;
using EmailService.Dtos;
using EmailService.Exceptions;
using EmailService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace EmailService.EmailProviders
{
    public class MailgunEmailProvider : IEmailProvider
    {
        private readonly ILogger<MailgunEmailProvider> _logger;
        private readonly string _domain;
        private readonly string _apiKey;
        private const string MailgunBaseUri = "https://api.mailgun.net/v3";

        public MailgunEmailProvider(
            IOptionsMonitor<EmailProviderSettings> optionsMonitor,
            ILogger<MailgunEmailProvider> logger)
        {
            var mailgunOptions = optionsMonitor.Get(EmailProviderSettings.Mailgun);
            _domain = mailgunOptions.Uri ?? throw new ArgumentException("Mailgun settings did not contain URI");
            _apiKey = mailgunOptions.ApiKey ?? throw new ArgumentException("Mailgun settings did not contain API key");

            _logger = logger;
        }

        public async Task SendAsync(Email email)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(MailgunBaseUri);
            client.Authenticator = new HttpBasicAuthenticator("api", _apiKey);

            var request = new RestRequest();
            request.AddParameter("domain", _domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter ("from", GetMailgunAddressString(email.FromName, email.From));
            request.AddParameter ("to", GetMailgunAddressString(email.ToName, email.To));
            request.AddParameter ("subject", email.Subject);
            request.AddParameter ("text", email.Body);
            request.Method = Method.POST;
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new FailedToSendEmailException($"Failed to send email using Mailgun. Response code: {response.StatusCode}");

            _logger.LogInformation("Sent email using Mailgun.");
        }

        private HttpRequestMessage BuildRequest(Email email)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", $"Basic {_apiKey}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", GetBase64AuthString());
            request.RequestUri = new Uri(_domain);

            var mailgunRequest = BuildMailgunRequest(email);
            request.Content = new StringContent(mailgunRequest.SerializeToJson(), Encoding.UTF8, "application/json");

            return request;
        }

        private string GetBase64AuthString()
        {
            var authString = $"api:{_apiKey}";
            var authStringBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            return authStringBase64;
        }

        private MailgunRequest BuildMailgunRequest(Email email)
        {
            return new MailgunRequest
            {
                To = new()
                {
                    GetMailgunAddressString(email.ToName, email.To),
                },
                From = GetMailgunAddressString(email.FromName, email.From),
                Subject = email.Subject,
                Text = email.Body
            };
        }

        private string GetMailgunAddressString(string name, string email) => $"{name} \u003C{email}\u003E";
    }
}
