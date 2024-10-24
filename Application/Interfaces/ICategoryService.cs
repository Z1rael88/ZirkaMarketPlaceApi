using Application.Dtos;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponseDto> CreateCategoryAsync(CategoryDto categoryDto);
    Task<CategoryResponseDto> UpdateCategoryAsync(CategoryDto categoryDto, Guid categoryId);
    Task<CategoryResponseDto> GetCategoryByIdAsync(Guid categoryId);
    Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
    Task DeleteCategoriesAsync(Guid categoryId);
}
