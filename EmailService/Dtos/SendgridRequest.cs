using System.Collections.Generic;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace EmailService.Dtos
{
    public class SendgridRequest
    {
        [JsonPropertyName("personalizations")] public List<SendgridRequestPersonalization> Personalizations { get; set; } = new();

        [JsonPropertyName("content")] public List<SendgridRequestContent> Content { get; set; } = new();

        [JsonPropertyName("from")] public SendgridRequestAddress From { get; set; } = new();

        [JsonPropertyName("reply_to")] public SendgridRequestAddress ReplyTo { get; set; } = new();
    }

    public class SendgridRequestPersonalization
    {
        [JsonPropertyName("to")] public List<SendgridRequestAddress> To { get; set; } = new();

        [JsonPropertyName("subject")] public string Subject { get; set; } = string.Empty;
    }

    public class SendgridRequestAddress
    {
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;

        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    }

    public class SendgridRequestContent
    {
        [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

        [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
    }
}
