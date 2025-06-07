using API.AppResponse;
using API.DTOs;
using API.Services.OrderServiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Requires authentication for all endpoints
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderManagementService;

        public OrderController(IOrderService orderManagementService)
        {
            _orderManagementService = orderManagementService;
        }

        [HttpGet("{id}")]
        public async Task<AppResponse<OrderDto>> GetOrder(int id)
        {
            return await _orderManagementService.GetOrderByIdAsync(id);
        }

        [HttpGet]
        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetAllOrders([FromQuery] PaginationDto? paginationDto = null)
        {
            return await _orderManagementService.GetAllOrdersAsync(paginationDto);
        }

        [HttpPost]
        public async Task<AppResponse<OrderDto>> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return new AppResponse<OrderDto>(null, "User not authenticated.", 401, false);
            }

            return await _orderManagementService.CreateOrderAsync(orderCreateDto, userId);
        }

        [HttpGet("my-orders")]
        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetMyOrders([FromQuery] PaginationDto? paginationDto = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return new AppResponse<PagedResultDto<OrderDto>>(null, "User not authenticated.", 401, false);
            }

            return await _orderManagementService.GetOrdersByUserAsync(userId, paginationDto);
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByRestaurant(int restaurantId, [FromQuery] PaginationDto? paginationDto = null)
        {
            return await _orderManagementService.GetOrdersByRestaurantAsync(restaurantId, paginationDto);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByEmployee(int employeeId, [FromQuery] PaginationDto? paginationDto = null)
        {
            return await _orderManagementService.GetOrdersByEmployeeAsync(employeeId, paginationDto);
        }

        [HttpPut("{id}/status")]
        public async Task<AppResponse<bool>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateDto)
        {
            return await _orderManagementService.UpdateOrderStatusAsync(id, updateDto.Status);
        }

        
    }
}