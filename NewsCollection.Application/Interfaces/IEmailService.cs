namespace NewsCollection.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendDigestEmailAsync(
        string toEmail,
        string frequency,
        List<(string Headline, string Summary, string Url)> articles
    );
    Task<bool> SendNotificationEmailAsync(
        string toEmail,
        (string Headline, string Summary, string Url) article
    );
    Task<bool> SendSummaryEmailAsync(
        string toEmail,
        List<(string CollectionName, string Headline, string Summary, string Url)> articles
    );
}