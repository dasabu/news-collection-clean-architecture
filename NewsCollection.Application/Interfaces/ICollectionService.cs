using NewsCollection.Application.Dtos;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionService
{
    //! Collection only
    // Task<List<CollectionDto>> GetAllCollectionsAsync();
    Task<List<CollectionDto>> GetAllCollectionsAsync(int page, int limit);
    Task<CollectionDto?> GetCollectionByIdAsync(int id);
    Task<CollectionDto?> CreateCollectionAsync(CreateCollectionDto request);
    Task<CollectionDto?> UpdateCollectionAsync(int id, UpdateCollectionDto request);
    Task<bool> DeleteCollectionAsync(int id);

    //! Article-Collection
    Task<bool> AddArticleToCollectionAsync(int collectionId, int articleId);
    Task<bool> SoftDeleteArticleFromCollectionAsync(int collectionId, int articleId);
    // Task<List<CollectionDto>> GetCollectionsContainingArticleAsync(int articleId);
    Task<List<CollectionDto>> GetCollectionsContainingArticleAsync(int articleId, int page, int limit); // get collection containing a specific article
    // Task<List<ArticleDto>> GetArticlesInCollectionAsync(int collectionId); // check what articles are in a specific collection
    Task<List<ArticleDto>> GetArticlesInCollectionAsync(int collectionId, int page, int limit);
}