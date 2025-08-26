using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class CollectionRepository(NewsCollectionContext context) : ICollectionRepository
{
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
}