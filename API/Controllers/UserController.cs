using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Services.UserServiceFolder;


namespace API.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [ApiController]
    [Route("api/[controller]")]    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRestaurantManagement _restaurantManagement;

        public UserController(IUserService userService, IRestaurantManagement restaurantManagement)
        {
            _userService = userService;
            _restaurantManagement = restaurantManagement;
        }
        
        [HttpPost("createOrder")]
        public async Task<AppResponse<OrderDto>> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            return await _userService.CreateOrder(orderCreateDto);
        }
        
        [HttpGet("restaurants")]
        [AllowAnonymous]
        public async Task<AppResponse<List<UserRestaurantDto>>> GetAllRestaurants()
        {
            return await _userService.GetAllRestaurants();
        }
        
        [HttpGet("dishes/{restaurantId}")]
        [AllowAnonymous]
        public async Task<AppResponse<List<UserRestaurantDishesDto>>> GetDishesByRestaurantId(int restaurantId)
        {
            return await _userService.GetAllRestaurantDishes(restaurantId);
        }
        
        [HttpGet("categories/{restaurantId}")]
        [AllowAnonymous]
        public async Task<AppResponse<List<UserRestaurantCategoriesDto>>> GetCategoriesByRestaurantId(int restaurantId)
        {
            return await _userService.GetAllRestaurantCategories(restaurantId);
        }

    }
}