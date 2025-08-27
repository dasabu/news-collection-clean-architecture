using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;
using System.Threading.Tasks;

namespace NewsCollection.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly NewsCollectionContext _context;

    public SubscriptionRepository(NewsCollectionContext context)
    {
        _context = context;
    }

    public async Task<UserSubscription?> GetSubscriptionAsync(int userId, int categoryId) =>
        await _context.UserSubscriptions
            .Include(us => us.Category)
            .FirstOrDefaultAsync(us => us.UserId == userId && us.CategoryId == categoryId);

    public async Task AddSubscriptionAsync(UserSubscription subscription)
    {
        await _context.UserSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSubscriptionAsync(int userId, int categoryId)
    {
        var subscription = await GetSubscriptionAsync(userId, categoryId);
        if (subscription != null)
        {
            _context.UserSubscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<UserSubscription>> GetUserSubscriptionsAsync(int userId) =>
        await _context.UserSubscriptions
            .Include(us => us.Category)
            .Where(us => us.UserId == userId)
            .ToListAsync();

    public async Task<User?> GetUserAsync(int userId) =>
        await _context.Users.FindAsync(userId);

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CategoryExistsAsync(int categoryId) =>
        await _context.Categories.AnyAsync(c => c.Id == categoryId);
}