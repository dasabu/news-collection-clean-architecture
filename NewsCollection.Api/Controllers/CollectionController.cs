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
}