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
            .Where(u => u.SubscriptionFrequency == subscriptionFrequency)
            .Include(u => u.Subscriptions)
            .ThenInclude(s => s.Category)
            .ToListAsync();
    }

    public async Task<List<User>> GetUsersWithActiveSubscriptionsAsync()
    {
        return await context.Users
            .Include(u => u.Subscriptions)
            .ThenInclude(s => s.Category)
            .Where(u => u.Subscriptions.Any(s => s.IsActive))
            .ToListAsync();
    }

    public async Task UpdateUserSubscriptionsAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<List<User>> GetUsersWithCollectionsAsync()
    {
        return await context.Users
            .Include(u => u.Collections)
            .ThenInclude(c => c.Articles)
            // .ThenInclude(ca => ca.Article)
            .Where(u => u.Collections.Any(c => !c.IsDeleted))
            .ToListAsync();
    }
}