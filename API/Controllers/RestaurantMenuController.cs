using API.AppResponse;
using API.DTOs;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantMenuController : ControllerBase
    {
        private readonly IRestaurantMenuService _restaurantMenuService;

        public RestaurantMenuController(IRestaurantMenuService restaurantMenuService)
        {
            _restaurantMenuService = restaurantMenuService;
        }

        // Category Endpoints
        [HttpGet("categories/{restaurantId}")]
        public async Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId)
        {
            return await _restaurantMenuService.GetCategories(restaurantId);
        }

        [Authorize]
        [HttpPost("categories")]
        public async Task<AppResponse<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            return await _restaurantMenuService.CreateCategory(categoryDto);
        }

        [Authorize]
        [HttpPut("categories/{id}")]
        public async Task<AppResponse<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto)
        {
            return await _restaurantMenuService.UpdateCategory(id, categoryDto);
        }

        [Authorize]
        [HttpDelete("categories/{id}")]
        public async Task<AppResponse<bool>> DeleteCategory(int id)
        {
            return await _restaurantMenuService.DeleteCategory(id);
        }

        // Dish Endpoints
        [HttpGet("dishes/{restaurantId}")]
        public async Task<AppResponse<List<DishDto>>> GetDishes(int restaurantId)
        {
            return await _restaurantMenuService.GetDishes(restaurantId);
        }

        [Authorize]
        [HttpPost("dishes")]
        public async Task<AppResponse<DishDto>> CreateDish([FromBody] CreateDishDto dishDto)
        {
            return await _restaurantMenuService.CreateDish(dishDto);
        }

        [Authorize]
        [HttpPut("dishes/{id}")]
        public async Task<AppResponse<DishDto>> UpdateDish(int id, [FromBody] UpdateDishDto dishDto)
        {
            return await _restaurantMenuService.UpdateDish(id, dishDto);
        }

        [Authorize]
        [HttpDelete("dishes/{id}")]
        public async Task<AppResponse<bool>> DeleteDish(int id)
        {
            return await _restaurantMenuService.DeleteDish(id);
        }
    }
} 