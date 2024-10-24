using Application.Dtos;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Interfaces;
using Mapster;

namespace Application.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = categoryDto.Adapt<Category>();
        var createdCategory = await categoryRepository.CreateCategoryAsync(category);
        return createdCategory.Adapt<CategoryResponseDto>();
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(CategoryDto categoryDto, Guid categoryId)
    {
        var existingCategory = await categoryRepository.GetCategoryByIdAsync(categoryId);
        if (existingCategory == null)
            throw new ArgumentException($"Category with id: {categoryId} not found");

        categoryDto.Adapt(existingCategory); 
        var updatedCategory = await categoryRepository.UpdateCategoryAsync(existingCategory);
        return updatedCategory.Adapt<CategoryResponseDto>();
    }

    public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid categoryId)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
        return category.Adapt<CategoryResponseDto>();
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync() 
    {
        var categories = await categoryRepository.GetCategoriesAsync();
        return categories.Adapt<IEnumerable<CategoryResponseDto>>();
    }

    public async Task DeleteCategoriesAsync(Guid categoryId) 
    {
        await categoryRepository.DeleteCategoryAsync(categoryId);
    }
}
