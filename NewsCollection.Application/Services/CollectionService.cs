using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Application.Mapping;

namespace NewsCollection.Application.Services;

public class CollectionService(ICollectionRepository repository, IHttpContextAccessor httpContext) : ICollectionService
{
    private int GetUserId() => int.Parse(httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new InvalidOperationException("User ID not found in token."));

    public async Task<List<CollectionDto>> GetAllCollectionsAsync() =>
        (await repository.GetCollectionsByUserIdAsync(GetUserId())).Select(c => c.ToDto()).ToList();

    public async Task<CollectionDto?> GetCollectionByIdAsync(int id)
    {
        var collection = await repository.GetCollectionByIdAsync(id);
        return collection?.UserId == GetUserId() ? collection.ToDto() : null;
    }

    public async Task<CollectionDto?> CreateCollectionAsync(CreateCollectionDto request)
    {
        if (await repository.CollectionExistsAsync(request.Name, GetUserId()))
            return null; // Collection name already exists for this user

        var collection = new Collection
        {
            Name = request.Name,
            Description = request.Description,
            UserId = GetUserId(),
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        await repository.AddCollectionAsync(collection);
        return collection.ToDto();
    }

    public async Task<CollectionDto?> UpdateCollectionAsync(int id, UpdateCollectionDto request)
    {
        var collection = await repository.GetCollectionByIdAsync(id);
        if (collection == null || collection.UserId != GetUserId())
            return null; // Collection not found or not owned by user

        if (await repository.CollectionExistsAsync(request.Name, GetUserId(), id))
            return null; // Another collection with the same name exists

        collection.Name = request.Name;
        collection.Description = request.Description;
        collection.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        await repository.UpdateCollectionAsync(collection);
        return collection.ToDto();
    }

    public async Task<bool> DeleteCollectionAsync(int id)
    {
        var collection = await repository.GetCollectionByIdAsync(id);
        if (collection == null || collection.UserId != GetUserId())
            return false; // Collection not found or not owned by user

        if (await repository.HasArticlesAsync(id))
            return false; // Cannot delete collection with articles

        await repository.DeleteCollectionAsync(id);
        return true;
    }
}