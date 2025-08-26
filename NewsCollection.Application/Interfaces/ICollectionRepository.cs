using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionRepository
{
    //! only Collection API
    Task<List<Collection>> GetCollectionsByUserIdAsync(int userId);
    Task<Collection?> GetCollectionByIdAsync(int id);
    Task<bool> CollectionExistsAsync(string name, int userId, int? excludeId = null);
    Task<bool> HasArticlesAsync(int id);
    Task AddCollectionAsync(Collection collection);
    Task UpdateCollectionAsync(Collection collection);
    Task DeleteCollectionAsync(int id);

    //! Article-Collection API
    Task<bool> ArticleExistsInCollectionAsync(int collectionId, int articleId);
    Task AddArticleToCollectionAsync(int collectionId, int articleId);
    Task SoftDeleteArticleFromCollectionAsync(int collectionId, int articleId);
    Task<List<Collection>> GetCollectionsContainingArticleAsync(int articleId, int userId); // Get collections containing a specific article
    Task<List<Article>> GetArticlesInCollectionAsync(int collectionId); // Get articles in a specific collection
}