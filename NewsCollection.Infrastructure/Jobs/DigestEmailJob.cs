using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsCollection.Application.Interfaces;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Jobs;

public class DigestEmailJob(
    NewsCollectionContext context,
    IEmailService emailService,
    IEmailLogRepository emailLogRepository,
    IUserRepository userRepository,
    ILogger<DigestEmailJob> logger
) {
    public async Task ExecuteAsync()
    {
        logger.LogInformation("DigestEmailJob started at {Time}", DateTime.UtcNow);
        var users = await userRepository.GetUsersWithSubscriptionsAsync("daily");

        foreach (var user in users)
        {
            try
            {
                var categoryIds = user.Subscriptions.Where(s => s.IsActive).Select(s => s.CategoryId).ToList();
                if (!categoryIds.Any()) continue;

                var articles = await context.Articles
                    .Where(a => categoryIds.Contains(a.CategoryId) && a.FetchedAt >= DateTime.UtcNow.AddDays(-1))
                    .Select(a => new { a.Headline, a.Summary, a.Url })
                    .ToListAsync();

                if (articles.Any())
                {
                    var success = await emailService.SendDigestEmailAsync(
                        user.Email,
                        user.SubscriptionFrequency,
                        articles.Select(a => (a.Headline, a.Summary, a.Url)).ToList());
                    await emailLogRepository.LogEmailAsync(user.Id, "digest", success, success ? null : "Failed to send digest email");
                    logger.LogInformation("Digest email sent to {Email} with {Count} articles", user.Email, articles.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending digest email to {Email}", user.Email);
                await emailLogRepository.LogEmailAsync(user.Id, "digest", false, ex.Message);
            }
        }
        logger.LogInformation("DigestEmailJob completed at {Time}", DateTime.UtcNow);
    }
}