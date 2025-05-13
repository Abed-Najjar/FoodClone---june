using API.AppResponse;
using API.DTOs;

public interface IRestaurantMenuService
{
    Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId);
    Task<AppResponse<CategoryDto>> GetCategoryById(int id);
    Task<AppResponse<CategoryDto>> GetCategoryByRestaurantId(int restaurantId);
    Task<AppResponse<CategoryDto>> CreateCategory(CreateCategoryDto categoryDto);
    Task<AppResponse<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto categoryDto);
    Task<AppResponse<bool>> DeleteCategory(int id);

    Task<AppResponse<List<DishDto>>> GetDishes(int restaurantId);
    Task<AppResponse<DishDto>> GetDishById(int id);
    Task<AppResponse<List<DishDto>>> GetDishesByCategory(int categoryId);
    Task<AppResponse<DishDto>> CreateDish(CreateDishDto dishDto);
    Task<AppResponse<DishDto>> UpdateDish(int id, UpdateDishDto dishDto);
    Task<AppResponse<bool>> DeleteDish(int id);
    
}
