using API.DTOs;
using API.AppResponse;

namespace API.Services.CmsServiceFolder
{
    public interface ICmsService
    {
        // Category methods
        Task<AppResponse<List<CategoriesDto>>> GetAllCategoriesAsync();
        Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId);
        Task<AppResponse<CategoryDto>> UpdateCategoryAsync(int id, CategoryDto categoryDto);
        Task<AppResponse<bool>> DeleteCategoryAsync(int id);
        
        // Restaurant methods
    Task<AppResponse<List<RestaurantDto>>> GetAllRestaurantsAsync();
        Task<AppResponse<AdminRestaurantDto>> GetRestaurantByIdAsync(int id);
        Task<AppResponse<AdminRestaurantDto>> CreateRestaurantAsync(AdminRestaurantCreateDto restaurantDto);
        Task<AppResponse<AdminRestaurantDto>> UpdateRestaurantAsync(int id, AdminRestaurantDto restaurantDto);
        Task<AppResponse<bool>> DeleteRestaurantAsync(int id);

        // Dish methods
        Task<AppResponse<AdminRestaurantDishDto>> CreateDishAsync(CreateDishDto dishDto);
        Task<AppResponse<List<AdminRestaurantDishDto>>> GetAllDishesAsync();
        Task<AppResponse<AdminRestaurantDishDto>> UpdateDishAsync(int id, AdminRestaurantDishDto dishDto);
        Task<AppResponse<bool>> DeleteDishAsync(int id);
        
        // User methods
        Task<AppResponse<List<UserDto>>> GetAllUsersAsync();
        Task<AppResponse<bool>> DeleteUserAsync(int id);
        
        // Order methods
        Task<AppResponse<List<OrderDto>>> GetAllOrdersAsync();
        Task<AppResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<AppResponse<bool>> UpdateOrderStatusAsync(int id, string status);
        Task<AppResponse<List<OrderDto>>> GetOrdersByUserAsync(int userId);
        Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId);
        Task<AppResponse<List<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId);
    }
}
