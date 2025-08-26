using NewsCollection.Domain.Entities;

namespace NewsCollection.Application.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<bool> CategoryExistsAsync(string name, int? excludeId = null);
    Task<bool> HasArticlesAsync(int id);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(int id);
}