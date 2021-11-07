namespace EmailService.Configuration
{
    public class EmailProviderSettings
    {
        public const string Mailgun = "Mailgun";
        public const string Sendgrid = "Sendgrid";

        public static string? ApiKey { get; set; }
        public int Rank { get; set; }
    }
}
