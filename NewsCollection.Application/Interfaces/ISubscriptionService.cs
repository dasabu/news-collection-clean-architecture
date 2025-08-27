using System;
using NewsCollection.Application.Dtos;

namespace NewsCollection.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDto?> SubscribeAsync(CreateSubscriptionDto request);
    Task<bool> UnsubscribeAsync(int categoryId);
    Task<List<SubscriptionDto>> GetUserSubscriptionsAsync();
    Task<bool> UpdateUserFrequencyAsync(UpdateUserFrequencyDto request);
}
