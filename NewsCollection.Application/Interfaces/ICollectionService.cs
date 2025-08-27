using NewsCollection.Application.Dtos;

namespace NewsCollection.Application.Interfaces;

public interface ICollectionService
{
    //! Collection
    Task<PaginatedResult<CollectionDto>> GetAllCollectionsAsync(int page, int limit);
    Task<CollectionDto?> GetCollectionByIdAsync(int id);
    Task<CollectionDto?> CreateCollectionAsync(CreateCollectionDto request);
    Task<CollectionDto?> UpdateCollectionAsync(int id, UpdateCollectionDto request);
    Task<bool> DeleteCollectionAsync(int id);
    //! Article-Collection
    Task<bool> AddArticleToCollectionAsync(int collectionId, int articleId);
    Task<bool> SoftDeleteArticleFromCollectionAsync(int collectionId, int articleId);
    Task<PaginatedResult<CollectionDto>> GetCollectionsContainingArticleAsync(int articleId, int page, int limit);
    Task<PaginatedResult<ArticleDto>> GetArticlesInCollectionAsync(int collectionId, int page, int limit);
}