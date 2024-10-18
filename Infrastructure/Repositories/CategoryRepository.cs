using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public CategoryRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }       

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var createdCategory = await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return createdCategory.Entity;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var categoryToUpdate = await GetCategoryByIdAsync(category.Id);
            if (categoryToUpdate == null)
                throw new ArgumentException($"Category with id: {category.Id} not found");

            _dbContext.Entry(categoryToUpdate).CurrentValues.SetValues(category);
            await _dbContext.SaveChangesAsync();
            return categoryToUpdate;
        }
        

        public async Task<Category> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
                throw new ArgumentException($"Category with id: {categoryId} not found");
            return category;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var categoryToDelete = await GetCategoryByIdAsync(categoryId);
            if (categoryToDelete == null)
                throw new ArgumentException($"Category with id: {categoryId} not found");

            _dbContext.Categories.Remove(categoryToDelete);
            await _dbContext.SaveChangesAsync();

        }

        
    }
}