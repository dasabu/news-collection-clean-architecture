using System;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewsCollection.Application.Interfaces;

namespace NewsCollection.Infrastructure.Providers;

public class NewsApiProvider(
    IConfiguration configuration,
    ILogger<NewsApiProvider> logger,
    HttpClient httpClient
) : INewsApiProvider
{
    public async Task<List<(string Headline, string Summary, string Url, DateTime PublicationDate)>> GetTopHeadlinesAsync(string category)
    {
        logger.LogInformation("Fetching top headlines for category {Category}", category);
        var apiKey = configuration["NewsApi:ApiKey"];
        
        // Check if API key is configured
        if (string.IsNullOrEmpty(apiKey))
        {
            logger.LogError("NewsApi:ApiKey is not configured in appsettings.json");
            return new List<(string, string, string, DateTime)>();
        }
        
        var fetchedUrl = $"https://newsapi.org/v2/top-headlines?category={category}&apiKey={apiKey}";    
        logger.LogInformation("Fetching url: {FetchedUrl}", fetchedUrl);    

        // Create request with User-Agent header
        var request = new HttpRequestMessage(HttpMethod.Get, fetchedUrl);
        request.Headers.Add("User-Agent", "NewsCollection/1.0");
        
        var response = await httpClient.SendAsync(request);
        logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to fetch articles for {Category}. Status: {StatusCode}, Error: {ErrorContent}", 
                category, response.StatusCode, errorContent);
            return new List<(string, string, string, DateTime)>();
        }

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        var articles = json.RootElement.GetProperty("articles").EnumerateArray();
        var result = new List<(string Headline, string Summary, string Url, DateTime PublicationDate)>();

        foreach (var article in articles)
        {
            var headline = article.GetProperty("title").GetString() ?? string.Empty;
            var summary = article.GetProperty("description").GetString() ?? string.Empty;
            var url = article.GetProperty("url").GetString() ?? string.Empty;
            var publicationDate = article.GetProperty("publishedAt").GetDateTime();
            result.Add((headline, summary, url, publicationDate));
        }

        logger.LogInformation("Fetched {Count} articles for {Category}", result.Count, category);
        return result;
    }
}