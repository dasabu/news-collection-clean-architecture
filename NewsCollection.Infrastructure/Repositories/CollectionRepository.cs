using System;
using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class CollectionRepository(NewsCollectionContext context) : ICollectionRepository
{
    public async Task<List<Collection>> GetCollectionsByUserIdAsync(int userId) =>
        await context.Collections
            .Where(c => c.UserId == userId)
            .Include(c => c.Articles)
            .ToListAsync();

    public async Task<Collection?> GetCollectionByIdAsync(int id) =>
        await context.Collections
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id);

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
        var collection = await context.Collections.FindAsync(id);
        if (collection != null)
        {
            context.Collections.Remove(collection);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> CollectionExistsForUserAsync(int collectionId, int userId) =>
        await context.Collections.AnyAsync(c => c.Id == collectionId && c.UserId == userId);

    public async Task<bool> ArticleExistsInCollectionAsync(int collectionId, int articleId) =>
        await context.CollectionArticles.AnyAsync(ca => ca.CollectionId == collectionId && ca.ArticleId == articleId);

    public async Task AddArticleToCollectionAsync(int collectionId, int articleId)
    {
        context.CollectionArticles.Add(new CollectionArticle { CollectionId = collectionId, ArticleId = articleId });
        await context.SaveChangesAsync();
    }

    public async Task RemoveArticleFromCollectionAsync(int collectionId, int articleId)
    {
        var entry = await context.CollectionArticles
            .FirstOrDefaultAsync(ca => ca.CollectionId == collectionId && ca.ArticleId == articleId);
        if (entry != null)
        {
            context.CollectionArticles.Remove(entry);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Collection>> GetCollectionsContainingArticleAsync(int articleId, int userId) =>
        await context.Collections
            .Where(c => c.UserId == userId && c.Articles.Any(a => a.Id == articleId))
            .Include(c => c.Articles)
            .ToListAsync();
}
