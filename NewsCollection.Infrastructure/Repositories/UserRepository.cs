using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class UserRepository(NewsCollectionContext context) : IUserRepository
{
    public async Task<List<User>> GetUsersWithSubscriptionsAsync(string subscriptionFrequency)
    {
        return await context.Users
            .Include(u => u.Subscriptions.Where(s => s.Frequency == subscriptionFrequency))
            .ThenInclude(s => s.Category)
            .Where(u => u.Subscriptions.Any(s => s.Frequency == subscriptionFrequency))
            .ToListAsync();
    }

    public async Task<List<User>> GetUsersWithActiveSubscriptionsAsync()
    {
        return await context.Users
            .Include(u => u.Subscriptions.Where(s => s.IsActive))
            .ThenInclude(s => s.Category)
            .Where(u => u.Subscriptions.Any(s => s.IsActive))
            .ToListAsync();
    }

    public async Task<List<User>> GetUsersWithActiveSubscriptionsByFrequencyAsync(string frequency)
    {
        return await context.Users
            .Include(u => u.Subscriptions.Where(s => s.IsActive && s.Frequency == frequency))
            .ThenInclude(s => s.Category)
            .Where(u => u.Subscriptions.Any(s => s.IsActive && s.Frequency == frequency))
            .ToListAsync();
    }

    public async Task UpdateUserSubscriptionsAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<List<User>> GetUsersWithCollectionsAsync()
    {
        // First get users with non-deleted collections
        var users = await context.Users
            .Include(u => u.Collections.Where(c => !c.IsDeleted))
            .Where(u => u.Collections.Any(c => !c.IsDeleted))
            .ToListAsync();
            
        // Then load the articles for each collection using the join table
        foreach (var user in users)
        {
            foreach (var collection in user.Collections)
            {
                // Load the CollectionArticle join entities with their Article entities
                var collectionArticles = await context.Set<CollectionArticle>()
                    .Include(ca => ca.Article)
                    .Where(ca => ca.CollectionId == collection.Id && !ca.IsDeleted)
                    .ToListAsync();
                    
                // Add the articles to the collection
                collection.Articles = collectionArticles
                    .Where(ca => ca.Article != null)
                    .Select(ca => ca.Article!)
                    .ToList();
            }
        }
        
        return users;
    }
}