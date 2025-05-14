using API.AppResponse;
using API.DTOs;

public interface ICategoryManagementService
{
    Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId);
    Task<AppResponse<CategoryDto>> GetCategoryById(int id);
    Task<AppResponse<CategoryDto>> GetCategoriesByRestaurantId(int restaurantId);
    Task<AppResponse<CategoryDto>> CreateCategory(CreateCategoryDto categoryDto);
    Task<AppResponse<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto categoryDto);
    Task<AppResponse<bool>> DeleteCategory(int id);
}

