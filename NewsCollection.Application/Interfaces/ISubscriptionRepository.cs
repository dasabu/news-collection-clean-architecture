using System;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<UserSubscription?> GetSubscriptionAsync(int userId, int categoryId);
    Task AddSubscriptionAsync(UserSubscription subscription);
    Task DeleteSubscriptionAsync(int userId, int categoryId);
    Task<List<UserSubscription>> GetUserSubscriptionsAsync(int userId);
    Task<User?> GetUserAsync(int userId);
    Task UpdateUserAsync(User user);
    Task<bool> CategoryExistsAsync(int categoryId);
}
