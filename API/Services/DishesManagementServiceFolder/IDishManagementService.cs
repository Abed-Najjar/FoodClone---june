using API.AppResponse;
using API.DTOs;

public interface IDishManagementService
{
    Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetDishesInRestaurant(int restaurantId, PaginationDto? paginationDto = null);
    Task<AppResponse<AdminRestaurantDishDto>> GetDishById(int id);
    Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetDishesByCategory(int categoryId, PaginationDto? paginationDto = null);
    Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetAllDishesAsync(PaginationDto? paginationDto = null); // Added method
    Task<AppResponse<AdminRestaurantDishDto>> CreateDish(CreateDishDto dishDto);
    Task<AppResponse<AdminRestaurantDishDto>> UpdateDish(int id, UpdateDishDto dishDto);
    Task<AppResponse<bool>> DeleteDish(int id);
}
