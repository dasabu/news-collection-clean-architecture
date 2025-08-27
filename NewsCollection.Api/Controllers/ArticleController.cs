using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;

namespace NewsCollection.Api.Controllers;

[Route("api/articles")]
[ApiController]
public class ArticleController(IArticleService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ArticleDto>>> GetArticles() =>
        Ok(await service.GetAllArticlesAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleDto>> GetArticle(int id)
    {
        var article = await service.GetArticleByIdAsync(id);
        return article == null ? NotFound("Article not found") : Ok(article);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ArticleDto>> CreateArticle(CreateArticleDto request)
    {
        var article = await service.CreateArticleAsync(request);
        return article == null
            ? BadRequest("Article URL already exists")
            : CreatedAtAction(nameof(GetArticle), new { id = article.Id }, article);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ArticleDto>> UpdateArticle(int id, UpdateArticleDto request)
    {
        var article = await service.UpdateArticleAsync(id, request);
        return article == null
            ? BadRequest("Article not found or URL already exists")
            : Ok(article);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var success = await service.DeleteArticleAsync(id);
        return success ? NoContent() : BadRequest("Article not found or is in a collection");
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<PaginatedResult<ArticleDto>>> GetArticlesByCategory(
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string sortOrder = "desc"
    )
    {
        var paginated = await service.GetArticlesByCategoryAsync(categoryId, page, limit, sortOrder);
        return paginated.TotalPages > 0 || page == 1 ? Ok(paginated) : BadRequest("Invalid page, limit, category ID, or sort order");
    }
}