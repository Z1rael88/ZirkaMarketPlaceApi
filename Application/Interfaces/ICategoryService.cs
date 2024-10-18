using Application.Dtos;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponceDto> CreateCategoryAsync(CategoryDto categoryDto);
    Task<CategoryResponceDto> UpdateCategoryAsync(CategoryDto categoryDto, Guid categoryId);
    Task<CategoryResponceDto> GetCategoryByIdAsync(Guid categoryId);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task DeleteCategoriesAsync(Guid categoryId);
}
