using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IRestaurantManagement
{
    Task<AppResponse<RestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto);
    Task<AppResponse<List<RestaurantDto>>> GetAllRestaurants();
    Task<AppResponse<RestaurantDto>> GetRestaurant(int id);
    Task<AppResponse<RestaurantDto>> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto dto);
    Task<AppResponse<RestaurantDto>> DeleteRestaurant(int id);
    Task<AppResponse<bool>> SuspendRestaurant(int id);
    Task<AppResponse<bool>> UnsuspendRestaurant(int id);
}
