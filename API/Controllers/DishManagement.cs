using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Provider,Admin")]
    [Route("api/[controller]")]
    public class DishManagement : ControllerBase
    {
        private readonly IDishManagementService _dishManagementService;

        public DishManagement(IDishManagementService dishManagementService)
        {
            _dishManagementService = dishManagementService;
        }
        
        
        [HttpGet("restaurant/dishes/{restaurantId}")]
        public async Task<AppResponse<List<AdminRestaurantDishDto>>> GetDishes(int restaurantId)
        {
            return await _dishManagementService.GetDishesInRestaurant(restaurantId);
        }

        [HttpGet("dishes/{id}")]
        public async Task<AppResponse<AdminRestaurantDishDto>> GetDishById(int id)
        {
            return await _dishManagementService.GetDishById(id);
        }
        
        [HttpPost("dishes")]
        public async Task<AppResponse<AdminRestaurantDishDto>> CreateDish(CreateDishDto dishDto)
        {
            return await _dishManagementService.CreateDish(dishDto);
        }

        [HttpPut("dishes/{id}")]
        public async Task<AppResponse<AdminRestaurantDishDto>> UpdateDish(int id, UpdateDishDto dishDto)
        {
            return await _dishManagementService.UpdateDish(id, dishDto);
        }

        [HttpDelete("dishes/{id}")]
        public async Task<AppResponse<bool>> DeleteDish(int id)
        {
            return await _dishManagementService.DeleteDish(id);
        }

        
        
        
        
        
        
    }
}   