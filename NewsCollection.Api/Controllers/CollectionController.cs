using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;

namespace NewsCollection.Api.Controllers;

[Route("api/collections")]
[ApiController]
public class CollectionController(ICollectionService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CollectionDto>>> GetCollections() =>
        Ok(await service.GetCollectionsAsync());

    [HttpPost]
    public async Task<ActionResult<CollectionDto>> CreateCollection(CreateCollectionDto request)
    {
        var collection = await service.CreateCollectionAsync(request);
        return CreatedAtAction(nameof(GetCollections), new { id = collection?.Id }, collection);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CollectionDto>> UpdateCollection(int id, CreateCollectionDto request)
    {
        var collection = await service.UpdateCollectionAsync(id, request);
        return collection == null ? NotFound() : Ok(collection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCollection(int id)
    {
        var success = await service.DeleteCollectionAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpPost("{collectionId}/articles")]
    public async Task<IActionResult> AddArticleToCollection(int collectionId, AddArticleToCollectionDto request)
    {
        var success = await service.AddArticleToCollectionAsync(collectionId, request.ArticleId);
        return success ? CreatedAtAction(nameof(GetCollections), new { id = collectionId }, null) : BadRequest("Article already exists or collection not found.");
    }

    [HttpDelete("{collectionId}/articles/{articleId}")]
    public async Task<IActionResult> RemoveArticleFromCollection(int collectionId, int articleId)
    {
        var success = await service.RemoveArticleFromCollectionAsync(collectionId, articleId);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("articles/{articleId}")]
    public async Task<ActionResult<List<CollectionDto>>> GetCollectionsContainingArticle(int articleId) =>
        Ok(await service.GetCollectionsContainingArticleAsync(articleId));
}
