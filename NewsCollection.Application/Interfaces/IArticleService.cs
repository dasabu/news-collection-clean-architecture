using System;
using NewsCollection.Application.Dtos;

namespace NewsCollection.Application.Interfaces;

public interface IArticleService
{
    Task<List<ArticleDto>> GetAllArticlesAsync();
    Task<ArticleDto?> GetArticleByIdAsync(int id);
    Task<ArticleDto?> CreateArticleAsync(CreateArticleDto request);
    Task<ArticleDto?> UpdateArticleAsync(int id, UpdateArticleDto request);
    Task<bool> DeleteArticleAsync(int id);
    Task<List<ArticleDto>> GetArticlesByCategoryAsync(int? categoryId, int page, int limit, string sortOrder); // Get articles pagination
    
    // for NewsSyncJob to save article into DB
    Task AddOrUpdateArticleAsync(string headline, string summary, string url, DateTime publicationDate, int categoryId);
}
