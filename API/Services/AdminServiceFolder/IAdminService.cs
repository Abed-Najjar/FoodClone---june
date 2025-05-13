using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IAdminService
{
    Task<AppResponse<UserDto>> CreateUser([FromBody] UserInputDto dto);
    Task<AppResponse<List<UserDto>>> GetAllUsers();
    Task<AppResponse<UserDto>> GetUser(int id);
    Task<AppResponse<bool>> DeleteUser(int id);
    Task<AppResponse<UserDto>> UpdateUser(int id, [FromBody] UserInputDto dto);
    Task<AppResponse<RestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto);
    Task<AppResponse<List<RestaurantDto>>> GetAllRestaurants();
    Task<AppResponse<RestaurantDto>> GetRestaurant(int id);
    Task<AppResponse<RestaurantDto>> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto dto);
    Task<AppResponse<RestaurantDto>> DeleteRestaurant(int id);
    Task<AppResponse<bool>> SuspendRestaurant(int id);
    Task<AppResponse<bool>> UnsuspendRestaurant(int id);
    Task<AppResponse<List<OrderDto>>> GetAllOrders();
    Task<AppResponse<OrderDto>> GetOrder(int id);
}
