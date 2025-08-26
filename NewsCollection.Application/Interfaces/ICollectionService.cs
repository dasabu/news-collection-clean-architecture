using NewsCollection.Application.Dtos;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionService
{
    Task<List<CollectionDto>> GetAllCollectionsAsync();
    Task<CollectionDto?> GetCollectionByIdAsync(int id);
    Task<CollectionDto?> CreateCollectionAsync(CreateCollectionDto request);
    Task<CollectionDto?> UpdateCollectionAsync(int id, UpdateCollectionDto request);
    Task<bool> DeleteCollectionAsync(int id);
}