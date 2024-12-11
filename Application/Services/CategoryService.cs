using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Application.Validators;
using Domain.Models;
using FluentValidation;
using Infrastructure.Interfaces;
using Mapster;

namespace Application.Services;

public class CategoryService(ICategoryRepository categoryRepository,IFileStorageService fileStorageService, IValidator<CategoryDto> categoryValidator) : ICategoryService
{
    public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        categoryValidator.ValidateAndThrow(categoryDto);
        var  photoUrlBase64 = await ConverterFromIFormFileToString.ConvertIFormFileToBase64Async(categoryDto.PhotoUrl);
        var category = categoryDto.Adapt<Category>();
        category.PhotoUrl = photoUrlBase64;
        var createdCategory = await categoryRepository.CreateCategoryAsync(category);
        createdCategory.PhotoUrl = await fileStorageService.UploadPhotoAsync(categoryDto.PhotoUrl, "category-photos");
        return createdCategory.Adapt<CategoryResponseDto>();
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(CategoryDto categoryDto, Guid categoryId)
    {
        categoryValidator.ValidateAndThrow(categoryDto);
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
