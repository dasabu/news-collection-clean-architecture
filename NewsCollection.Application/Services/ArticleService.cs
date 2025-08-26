using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Application.Mapping;

namespace NewsCollection.Application.Services;

public class ArticleService(IArticleRepository repository, IHttpContextAccessor httpContext) : IArticleService
{
    private int GetUserId() => int.Parse(httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new InvalidOperationException("User ID not found in token."));

    public async Task<List<ArticleDto>> GetAllArticlesAsync() =>
        (await repository.GetAllArticlesAsync()).Select(a => a.ToDto()).ToList();

    public async Task<ArticleDto?> GetArticleByIdAsync(int id)
    {
        var article = await repository.GetArticleByIdAsync(id);
        return article?.ToDto();
    }

    public async Task<ArticleDto?> CreateArticleAsync(CreateArticleDto request)
    {
        if (await repository.ArticleExistsAsync(request.Url))
            return null; // Article URL already exists

        var article = new Article
        {
            Headline = request.Headline,
            Summary = request.Summary,
            Url = request.Url,
            PublicationDate = request.PublicationDate,
            CategoryId = request.CategoryId
        };
        await repository.AddArticleAsync(article);
        return article.ToDto();
    }

    public async Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto request)
    {
        var article = await repository.GetArticleByIdAsync(id);
        if (article == null)
            return null; // Article not found

        if (await repository.ArticleExistsAsync(request.Url, id))
            return null; // Another article with the same URL exists

        article.Headline = request.Headline;
        article.Summary = request.Summary;
        article.Url = request.Url;
        article.PublicationDate = request.PublicationDate;
        article.CategoryId = request.CategoryId;
        await repository.UpdateArticleAsync(article);
        return article.ToDto();
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var article = await repository.GetArticleByIdAsync(id);
        if (article == null)
            return false; // Article not found

        if (await repository.IsInAnyCollectionAsync(id))
            return false; // Cannot delete article that is in any collection

        await repository.DeleteArticleAsync(id);
        return true;
    }
}