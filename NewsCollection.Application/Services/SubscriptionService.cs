using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Services;

public class SubscriptionService(
    ISubscriptionRepository repository, IHttpContextAccessor httpContext,
    ICategoryRepository categoryRepository
) : ISubscriptionService
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
                existing.LastNotified,
                existing.Frequency
            );

        var subscription = new UserSubscription
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            IsActive = true,
            Frequency = request.Frequency ?? "daily" // Default to daily if not specified
        };

        await repository.AddSubscriptionAsync(subscription);

        var userSubscription = await repository.GetSubscriptionAsync(userId, request.CategoryId);
        var categoryName = userSubscription is null ? string.Empty : userSubscription.Category.Name;

        return new SubscriptionDto(
            subscription.CategoryId,
            categoryName,
            subscription.IsActive,
            subscription.LastNotified,
            subscription.Frequency
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
            s.LastNotified,
            s.Frequency
        )).ToList();
    }

    public async Task<bool> UpdateUserFrequencyAsync(UpdateUserFrequencyDto request)
    {
        if (request.Frequency != "daily" && request.Frequency != "weekly")
            return false;

        int userId = GetUserId();
        
        // If categoryId is provided, update just that subscription
        if (request.CategoryId.HasValue)
        {
            var subscription = await repository.GetSubscriptionAsync(userId, request.CategoryId.Value);
            if (subscription == null)
                return false;
                
            subscription.Frequency = request.Frequency;
            await repository.UpdateSubscriptionAsync(subscription);
        }
        // Otherwise update all active subscriptions
        else
        {
            var subscriptions = await repository.GetUserSubscriptionsAsync(userId);
            foreach (var subscription in subscriptions.Where(s => s.IsActive))
            {
                subscription.Frequency = request.Frequency;
            }
            await repository.UpdateSubscriptionsAsync(subscriptions);
        }
        
        return true;
    }

    public async Task<List<SubscriptionDto>> BatchSubscribeAsync(List<SubscriptionRequestDto> requests)
    {
        int userId = GetUserId();
        var results = new List<SubscriptionDto>();
        
        foreach (var request in requests)
        {
            var subscription = await repository.GetSubscriptionAsync(userId, request.CategoryId)
                ?? new UserSubscription { UserId = userId, CategoryId = request.CategoryId };
                
            subscription.IsActive = request.IsActive;
            subscription.Frequency = request.Frequency;
            
            if (await repository.GetSubscriptionAsync(userId, request.CategoryId) == null)
                await repository.AddSubscriptionAsync(subscription);
            else
                await repository.UpdateSubscriptionAsync(subscription);
                
            var category = await categoryRepository.GetCategoryByIdAsync(request.CategoryId);
            results.Add(new SubscriptionDto(
                subscription.CategoryId,
                category?.Name ?? string.Empty,
                subscription.IsActive,
                subscription.LastNotified,
                subscription.Frequency
            ));
        }
        
        return results;
    }

}