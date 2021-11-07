using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EmailService.Models;

namespace EmailService.Dtos
{
    public class SendEmailRequest
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("to")]
        public string To { get; set; } = "";

        [Required]
        [EmailAddress]
        [JsonPropertyName("to_name")]
        public string ToName { get; set; } = "";

        [Required]
        [EmailAddress]
        [JsonPropertyName("from")]
        public string From { get; set; } = "";

        [Required]
        [EmailAddress]
        [JsonPropertyName("from_name")]
        public string FromName { get; set; } = "";

        [Required]
        [EmailAddress]
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = "";

        [Required]
        [EmailAddress]
        [JsonPropertyName("body")]
        public string Body { get; set; } = "";

        public static explicit operator Email(SendEmailRequest request) => new(request.To, request.ToName, request.From, request.FromName, request.Subject, request.Body);
    }
}
