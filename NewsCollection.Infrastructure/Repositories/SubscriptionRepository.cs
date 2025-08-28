using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;
using System.Threading.Tasks;

namespace NewsCollection.Infrastructure.Repositories;

public class SubscriptionRepository(NewsCollectionContext context) : ISubscriptionRepository
{
    public async Task<UserSubscription?> GetSubscriptionAsync(int userId, int categoryId) =>
        await context.UserSubscriptions
            .Include(us => us.Category)
            .FirstOrDefaultAsync(us => us.UserId == userId && us.CategoryId == categoryId);

    public async Task AddSubscriptionAsync(UserSubscription subscription)
    {
        await context.UserSubscriptions.AddAsync(subscription);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSubscriptionAsync(int userId, int categoryId)
    {
        var subscription = await GetSubscriptionAsync(userId, categoryId);
        if (subscription != null)
        {
            context.UserSubscriptions.Remove(subscription);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<UserSubscription>> GetUserSubscriptionsAsync(int userId) =>
        await context.UserSubscriptions
            .Include(us => us.Category)
            .Where(us => us.UserId == userId)
            .ToListAsync();

    public async Task<User?> GetUserAsync(int userId) =>
        await context.Users.FindAsync(userId);

    public async Task UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<bool> CategoryExistsAsync(int categoryId) =>
        await context.Categories.AnyAsync(c => c.Id == categoryId);
        
    public async Task UpdateSubscriptionAsync(UserSubscription subscription)
    {
        context.UserSubscriptions.Update(subscription);
        await context.SaveChangesAsync();
    }
    
    public async Task UpdateSubscriptionsAsync(List<UserSubscription> subscriptions)
    {
        foreach (var subscription in subscriptions)
        {
            context.UserSubscriptions.Update(subscription);
        }
        await context.SaveChangesAsync();
    }
}