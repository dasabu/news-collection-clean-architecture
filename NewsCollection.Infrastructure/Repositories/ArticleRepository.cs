using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class ArticleRepository(NewsCollectionContext context) : IArticleRepository
{
    public async Task<List<Article>> GetAllArticlesAsync() =>
        await context.Articles
            .Include(a => a.Category)
            .ToListAsync();

    public async Task<Article?> GetArticleByIdAsync(int id) =>
        await context.Articles
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<bool> ArticleExistsAsync(string url, int? excludeId = null) =>
        await context.Articles
            .AnyAsync(a => a.Url.ToLower() == url.ToLower() && (excludeId == null || a.Id != excludeId));

    public async Task<bool> IsInAnyCollectionAsync(int id) =>
        await context.CollectionArticles.AnyAsync(ca => ca.ArticleId == id);

    public async Task AddArticleAsync(Article article)
    {
        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();
    }

    public async Task UpdateArticleAsync(Article article)
    {
        context.Articles.Update(article);
        await context.SaveChangesAsync();
    }

    public async Task DeleteArticleAsync(int id)
    {
        var article = await context.Articles.FindAsync(id);
        if (article != null)
        {
            context.Articles.Remove(article);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Article>> GetArticlesByCategoryAsync(int? categoryId, int page, int limit, string sortOrder)
    {
        var query = context.Articles
            .Include(a => a.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        query = sortOrder == "asc"
            ? query.OrderBy(a => a.PublicationDate)
            : query.OrderByDescending(a => a.PublicationDate);

        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Article?> GetByUrlAsync(string url) =>
        await context.Articles.FirstOrDefaultAsync(a => a.Url == url);
}