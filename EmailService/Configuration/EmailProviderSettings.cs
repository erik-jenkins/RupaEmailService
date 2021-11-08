namespace EmailService.Configuration
{
    public class EmailProviderSettings
    {
        public const string Mailgun = "Mailgun";
        public const string Sendgrid = "Sendgrid";

        public string? Uri { get; set; } = null;
        public string? ApiKey { get; set; } = null;
        public int Rank { get; set; }
    }
}
