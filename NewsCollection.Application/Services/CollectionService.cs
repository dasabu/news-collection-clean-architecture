using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Application.Mapping;
using NewsCollection.Domain.Entities;

public class CollectionService(ICollectionRepository repository, IHttpContextAccessor httpContext) : ICollectionService
{
    private int GetUserId() => int.Parse(httpContext.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public async Task<List<CollectionDto>> GetCollectionsAsync()
    {
        var collections = await repository.GetCollectionsByUserIdAsync(GetUserId());
        return collections.Select(c => c.ToDto()).ToList();
    }

    public async Task<CollectionDto?> CreateCollectionAsync(CreateCollectionDto request)
    {
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

    public async Task<CollectionDto?> UpdateCollectionAsync(int id, CreateCollectionDto request)
    {
        var collection = await repository.GetCollectionByIdAsync(id);
        if (collection == null || collection.UserId != GetUserId()) return null;
        collection.Name = request.Name;
        collection.Description = request.Description;
        collection.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        await repository.UpdateCollectionAsync(collection);
        return collection.ToDto();
    }

    public async Task<bool> DeleteCollectionAsync(int id)
    {
        var collection = await repository.GetCollectionByIdAsync(id);
        if (collection == null || collection.UserId != GetUserId()) return false;
        await repository.DeleteCollectionAsync(id);
        return true;
    }

    public async Task<bool> AddArticleToCollectionAsync(int collectionId, int articleId)
    {
        if (!await repository.CollectionExistsForUserAsync(collectionId, GetUserId()) ||
            await repository.ArticleExistsInCollectionAsync(collectionId, articleId))
            return false;
        await repository.AddArticleToCollectionAsync(collectionId, articleId);
        return true;
    }

    public async Task<bool> RemoveArticleFromCollectionAsync(int collectionId, int articleId)
    {
        if (!await repository.CollectionExistsForUserAsync(collectionId, GetUserId())) return false;
        await repository.RemoveArticleFromCollectionAsync(collectionId, articleId);
        return true;
    }

    public async Task<List<CollectionDto>> GetCollectionsContainingArticleAsync(int articleId)
    {
        var collections = await repository.GetCollectionsContainingArticleAsync(articleId, GetUserId());
        return collections.Select(c => c.ToDto()).ToList();
    }
}