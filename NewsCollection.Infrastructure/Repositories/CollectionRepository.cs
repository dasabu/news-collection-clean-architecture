using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class CollectionRepository(NewsCollectionContext context) : ICollectionRepository
{
    public async Task<(List<Collection> Items, int TotalCount)> GetCollectionsByUserIdAsync(int userId, int page, int limit)
    {
        var query = context.Collections
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Include(c => c.Articles);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Collection?> GetCollectionByIdAsync(int id) =>
        await context.Collections
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

    public async Task<bool> CollectionExistsAsync(string name, int userId, int? excludeId = null) =>
        await context.Collections
            .AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.UserId == userId && (excludeId == null || c.Id != excludeId) && !c.IsDeleted);

    public async Task<bool> HasArticlesAsync(int id) =>
        await context.CollectionArticles.AnyAsync(ca => ca.CollectionId == id && !ca.IsDeleted);

    public async Task AddCollectionAsync(Collection collection)
    {
        await context.Collections.AddAsync(collection);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCollectionAsync(Collection collection)
    {
        context.Collections.Update(collection);
        await context.SaveChangesAsync();
    }

    public async Task DeleteCollectionAsync(int id)
    {
        var collectionArticles = await context.CollectionArticles
            .IgnoreQueryFilters()
            .Where(ca => ca.CollectionId == id)
            .ToListAsync();

        foreach (var ca in collectionArticles)
        {
            ca.IsDeleted = true;
        }

        var collection = await context.Collections
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id);
        if (collection != null)
        {
            collection.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ArticleExistsInCollectionAsync(int collectionId, int articleId) =>
        await context.CollectionArticles
            .AnyAsync(ca => ca.CollectionId == collectionId && ca.ArticleId == articleId && !ca.IsDeleted);

    public async Task AddArticleToCollectionAsync(int collectionId, int articleId)
    {
        // Check for an existing soft-deleted record
        var existing = await context.CollectionArticles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ca => ca.CollectionId == collectionId && ca.ArticleId == articleId);

        if (existing != null)
        {
            if (!existing.IsDeleted)
                return; // Article is already active in the collection

            // Restore the soft-deleted record
            existing.IsDeleted = false;
            existing.CreatedAt = DateTime.UtcNow; // Update timestamp if needed
        }
        else
        {
            // Create a new record
            await context.CollectionArticles.AddAsync(
                new CollectionArticle
                {
                    CollectionId = collectionId,
                    ArticleId = articleId,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }

        await context.SaveChangesAsync();
    }

    public async Task SoftDeleteArticleFromCollectionAsync(int collectionId, int articleId)
    {
        var entry = await context.CollectionArticles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ca => ca.CollectionId == collectionId && ca.ArticleId == articleId);
        if (entry != null)
        {
            entry.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task<(List<Collection> Items, int TotalCount)> GetCollectionsContainingArticleAsync(int articleId, int userId, int page, int limit)
    {
        var query = context.Collections
            .Where(c => c.UserId == userId && !c.IsDeleted && c.Articles.Any(a => a.Id == articleId))
            .Include(c => c.Articles);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<Article> Items, int TotalCount)> GetArticlesInCollectionAsync(int collectionId, int page, int limit)
    {
        var query = context.CollectionArticles
            .Where(ca => ca.CollectionId == collectionId && !ca.IsDeleted)
            .Include(ca => ca.Article)
            .ThenInclude(a => a!.Category);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(ca => ca.Article!.PublicationDate)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(ca => ca.Article!)
            .ToListAsync();

        return (items, totalCount);
    }
}