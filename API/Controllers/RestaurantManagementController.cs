using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class RestaurantManagementController : ControllerBase
    {
        private readonly IRestaurantManagement _restaurantManagementService;

        public RestaurantManagementController(IRestaurantManagement restaurantManagementService)
        {
            _restaurantManagementService = restaurantManagementService;
        }

        [HttpPost("create")]
        public async Task<AppResponse<AdminRestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto)
        {
            return await _restaurantManagementService.CreateRestaurant(dto);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("getAll")]
        public async Task<AppResponse<List<AdminRestaurantDto>>> GetAllRestaurants()
        {
            return await _restaurantManagementService.GetAllRestaurants();
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("get/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> GetRestaurant(int id)
        {
            return await _restaurantManagementService.GetRestaurant(id);
        }

        [HttpPut("update/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto dto)
        {
            return await _restaurantManagementService.UpdateRestaurant(id, dto);
        }

        [HttpDelete("delete/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> DeleteRestaurant(int id)
        {
            return await _restaurantManagementService.DeleteRestaurant(id);
        }
        
        [HttpPut("suspend/{id}")]
        public async Task<AppResponse<bool>> SuspendRestaurant(int id)
        {
            return await _restaurantManagementService.SuspendRestaurant(id);
        }

        [HttpPut("unsuspend/{id}")]
        public async Task<AppResponse<bool>> UnsuspendRestaurant(int id)
        {
            return await _restaurantManagementService.UnsuspendRestaurant(id);
        }


        [Authorize(Roles = "Provider")]
        [HttpPut("open/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> OpenRestaurant(int id)
        {
            return await _restaurantManagementService.OpenRestaurant(id);
        }

        [Authorize(Roles = "Provider")]
        [HttpPut("close/{id}")]
        public async Task<AppResponse<AdminRestaurantDto>> CloseRestaurant(int id)
        {
            return await _restaurantManagementService.CloseRestaurant(id);
        
        }
    }
}


