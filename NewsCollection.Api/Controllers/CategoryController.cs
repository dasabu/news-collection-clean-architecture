using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;

namespace NewsCollection.Api.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController(ICategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories() =>
        Ok(await service.GetAllCategoriesAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await service.GetCategoryByIdAsync(id);
        return category == null ? NotFound("Category not found") : Ok(category);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto request)
    {
        var category = await service.CreateCategoryAsync(request);
        return category == null
            ? BadRequest("Category name already exists")
            : CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto request)
    {
        var category = await service.UpdateCategoryAsync(id, request);
        return category == null
            ? BadRequest("Category not found or name already exists")
            : Ok(category);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var success = await service.DeleteCategoryAsync(id);
        return success ? NoContent() : BadRequest("Category not found or contains articles");
    }
}