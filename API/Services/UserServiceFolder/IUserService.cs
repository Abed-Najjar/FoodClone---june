using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Services.UserServiceFolder
{
    public interface IUserService
    {
        Task<AppResponse<OrderDto>> CreateOrder([FromBody] OrderCreateDto orderCreateDto);
        Task<AppResponse<List<UserRestaurantDto>>> GetAllRestaurants();
        
    }
}