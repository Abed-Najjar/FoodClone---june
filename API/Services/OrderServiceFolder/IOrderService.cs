
using API.AppResponse;
using API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.OrderServiceFolder
{
    public interface IOrderService
    {
        Task<AppResponse<PagedResultDto<OrderDto>>> GetAllOrdersAsync(PaginationDto? paginationDto = null);
        Task<AppResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByUserAsync(int userId, PaginationDto? paginationDto = null);
        Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId, PaginationDto? paginationDto = null);
        Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId, PaginationDto? paginationDto = null);
        Task<AppResponse<bool>> UpdateOrderStatusAsync(int id, string status);
        Task<AppResponse<OrderDto>> CreateOrderAsync(OrderCreateDto orderCreateDto, int userId);
        Task<AppResponse<bool>> DeleteOrderAsync(int id);
    }
}
