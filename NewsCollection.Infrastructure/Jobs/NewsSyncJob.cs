using Microsoft.Extensions.Logging;
using NewsCollection.Application.Interfaces;
using System.Threading.Tasks;
using System;

namespace NewsCollection.Infrastructure.Jobs;

public class NewsSyncJob(
    IArticleService articleService,
    INewsApiProvider newsApiProvider,
    ILogger<NewsSyncJob> logger
) {
    public async Task ExecuteAsync()
    {
        logger.LogInformation("NewsSyncJob started at {Time}", DateTime.UtcNow);
        var categories = new[] { "technology", "business", "health", "sports", "science" };

        for (int i = 0; i < categories.Length; i++)
        {
            try
            {
                if (i > 0)
                {
                    // delay between API calls to avoid rate limiting
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    logger.LogInformation("Waiting {WaitSeconds} seconds between API calls for category {Category}", 2, categories[i]);
                }
                
                var articles = await newsApiProvider.GetTopHeadlinesAsync(categories[i]);
                foreach (var article in articles)
                {
                    await articleService.AddOrUpdateArticleAsync(
                        article.Headline,
                        article.Summary,
                        article.Url,
                        article.PublicationDate,
                        i + 1); // category from 1 to 5 
                }
                logger.LogInformation("Synced {Count} articles for {Category}", articles.Count, categories[i]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error syncing articles for {Category}", categories[i]);
            }
        }
        logger.LogInformation("NewsSyncJob completed at {Time}", DateTime.UtcNow);
    }
}