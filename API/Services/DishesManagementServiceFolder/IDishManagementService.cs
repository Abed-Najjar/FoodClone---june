using API.AppResponse;
using API.DTOs;

public interface IDishManagementService
{
    Task<AppResponse<List<DishDto>>> GetDishes(int restaurantId);
    Task<AppResponse<DishDto>> GetDishById(int id);
    Task<AppResponse<List<DishDto>>> GetDishesByCategory(int categoryId);
    Task<AppResponse<DishDto>> CreateDish(CreateDishDto dishDto);
    Task<AppResponse<DishDto>> UpdateDish(int id, UpdateDishDto dishDto);
    Task<AppResponse<bool>> DeleteDish(int id);
}
