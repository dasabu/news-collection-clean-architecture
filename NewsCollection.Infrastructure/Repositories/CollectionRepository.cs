using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class CollectionRepository(NewsCollectionContext context) : ICollectionRepository
{
    //! Collection only
    public async Task<List<Collection>> GetCollectionsByUserIdAsync(int userId)
    {
        var collections = await context.Collections
            .Where(c => c.UserId == userId)
            .Include(c => c.Articles)
            .ToListAsync();
        return collections;
    }


    public async Task<Collection?> GetCollectionByIdAsync(int id) =>
        await context.Collections
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<bool> CollectionExistsAsync(string name, int userId, int? excludeId = null) =>
        await context.Collections
            .AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.UserId == userId && (excludeId == null || c.Id != excludeId));

    public async Task<bool> HasArticlesAsync(int id) =>
        await context.CollectionArticles.AnyAsync(ca => ca.CollectionId == id);

    public async Task AddCollectionAsync(Collection collection)
    {
        context.Collections.Add(collection);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCollectionAsync(Collection collection)
    {
        context.Collections.Update(collection);
        await context.SaveChangesAsync();
    }

    public async Task DeleteCollectionAsync(int id)
    {
        var collection = await context.Collections
            .IgnoreQueryFilters() // ignore global query filter, to find soft-deleted collection
            .FirstOrDefaultAsync(c => c.Id == id);
        if (collection != null)
        {
            collection.IsDeleted = true; // mark as soft-deleted
            await context.SaveChangesAsync();
        }
    }

    //! Article-Collection
    public async Task<bool> ArticleExistsInCollectionAsync(int collectionId, int articleId) =>
        await context.CollectionArticles
            .AnyAsync(ca => ca.CollectionId == collectionId && ca.ArticleId == articleId && !ca.IsDeleted);

    public async Task AddArticleToCollectionAsync(int collectionId, int articleId)
    {
        context.CollectionArticles.Add(new CollectionArticle { CollectionId = collectionId, ArticleId = articleId, IsDeleted = false });
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

    public async Task<List<Collection>> GetCollectionsContainingArticleAsync(int articleId, int userId) =>
        await context.Collections
            .Where(c => c.UserId == userId && c.Articles.Any(a => a.Id == articleId))
            .Include(c => c.Articles)
            .ToListAsync();
    
    public async Task<List<Article>> GetArticlesInCollectionAsync(int collectionId) =>
        await context.CollectionArticles
            .Where(ca => ca.CollectionId == collectionId && !ca.IsDeleted)
            .Include(ca => ca.Article)
            .ThenInclude(a => a!.Category)
            .Select(ca => ca.Article!)
            .ToListAsync();
}