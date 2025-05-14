using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IRestaurantManagement
{
    Task<AppResponse<AdminRestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto);
    Task<AppResponse<List<AdminRestaurantDto>>> GetAllRestaurants();
    Task<AppResponse<AdminRestaurantDto>> GetRestaurant(int id);
    Task<AppResponse<AdminRestaurantDto>> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto dto);
    Task<AppResponse<AdminRestaurantDto>> DeleteRestaurant(int id);
    Task<AppResponse<bool>> SuspendRestaurant(int id);
    Task<AppResponse<bool>> UnsuspendRestaurant(int id);
    Task<AppResponse<AdminRestaurantDto>> OpenRestaurant(int id);
    Task<AppResponse<AdminRestaurantDto>> CloseRestaurant(int id);
}
