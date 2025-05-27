using API.DTOs;
using API.Services.OrderServiceFolder; // Added
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
        // Remove ICmsService
        // private readonly ICmsService _cmsService;
        private readonly ILogger<CmsController> _logger;
        private readonly IRestaurantManagement _restaurantManagementService; // Added
        private readonly IDishManagementService _dishManagementService; // Added
        private readonly ICategoryManagementService _categoryManagementService; // Added
        private readonly IUserManagementService _userManagementService; // Added
        private readonly IOrderService _orderService; // Added


        public CmsController(
            ILogger<CmsController> logger,
            IRestaurantManagement restaurantManagementService,
            IDishManagementService dishManagementService,
            ICategoryManagementService categoryManagementService,
            IUserManagementService userManagementService,
            IOrderService orderService) // Updated constructor
        {
            _logger = logger;
            _restaurantManagementService = restaurantManagementService;
            _dishManagementService = dishManagementService;
            _categoryManagementService = categoryManagementService;
            _userManagementService = userManagementService;
            _orderService = orderService; // Added
        }        [HttpGet("categories")]
        public async Task<AppResponse<List<CategoriesDto>>> GetAllCategories()
        {
            return await _categoryManagementService.GetAllCategoriesAsync();
        }

        [HttpGet("categories/{restaurantId}")]
        public async Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId)
        {
            return await _categoryManagementService.GetCategories(restaurantId);
        }

        [HttpGet("restaurants")]
        public async Task<AppResponse<List<AdminRestaurantDto>>> GetAllRestaurants() // Changed DTO to AdminRestaurantDto
        {
            return await _restaurantManagementService.GetAllRestaurants();
        }        [HttpGet("dishes")]
        public async Task<AppResponse<List<DishDto>>> GetAllDishes()
        {
            return await _dishManagementService.GetAllDishesAsync();
        }

        [HttpGet("users")]
        public async Task<AppResponse<List<UserDto>>> GetAllUsers()
        {
            return await _userManagementService.GetAllUsers();
        }

        [HttpGet("orders")]
        public async Task<AppResponse<List<OrderDto>>> GetAllOrders()
        {
            return await _orderService.GetAllOrdersAsync();
        }

        [HttpGet("orders/{id}")]
        public async Task<AppResponse<OrderDto>> GetOrderById(int id)
        {
            return await _orderService.GetOrderByIdAsync(id);
        }

        [HttpGet("orders/user/{userId}")]
        public async Task<AppResponse<List<OrderDto>>> GetOrdersByUser(int userId)
        {
            return await _orderService.GetOrdersByUserAsync(userId);
        }

        [HttpGet("orders/restaurant/{restaurantId}")]
        public async Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurant(int restaurantId)
        {
            return await _orderService.GetOrdersByRestaurantAsync(restaurantId);
        }

        [HttpGet("orders/employee/{employeeId}")]
        public async Task<AppResponse<List<OrderDto>>> GetOrdersByEmployee(int employeeId)
        {
            return await _orderService.GetOrdersByEmployeeAsync(employeeId);
        }

        [HttpPut("categories/{id}")]
        public async Task<AppResponse<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto) // Changed to UpdateCategoryDto
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<CategoryDto>(null, "Invalid model state", 400, false);
            }

            return await _categoryManagementService.UpdateCategory(id, categoryDto);
        }

        [HttpDelete("categories/{id}")]
        public async Task<AppResponse<bool>> DeleteCategory(int id)
        {
            return await _categoryManagementService.DeleteCategory(id);
        }

        [HttpPost("restaurants")]
        public async Task<AppResponse<AdminRestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto restaurantDto) // Changed to RestaurantCreateDto
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Invalid model state", 400, false);
            }

            return await _restaurantManagementService.CreateRestaurant(restaurantDto);
        }
        
        [HttpPut("restaurants/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto restaurantDto) // Changed to RestaurantUpdateDto
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Invalid model state", 400, false);
            }
            _logger.LogInformation($"Updating restaurant with ID: {id}"); // OpeningHours might not be in RestaurantUpdateDto
            return await _restaurantManagementService.UpdateRestaurant(id, restaurantDto);
        }

        [HttpDelete("restaurants/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> DeleteRestaurant(int id) // Return type changed to match service
        {
            return await _restaurantManagementService.DeleteRestaurant(id);
        }

        [HttpPost("create-dish")] // Route kept for now, consider aligning with DishManagement controller
        public async Task<AppResponse<AdminRestaurantDishDto>> CreateDish([FromBody] CreateDishDto dishDto)
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Invalid model state", 400, false);
            }

            return await _dishManagementService.CreateDish(dishDto);
        }

        [HttpPut("dishes/{id}")]
        public async Task<AppResponse<AdminRestaurantDishDto>> UpdateDish(int id, [FromBody] UpdateDishDto dishDto) // Changed to UpdateDishDto
        {
            if (!ModelState.IsValid)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Invalid model state", 400, false);
            }

            return await _dishManagementService.UpdateDish(id, dishDto);
        }

        [HttpDelete("dishes/{id}")]
        public async Task<AppResponse<bool>> DeleteDish(int id)
        {
            return await _dishManagementService.DeleteDish(id);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<AppResponse<bool>> UpdateOrderStatus(int id, [FromBody] string status) // Consider a DTO for status
        {
            return await _orderService.UpdateOrderStatusAsync(id, status);
        }

        [HttpDelete("users/{id}")]
        public async Task<AppResponse<bool>> DeleteUser(int id)
        {
            return await _userManagementService.DeleteUser(id);
        }
    }
}
