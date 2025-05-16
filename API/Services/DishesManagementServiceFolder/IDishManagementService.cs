using API.AppResponse;
using API.DTOs;

public interface IDishManagementService
{
    Task<AppResponse<List<AdminRestaurantDishDto>>> GetDishesInRestaurant(int restaurantId);
    Task<AppResponse<AdminRestaurantDishDto>> GetDishById(int id);
    Task<AppResponse<List<AdminRestaurantDishDto>>> GetDishesByCategory(int categoryId);
    Task<AppResponse<AdminRestaurantDishDto>> CreateDish(CreateDishDto dishDto);
    Task<AppResponse<AdminRestaurantDishDto>> UpdateDish(int id, UpdateDishDto dishDto);
    Task<AppResponse<bool>> DeleteDish(int id);
}
