using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Application.Mapping;

namespace NewsCollection.Application.Services;

public class CollectionService(ICollectionRepository repository, IHttpContextAccessor httpContext) : ICollectionService
{
    //! Collection only services
    private int GetUserId() => int.Parse(httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User ID not found in token."));

    public async Task<List<CollectionDto>> GetAllCollectionsAsync(int page, int limit)
    {
        // return (await repository.GetCollectionsByUserIdAsync(GetUserId())).Select(c => c.ToDto()).ToList();
        if (page < 1 || limit < 1 || limit > 100)
            return [];

        return (await repository.GetCollectionsByUserIdAsync(GetUserId(), page, limit))
            .Select(c => c.ToDto())
            .ToList();
    }

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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false // just ensure that new collection is not marked as deleted
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
        collection.UpdatedAt = DateTime.UtcNow;
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

    //! Article-Collection Services
    public async Task<bool> AddArticleToCollectionAsync(int collectionId, int articleId)
    {
        var collection = await repository.GetCollectionByIdAsync(collectionId);
        if (collection == null || collection.UserId != GetUserId())
            return false;

        if (await repository.ArticleExistsInCollectionAsync(collectionId, articleId))
            return false; // Prevent duplicate

        await repository.AddArticleToCollectionAsync(collectionId, articleId);
        return true;
    }

    public async Task<bool> SoftDeleteArticleFromCollectionAsync(int collectionId, int articleId)
    {
        var collection = await repository.GetCollectionByIdAsync(collectionId);
        if (collection == null || collection.UserId != GetUserId())
            return false;

        await repository.SoftDeleteArticleFromCollectionAsync(collectionId, articleId);
        return true;
    }

    public async Task<List<CollectionDto>> GetCollectionsContainingArticleAsync(int articleId, int page, int limit)
    {
        // return (await repository.GetCollectionsContainingArticleAsync(articleId, GetUserId())).Select(c => c.ToDto()).ToList();
        if (page < 1 || limit < 1 || limit > 100)
            return [];

        return (await repository.GetCollectionsContainingArticleAsync(articleId, GetUserId(), page, limit))
            .Select(c => c.ToDto())
            .ToList();
    }


    public async Task<List<ArticleDto>> GetArticlesInCollectionAsync(int collectionId, int page, int limit)
    {
        // var collection = await repository.GetCollectionByIdAsync(collectionId);
        // if (collection == null || collection.UserId != GetUserId())
        //     return [];

        // return (await repository.GetArticlesInCollectionAsync(collectionId)).Select(a => a.ToDto()).ToList();
        if (page < 1 || limit < 1 || limit > 100)
            return [];

        var collection = await repository.GetCollectionByIdAsync(collectionId);
        if (collection == null || collection.UserId != GetUserId())
            return [];

        return (await repository.GetArticlesInCollectionAsync(collectionId, page, limit))
            .Select(a => a.ToDto())
            .ToList();
    }
}