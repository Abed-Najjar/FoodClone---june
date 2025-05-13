using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        private readonly IAdminService _adminService = adminService;

        [HttpPost("createUser")]
        public async Task<AppResponse<UserDto>> CreateUser([FromBody] UserInputDto dto)
        {
            return await _adminService.CreateUser(dto);
        }

        [HttpGet("getAllUsers")]
        public async Task<AppResponse<List<UserDto>>> GetAllUsers()
        {
            return await _adminService.GetAllUsers();

        }

        [HttpGet("getUser/{id}")]
        public async Task<AppResponse<UserDto>> GetUser(int id)
        {
            return await _adminService.GetUser(id);

        }

        [HttpDelete("deleteUser/{id}")]
        public async Task<AppResponse<bool>> DeleteUser(int id)
        {
            return await _adminService.DeleteUser(id);

        }

        [HttpPut("updateUser/{id}")]
        public async Task<AppResponse<UserDto>> UpdateUser(int id, [FromBody] UserInputDto dto)
        {
            return await _adminService.UpdateUser(id, dto);

        }


        [HttpPost("createRestaurant")]
        public async Task<AppResponse<RestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto)
        {
            return await _adminService.CreateRestaurant(dto);
        }

        [HttpGet("getAllRestaurants")]
        public async Task<AppResponse<List<RestaurantDto>>> GetAllRestaurants()
        {
            return await _adminService.GetAllRestaurants();

        }

        [HttpGet("getRestaurant/{id}")]
        public async Task<AppResponse<RestaurantDto>> GetRestaurant(int id)
        {
            return await _adminService.GetRestaurant(id);
        }

        [HttpPut("updateRestaurant/{id}")]
        public async Task<AppResponse<RestaurantDto>> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto dto)
        {
            return await _adminService.UpdateRestaurant(id, dto);
        }

        [HttpDelete("deleteRestaurant/{id}")]
        public async Task<AppResponse<RestaurantDto>> DeleteRestaurant(int id)
        {
            return await _adminService.DeleteRestaurant(id);
        }

        [HttpPut("suspendRestaurant/{id}")]
        public async Task<AppResponse<bool>> SuspendRestaurant(int id)
        {
            return await _adminService.SuspendRestaurant(id);
        }

        [HttpPut("unsuspendRestaurant/{id}")]
        public async Task<AppResponse<bool>> UnsuspendRestaurant(int id)
        {
            return await _adminService.UnsuspendRestaurant(id);
        }

        [HttpGet("getAllOrders")]
        public async Task<AppResponse<List<OrderDto>>> GetAllOrders()
        {
            return await _adminService.GetAllOrders();
        }

        [HttpGet("getOrder/{id}")]
        public async Task<AppResponse<OrderDto>> GetOrder(int id)
        {
            return await _adminService.GetOrder(id);
        }
     
    }
}


