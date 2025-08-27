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

    public async Task<PaginatedResult<CollectionDto>> GetAllCollectionsAsync(int page, int limit)
    {
        if (page < 1 || limit < 1 || limit > 100)
            return new PaginatedResult<CollectionDto> { CurrentPage = page, PageSize = limit };

        var (items, totalCount) = await repository.GetCollectionsByUserIdAsync(GetUserId(), page, limit);

        return new PaginatedResult<CollectionDto>
        {
            Items = items.Select(c => c.ToDto()).ToList(),
            CurrentPage = page,
            PageSize = limit,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)limit)
        };
    }

    public async Task<CollectionDto?> GetCollectionByIdAsync(int id)
    {
        var collection = await repository.GetCollectionByIdAsync(id);
        return collection?.UserId == GetUserId() ? collection.ToDto() : null;
    }

    public async Task<CollectionDto?> CreateCollectionAsync(CreateCollectionDto request)
    {
        if (await repository.CollectionExistsAsync(request.Name, GetUserId()))
            return null;

        var collection = new Collection
        {
            Name = request.Name,
            Description = request.Description,
            UserId = GetUserId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await repository.AddCollectionAsync(collection);
        return collection.ToDto();
    }

    public async Task<CollectionDto?> UpdateCollectionAsync(int id, UpdateCollectionDto request)
    {
        var collection = await repository.GetCollectionByIdAsync(id);
        if (collection == null || collection.UserId != GetUserId())
            return null;

        if (await repository.CollectionExistsAsync(request.Name, GetUserId(), id))
            return null;

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
            return false;

        if (await repository.HasArticlesAsync(id))
            return false;

        await repository.DeleteCollectionAsync(id);
        return true;
    }

    public async Task<bool> AddArticleToCollectionAsync(int collectionId, int articleId)
    {
        var collection = await repository.GetCollectionByIdAsync(collectionId);
        if (collection == null || collection.UserId != GetUserId())
            return false;

        if (await repository.ArticleExistsInCollectionAsync(collectionId, articleId))
            return false;

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

    public async Task<PaginatedResult<CollectionDto>> GetCollectionsContainingArticleAsync(int articleId, int page, int limit)
    {
        if (page < 1 || limit < 1 || limit > 100)
            return new PaginatedResult<CollectionDto> { CurrentPage = page, PageSize = limit };

        var (items, totalCount) = await repository.GetCollectionsContainingArticleAsync(articleId, GetUserId(), page, limit);

        return new PaginatedResult<CollectionDto>
        {
            Items = items.Select(c => c.ToDto()).ToList(),
            CurrentPage = page,
            PageSize = limit,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)limit)
        };
    }

    public async Task<PaginatedResult<ArticleDto>> GetArticlesInCollectionAsync(int collectionId, int page, int limit)
    {
        if (page < 1 || limit < 1 || limit > 100)
            return new PaginatedResult<ArticleDto> { CurrentPage = page, PageSize = limit };

        var collection = await repository.GetCollectionByIdAsync(collectionId);
        if (collection == null || collection.UserId != GetUserId())
            return new PaginatedResult<ArticleDto> { CurrentPage = page, PageSize = limit };

        var (items, totalCount) = await repository.GetArticlesInCollectionAsync(collectionId, page, limit);

        return new PaginatedResult<ArticleDto>
        {
            Items = items.Select(a => a.ToDto()).ToList(),
            CurrentPage = page,
            PageSize = limit,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)limit)
        };
    }
}