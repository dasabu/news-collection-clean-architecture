using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using NewsCollection.Application.Dtos;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Application.Mapping;

namespace NewsCollection.Application.Services;

public class CategoryService(ICategoryRepository repository, IHttpContextAccessor httpContext) : ICategoryService
{
    private int GetUserId() => int.Parse(httpContext.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new InvalidOperationException("User ID not found in token."));

    public async Task<List<CategoryDto>> GetAllCategoriesAsync() =>
        (await repository.GetAllCategoriesAsync()).Select(c => c.ToDto()).ToList();

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await repository.GetCategoryByIdAsync(id);
        return category?.ToDto();
    }

    public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto request)
    {
        if (await repository.CategoryExistsAsync(request.Name))
            return null; // Category name already exists

        var category = new Category
        {
            Name = request.Name
        };
        await repository.AddCategoryAsync(category);
        return category.ToDto();
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto request)
    {
        var category = await repository.GetCategoryByIdAsync(id);
        if (category == null)
            return null; // Category not found

        if (await repository.CategoryExistsAsync(request.Name, id))
            return null; // Another category with the same name exists

        category.Name = request.Name;
        await repository.UpdateCategoryAsync(category);
        return category.ToDto();
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await repository.GetCategoryByIdAsync(id);
        if (category == null)
            return false; // Category not found

        if (await repository.HasArticlesAsync(id))
            return false; // Cannot delete category with articles

        await repository.DeleteCategoryAsync(id);
        return true;
    }
}