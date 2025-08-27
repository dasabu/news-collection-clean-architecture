using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface IArticleRepository
{
    Task<List<Article>> GetAllArticlesAsync();
    Task<Article?> GetArticleByIdAsync(int id);
    Task<bool> ArticleExistsAsync(string url, int? excludeId = null);
    Task<bool> IsInAnyCollectionAsync(int id);
    Task AddArticleAsync(Article article);
    Task UpdateArticleAsync(Article article);
    Task DeleteArticleAsync(int id);
    Task<List<Article>> GetArticlesByCategoryAsync(int? categoryId, int page, int limit, string sortOrder);

    Task<Article?> GetByUrlAsync(string url);
}