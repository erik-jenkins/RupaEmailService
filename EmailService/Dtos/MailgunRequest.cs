using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
// Disabling ReSharper warnings on the next line because accessors are used behind the scenes by the JsonSerializer
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace EmailService.Dtos
{
    public class MailgunRequest
    {
        // using a string here to simplify the serialization of the address into the format "Name <email@example.com>"
        [JsonPropertyName("to")] public List<string> To { get; set; } = new();

        [JsonPropertyName("from")] public string From { get; set; } = "";

        [JsonPropertyName("subject")] public string Subject { get; set; } = "";

        [JsonPropertyName("text")] public string Text { get; set; } = "";

        public string SerializeToJson()
        {
            var encoderSettings = new TextEncoderSettings();
            encoderSettings.AllowCharacters('\u003C', '\u003E');
            encoderSettings.AllowCharacters('<', '>');
            encoderSettings.AllowRange(UnicodeRanges.BasicLatin);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(this, jsonSerializerOptions);
        }
    }
}
