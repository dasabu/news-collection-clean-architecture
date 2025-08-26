using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionRepository
{
    Task<List<Collection>> GetCollectionsByUserIdAsync(int userId);
    Task<Collection?> GetCollectionByIdAsync(int id);
    Task AddCollectionAsync(Collection collection);
    Task UpdateCollectionAsync(Collection collection);
    Task DeleteCollectionAsync(int id);
    Task<bool> CollectionExistsForUserAsync(int collectionId, int userId);
    Task<bool> ArticleExistsInCollectionAsync(int collectionId, int articleId);
    Task AddArticleToCollectionAsync(int collectionId, int articleId);
    Task RemoveArticleFromCollectionAsync(int collectionId, int articleId);
    Task<List<Collection>> GetCollectionsContainingArticleAsync(int articleId, int userId);
}
