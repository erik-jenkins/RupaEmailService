namespace EmailService.Configuration
{
    public class EmailProviderSettings
    {
        public const string Mailgun = "Mailgun";
        public const string Sendgrid = "Sendgrid";

        public string? Uri { get; set; }
        public string? ApiKey { get; set; }
        public int Rank { get; set; }
    }
}
