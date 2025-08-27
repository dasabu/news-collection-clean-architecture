using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewsCollection.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace NewsCollection.Infrastructure.Providers;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    public async Task<bool> SendDigestEmailAsync(string toEmail, string frequency, List<(string Headline, string Summary, string Url)> articles)
    {
        if (!articles.Any())
        {
            logger.LogInformation("No articles to send in digest email to {Email}", toEmail);
            return true;
        }

        var client = new SendGridClient(configuration["SendGrid:ApiKey"]);
        var from = new EmailAddress(configuration["SendGrid:SenderEmail"], "NewsCollection");
        var subject = $"Your {frequency} News Digest";
        var htmlContent = BuildDigestHtml(articles);
        var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(toEmail), subject, null, htmlContent);

        try
        {
            var response = await client.SendEmailAsync(msg);
            logger.LogInformation("Digest email sent to {Email}, Status: {StatusCode}", toEmail, response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send digest email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendNotificationEmailAsync(string toEmail, (string Headline, string Summary, string Url) article)
    {
        var client = new SendGridClient(configuration["SendGrid:ApiKey"]);
        var from = new EmailAddress(configuration["SendGrid:SenderEmail"], "NewsCollection");
        var subject = "New Article Notification";
        var htmlContent = BuildNotificationHtml(article);
        var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(toEmail), subject, null, htmlContent);

        try
        {
            var response = await client.SendEmailAsync(msg);
            logger.LogInformation("Notification email sent to {Email}, Status: {StatusCode}", toEmail, response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send notification email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendSummaryEmailAsync(string toEmail, List<(string CollectionName, string Headline, string Summary, string Url)> articles)
    {
        if (!articles.Any())
        {
            logger.LogInformation("No articles to send in summary email to {Email}", toEmail);
            return true;
        }

        var client = new SendGridClient(configuration["SendGrid:ApiKey"]);
        var from = new EmailAddress(configuration["SendGrid:SenderEmail"], "NewsCollection");
        var subject = "Your Weekly Collection Summary";
        var htmlContent = BuildSummaryHtml(articles);
        var msg = MailHelper.CreateSingleEmail(from, new EmailAddress(toEmail), subject, null, htmlContent);

        try
        {
            var response = await client.SendEmailAsync(msg);
            logger.LogInformation("Summary email sent to {Email}, Status: {StatusCode}", toEmail, response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send summary email to {Email}", toEmail);
            return false;
        }
    }

    private static string BuildDigestHtml(List<(string Headline, string Summary, string Url)> articles)
    {
        var sb = new StringBuilder();
        sb.Append("<h1>Your News Digest</h1><ul>");
        foreach (var article in articles)
        {
            sb.Append($"<li><a href=\"{article.Url}\">{article.Headline}</a>: {article.Summary}</li>");
        }
        sb.Append("</ul><p><a href=\"https://yourapp.com/unsubscribe\">Unsubscribe</a></p>");
        return sb.ToString();
    }

    private static string BuildNotificationHtml((string Headline, string Summary, string Url) article)
    {
        return $"<h1>New Article</h1><p><a href=\"{article.Url}\">{article.Headline}</a>: {article.Summary}</p><p><a href=\"https://yourapp.com/unsubscribe\">Unsubscribe</a></p>";
    }

    private static string BuildSummaryHtml(List<(string CollectionName, string Headline, string Summary, string Url)> articles)
    {
        var sb = new StringBuilder();
        sb.Append("<h1>Your Weekly Collection Summary</h1>");
        var groupedArticles = articles.GroupBy(a => a.CollectionName);
        foreach (var group in groupedArticles)
        {
            sb.Append($"<h2>{group.Key}</h2><ul>");
            foreach (var article in group)
            {
                sb.Append($"<li><a href=\"{article.Url}\">{article.Headline}</a>: {article.Summary}</li>");
            }
            sb.Append("</ul>");
        }
        sb.Append("<p><a href=\"https://yourapp.com/unsubscribe\">Unsubscribe</a></p>");
        return sb.ToString();
    }
}