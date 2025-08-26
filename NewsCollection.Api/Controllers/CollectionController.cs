using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;

namespace NewsCollection.Api.Controllers;

[Authorize]
[Route("api/collections")]
[ApiController]
public class CollectionController(ICollectionService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CollectionDto>>> GetCollections() =>
        Ok(await service.GetAllCollectionsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<CollectionDto>> GetCollection(int id)
    {
        var collection = await service.GetCollectionByIdAsync(id);
        return collection == null ? NotFound("Collection not found") : Ok(collection);
    }

    [HttpPost]
    public async Task<ActionResult<CollectionDto>> CreateCollection(CreateCollectionDto request)
    {
        var collection = await service.CreateCollectionAsync(request);
        return collection == null
            ? BadRequest("Collection name already exists")
            : CreatedAtAction(nameof(GetCollection), new { id = collection.Id }, collection);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CollectionDto>> UpdateCollection(int id, UpdateCollectionDto request)
    {
        var collection = await service.UpdateCollectionAsync(id, request);
        return collection == null
            ? BadRequest("Collection not found or name already exists")
            : Ok(collection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        var success = await service.DeleteCollectionAsync(id);
        return success ? NoContent() : BadRequest("Collection not found or contains articles");
    }

    [HttpPost("{collectionId}/articles/{articleId}")]
    public async Task<IActionResult> AddArticleToCollection(int collectionId, int articleId)
    {
        var success = await service.AddArticleToCollectionAsync(collectionId, articleId);
        return success ? Ok("Article added to collection") : BadRequest("Article already in collection or collection not found");
    }

    [HttpDelete("{collectionId}/articles/{articleId}")]
    public async Task<IActionResult> RemoveArticleFromCollection(int collectionId, int articleId)
    {
        var success = await service.SoftDeleteArticleFromCollectionAsync(collectionId, articleId);
        return success ? Ok("Article removed from collection") : BadRequest("Article or collection not found");
    }

    [HttpGet("articles/{articleId}")]
    public async Task<ActionResult<List<CollectionDto>>> GetCollectionsContainingArticle(int articleId) =>
        Ok(await service.GetCollectionsContainingArticleAsync(articleId));
    
    [HttpGet("{collectionId}/articles")]
    public async Task<ActionResult<List<ArticleDto>>> GetArticlesInCollection(int collectionId)
    {
        var articles = await service.GetArticlesInCollectionAsync(collectionId);
        return articles.Any() || (await service.GetCollectionByIdAsync(collectionId) != null)
            ? Ok(articles)
            : NotFound("Collection not found");
    }
}