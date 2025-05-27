
using API.AppResponse;
using API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.OrderServiceFolder
{
    public interface IOrderService
    {
        Task<AppResponse<List<OrderDto>>> GetAllOrdersAsync();
        Task<AppResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<AppResponse<List<OrderDto>>> GetOrdersByUserAsync(int userId);
        Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId);
        Task<AppResponse<List<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId);
        Task<AppResponse<bool>> UpdateOrderStatusAsync(int id, string status);
    }
}
