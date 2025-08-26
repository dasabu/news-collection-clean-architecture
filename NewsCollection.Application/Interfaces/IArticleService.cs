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
}
