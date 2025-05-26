using API.DTOs;
using API.Services.CmsServiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.AppResponse;

namespace API.Controllers
{
    [ApiController]
    [Route("api/cms")]
    [Authorize(Roles = "Admin")]
    public class CmsController : ControllerBase
    {
        private readonly ICmsService _cmsService;
        private readonly ILogger<CmsController> _logger;

        public CmsController(ICmsService cmsService, ILogger<CmsController> logger)
        {
            _cmsService = cmsService;
            _logger = logger;
        }

        [HttpGet("categories")]
        public async Task<AppResponse<List<CategoriesDto>>> GetAllCategories()
        {
            return await _cmsService.GetAllCategoriesAsync();
        }

        [HttpGet("restaurants")]
        public async Task<AppResponse<List<RestaurantDto>>> GetAllRestaurants()
        {
            return await _cmsService.GetAllRestaurantsAsync();
        }

        [HttpGet("dishes")]
        public async Task<AppResponse<List<AdminRestaurantDishDto>>> GetAllDishes()
        {
            return await _cmsService.GetAllDishesAsync();
        }

        [HttpGet("users")]
        public async Task<AppResponse<List<UserDto>>> GetAllUsers()
        {
            return await _cmsService.GetAllUsersAsync();
        }

        [HttpGet("orders")]
        public async Task<AppResponse<List<OrderDto>>> GetAllOrders()
        {
            return await _cmsService.GetAllOrdersAsync();
        }

        [HttpGet("orders/{id}")]
        public async Task<AppResponse<OrderDto>> GetOrderById(int id)
        {
            return await _cmsService.GetOrderByIdAsync(id);
        }

        [HttpGet("orders/user/{userId}")]
        public async Task<AppResponse<List<OrderDto>>> GetOrdersByUser(int userId)
        {
            return await _cmsService.GetOrdersByUserAsync(userId);
        }

        [HttpGet("orders/restaurant/{restaurantId}")]
        public async Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurant(int restaurantId)
        {
            return await _cmsService.GetOrdersByRestaurantAsync(restaurantId);
        }

        [HttpGet("orders/employee/{employeeId}")]
        public async Task<AppResponse<List<OrderDto>>> GetOrdersByEmployee(int employeeId)
        {
            return await _cmsService.GetOrdersByEmployeeAsync(employeeId);
        }

        [HttpPut("categories/{id}")]
        public async Task<AppResponse<CategoryDto>> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<CategoryDto>(null, "Invalid model state", 400, false);
            }

            return await _cmsService.UpdateCategoryAsync(id, categoryDto);
        }

        [HttpDelete("categories/{id}")]
        public async Task<AppResponse<bool>> DeleteCategory(int id)
        {
            return await _cmsService.DeleteCategoryAsync(id);
        }

        [HttpPost("restaurants")]
        public async Task<AppResponse<AdminRestaurantDto>> CreateRestaurant([FromBody] AdminRestaurantCreateDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Invalid model state", 400, false);
            }

            return await _cmsService.CreateRestaurantAsync(restaurantDto);
        }        [HttpPut("restaurants/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> UpdateRestaurant(int id, [FromBody] AdminRestaurantDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Invalid model state", 400, false);
            }

            _logger.LogInformation($"Updating restaurant with ID: {id}, Opening Hours: {restaurantDto.OpeningHours}");
            return await _cmsService.UpdateRestaurantAsync(id, restaurantDto);
        }

        [HttpDelete("restaurants/{id}")]
        public async Task<AppResponse<bool>> DeleteRestaurant(int id)
        {
            return await _cmsService.DeleteRestaurantAsync(id);
        }

        [HttpPost("create-dish")]
        public async Task<AppResponse<AdminRestaurantDishDto>> CreateDish([FromBody] CreateDishDto dishDto)
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Invalid model state", 400, false);
            }

            return await _cmsService.CreateDishAsync(dishDto);
        }

        [HttpPut("dishes/{id}")]
        public async Task<AppResponse<AdminRestaurantDishDto>> UpdateDish(int id, [FromBody] AdminRestaurantDishDto dishDto)
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Invalid model state", 400, false);
            }

            return await _cmsService.UpdateDishAsync(id, dishDto);
        }

        [HttpDelete("dishes/{id}")]
        public async Task<AppResponse<bool>> DeleteDish(int id)
        {
            return await _cmsService.DeleteDishAsync(id);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<AppResponse<bool>> UpdateOrderStatus(int id, [FromBody] string status)
        {
            return await _cmsService.UpdateOrderStatusAsync(id, status);
        }

        [HttpDelete("users/{id}")]
        public async Task<AppResponse<bool>> DeleteUser(int id)
        {
            return await _cmsService.DeleteUserAsync(id);
        }
    }
}
