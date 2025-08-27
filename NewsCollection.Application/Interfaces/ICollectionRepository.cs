using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionRepository
{
    Task<(List<Collection> Items, int TotalCount)> GetCollectionsByUserIdAsync(int userId, int page, int limit);
    Task<Collection?> GetCollectionByIdAsync(int id);
    Task<bool> CollectionExistsAsync(string name, int userId, int? excludeId = null);
    Task<bool> HasArticlesAsync(int id);
    Task AddCollectionAsync(Collection collection);
    Task UpdateCollectionAsync(Collection collection);
    Task DeleteCollectionAsync(int id);
    Task<bool> ArticleExistsInCollectionAsync(int collectionId, int articleId);
    Task AddArticleToCollectionAsync(int collectionId, int articleId);
    Task SoftDeleteArticleFromCollectionAsync(int collectionId, int articleId);
    Task<(List<Collection> Items, int TotalCount)> GetCollectionsContainingArticleAsync(int articleId, int userId, int page, int limit);
    Task<(List<Article> Items, int TotalCount)> GetArticlesInCollectionAsync(int collectionId, int page, int limit);
}