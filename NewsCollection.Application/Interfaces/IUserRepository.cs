using System;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetUsersWithSubscriptionsAsync(string subscriptionFrequency);
    Task<List<User>> GetUsersWithActiveSubscriptionsAsync();
    Task<List<User>> GetUsersWithActiveSubscriptionsByFrequencyAsync(string frequency);
    Task UpdateUserSubscriptionsAsync(User user);
    Task<List<User>> GetUsersWithCollectionsAsync();
}
