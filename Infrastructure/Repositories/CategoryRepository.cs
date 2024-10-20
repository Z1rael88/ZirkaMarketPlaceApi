using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(IApplicationDbContext dbContext) : ICategoryRepository
    {       

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var createdCategory = await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return createdCategory.Entity;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var categoryToUpdate = await GetCategoryByIdAsync(category.Id);
            if (categoryToUpdate == null)
                throw new ArgumentException($"Category with id: {category.Id} not found");

            dbContext.Entry(categoryToUpdate).CurrentValues.SetValues(category);
            await dbContext.SaveChangesAsync();
            return categoryToUpdate;
        }
        

        public async Task<Category> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
                throw new ArgumentException($"Category with id: {categoryId} not found");
            return category;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await dbContext.Categories.ToListAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var categoryToDelete = await GetCategoryByIdAsync(categoryId);
            if (categoryToDelete == null)
                throw new ArgumentException($"Category with id: {categoryId} not found");

            dbContext.Categories.Remove(categoryToDelete);
            await dbContext.SaveChangesAsync();

        }

        
    }
}