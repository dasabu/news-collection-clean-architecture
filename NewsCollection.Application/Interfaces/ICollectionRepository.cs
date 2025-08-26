using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionRepository
{
    Task<List<Collection>> GetCollectionsByUserIdAsync(int userId);
    Task<Collection?> GetCollectionByIdAsync(int id);
    Task<bool> CollectionExistsAsync(string name, int userId, int? excludeId = null);
    Task<bool> HasArticlesAsync(int id);
    Task AddCollectionAsync(Collection collection);
    Task UpdateCollectionAsync(Collection collection);
    Task DeleteCollectionAsync(int id);
}