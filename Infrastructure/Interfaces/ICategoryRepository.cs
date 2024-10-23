using Domain.Models;

namespace Infrastructure.Interfaces;

public interface ICategoryRepository
{
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task<Category> GetCategoryByIdAsync(Guid categoryid);
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task DeleteCategoryAsync(Guid categoryid);
}
