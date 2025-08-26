using Microsoft.EntityFrameworkCore;
using NewsCollection.Application.Interfaces;
using NewsCollection.Domain.Entities;
using NewsCollection.Infrastructure.Data;

namespace NewsCollection.Infrastructure.Repositories;

public class CategoryRepository(NewsCollectionContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetAllCategoriesAsync() =>
        await context.Categories.ToListAsync();

    public async Task<Category?> GetCategoryByIdAsync(int id) =>
        await context.Categories.FindAsync(id);

    public async Task<bool> CategoryExistsAsync(string name, int? excludeId = null) =>
        await context.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower() && (excludeId == null || c.Id != excludeId));

    public async Task<bool> HasArticlesAsync(int id) =>
        await context.Articles.AnyAsync(a => a.CategoryId == id);

    public async Task AddCategoryAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category != null)
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
    }
}