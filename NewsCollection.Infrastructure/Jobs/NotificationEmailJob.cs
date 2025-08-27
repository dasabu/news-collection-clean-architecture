using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsCollection.Application.Interfaces;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Jobs;

public class NotificationJob(
    NewsCollectionContext context,
    IEmailService emailService,
    IEmailLogRepository emailLogRepository,
    IUserRepository userRepository,
    ILogger<NotificationJob> logger
) {
    public async Task ExecuteAsync()
    {
        logger.LogInformation("NotificationJob started at {Time}", DateTime.UtcNow);
        var users = await userRepository.GetUsersWithActiveSubscriptionsAsync();

        foreach (var user in users)
        {
            try
            {
                var categoryIds = user.Subscriptions.Where(s => s.IsActive).Select(s => s.CategoryId).ToList();
                if (!categoryIds.Any()) continue;

                // get latest LastNotified or null if all subscriptions still don't have LastNotified
                var lastNotified = user.Subscriptions.Where(s => s.IsActive).Max(s => s.LastNotified) ?? DateTime.MinValue;

                var articles = await context.Articles
                    .Where(a => categoryIds.Contains(a.CategoryId) &&
                                a.FetchedAt >= DateTime.UtcNow.AddHours(-1) &&
                                a.FetchedAt > lastNotified)
                    .Select(a => new { a.Headline, a.Summary, a.Url })
                    .ToListAsync();

                foreach (var article in articles)
                {
                    var success = await emailService.SendNotificationEmailAsync(
                        user.Email,
                        (article.Headline, article.Summary, article.Url));
                    await emailLogRepository.LogEmailAsync(user.Id, "notification", success, success ? null : "Failed to send notification email");
                    logger.LogInformation("Notification email sent to {Email} for article {Headline}", user.Email, article.Headline);

                    var subscriptions = user.Subscriptions.Where(s => s.IsActive && categoryIds.Contains(s.CategoryId));
                    foreach (var subscription in subscriptions)
                    {
                        subscription.LastNotified = DateTime.UtcNow;
                    }
                    await userRepository.UpdateUserSubscriptionsAsync(user);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending notification email to {Email}", user.Email);
                await emailLogRepository.LogEmailAsync(user.Id, "notification", false, ex.Message);
            }
        }
        logger.LogInformation("NotificationJob completed at {Time}", DateTime.UtcNow);
    }
}