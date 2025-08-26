using System;
using NewsCollection.Application.Dtos;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionService
{
    Task<List<CollectionDto>> GetCollectionsAsync();
    Task<CollectionDto?> CreateCollectionAsync(CreateCollectionDto request);
    Task<CollectionDto?> UpdateCollectionAsync(int id, CreateCollectionDto request);
    Task<bool> DeleteCollectionAsync(int id);
    Task<bool> AddArticleToCollectionAsync(int collectionId, int articleId);
    Task<bool> RemoveArticleFromCollectionAsync(int collectionId, int articleId);
    Task<List<CollectionDto>> GetCollectionsContainingArticleAsync(int articleId);
}
