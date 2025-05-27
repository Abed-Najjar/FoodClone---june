using API.AppResponse;
using API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories.OrderRepositoryFolder // New folder
{
    public interface IOrderRepository
    {
        Task<AppResponse<List<OrderDto>>> GetAllOrdersAsync();
        Task<AppResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<AppResponse<List<OrderDto>>> GetOrdersByUserAsync(int userId);
        Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId);
        Task<AppResponse<List<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId);
        Task<AppResponse<bool>> UpdateOrderStatusAsync(int id, string status);
        // Define other necessary methods that were in CmsRepository for orders
    }
}
