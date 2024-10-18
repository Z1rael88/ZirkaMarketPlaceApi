using Application.Dtos;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Interfaces;
using Mapster;

namespace Infrastructure.Repositories;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<CategoryResponceDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = categoryDto.Adapt<Category>();
        var createdCategory = await categoryRepository.CreateCategoryAsync(category);
        return createdCategory.Adapt<CategoryResponceDto>();
    }

    public async Task<CategoryResponceDto> UpdateCategoryAsync(CategoryDto categoryDto, Guid categoryId)
    {
        var existingCategory = await categoryRepository.GetCategoryByIdAsync(categoryId);
        if (existingCategory == null)
            throw new ArgumentException($"Category with id: {categoryId} not found");

        categoryDto.Adapt(existingCategory); 
        var updatedCategory = await categoryRepository.UpdateCategoryAsync(existingCategory);
        return updatedCategory.Adapt<CategoryResponceDto>();
    }

    public async Task<CategoryResponceDto> GetCategoryByIdAsync(Guid categoryId)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
        return category.Adapt<CategoryResponceDto>();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync() 
    {
        var categories = await categoryRepository.GetCategoriesAsync();
        return categories.Adapt<IEnumerable<CategoryDto>>();
    }

    public async Task DeleteCategoriesAsync(Guid categoryId) 
    {
        await categoryRepository.DeleteCategoryAsync(categoryId);
    }
}
