using NewsCollection.Application.Dtos;

namespace NewsCollection.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto?> CreateCategoryAsync(CreateCategoryDto request);
    Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto request);
    Task<bool> DeleteCategoryAsync(int id);
}