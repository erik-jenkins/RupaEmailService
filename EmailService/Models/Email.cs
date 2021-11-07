namespace EmailService.Models
{
    public record Email(
        string To,
        string ToName,
        string From,
        string FromName,
        string Subject,
        string Body);
}
