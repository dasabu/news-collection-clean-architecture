using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsCollection.Application.Interfaces;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Jobs;

public class SummaryJob(
    NewsCollectionContext _context,
    IEmailService _emailService,
    IEmailLogRepository _emailLogRepository,
    IUserRepository _userRepository,
    ILogger<SummaryJob> _logger
) {
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("SummaryJob started at {Time}", DateTime.UtcNow);
        var users = await _userRepository.GetUsersWithCollectionsAsync();

        foreach (var user in users)
        {
            try
            {
                var articles = await _context.CollectionArticles
                    .Include(ca => ca.Collection)
                    .Include(ca => ca.Article)
                    .Where(ca => ca.Collection != null &&
                                 ca.Collection.UserId == user.Id &&
                                 !ca.IsDeleted &&
                                 ca.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                    .Select(ca => new
                    {
                        CollectionName = ca.Collection != null ? ca.Collection.Name : "Unknown",
                        Headline = ca.Article != null ? ca.Article.Headline : string.Empty,
                        Summary = ca.Article != null ? ca.Article.Summary : string.Empty,
                        Url = ca.Article != null ? ca.Article.Url : string.Empty
                    })
                    .ToListAsync();

                if (articles.Any())
                {
                    var success = await _emailService.SendSummaryEmailAsync(
                        user.Email,
                        articles.Select(a => (a.CollectionName, a.Headline, a.Summary, a.Url)).ToList());
                    await _emailLogRepository.LogEmailAsync(user.Id, "summary", success, success ? null : "Failed to send summary email");
                    _logger.LogInformation("Summary email sent to {Email} with {Count} articles", user.Email, articles.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending summary email to {Email}", user.Email);
                await _emailLogRepository.LogEmailAsync(user.Id, "summary", false, ex.Message);
            }
        }
        _logger.LogInformation("SummaryJob completed at {Time}", DateTime.UtcNow);
    }
}