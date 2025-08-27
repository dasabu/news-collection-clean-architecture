using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Services;

public class SubscriptionService(ISubscriptionRepository repository, IHttpContextAccessor httpContext) : ISubscriptionService
{

    private int GetUserId() => int.Parse(httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new InvalidOperationException("User ID not found in token."));

    public async Task<SubscriptionDto?> SubscribeAsync(CreateSubscriptionDto request)
    {
        if (request.CategoryId <= 0 || !await repository.CategoryExistsAsync(request.CategoryId))
            return null;

        int userId = GetUserId();
        var existing = await repository.GetSubscriptionAsync(userId, request.CategoryId);
        if (existing != null)
            return new SubscriptionDto(
                existing.CategoryId,
                existing.Category.Name,
                existing.IsActive,
                existing.LastNotified
            );

        var subscription = new UserSubscription
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            IsActive = true
        };

        await repository.AddSubscriptionAsync(subscription);

        var userSubscription = await repository.GetSubscriptionAsync(userId, request.CategoryId);
        var categoryName = userSubscription is null ? string.Empty : userSubscription.Category.Name;

        return new SubscriptionDto(
            subscription.CategoryId,
            categoryName,
            subscription.IsActive,
            subscription.LastNotified
        );
    }

    public async Task<bool> UnsubscribeAsync(int categoryId)
    {
        if (categoryId <= 0 || !await repository.CategoryExistsAsync(categoryId))
            return false;

        await repository.DeleteSubscriptionAsync(GetUserId(), categoryId);
        return true;
    }

    public async Task<List<SubscriptionDto>> GetUserSubscriptionsAsync()
    {
        var subscriptions = await repository.GetUserSubscriptionsAsync(GetUserId());
        return subscriptions.Select(s => new SubscriptionDto
        (
            s.CategoryId,
            s.Category.Name,
            s.IsActive,
            s.LastNotified
        )).ToList();
    }

    public async Task<bool> UpdateUserFrequencyAsync(UpdateUserFrequencyDto request)
    {
        if (request.Frequency != "daily" && request.Frequency != "weekly")
            return false;

        var user = await repository.GetUserAsync(GetUserId());
        if (user == null)
            return false;

        user.SubscriptionFrequency = request.Frequency;
        await repository.UpdateUserAsync(user);
        return true;
    }
}