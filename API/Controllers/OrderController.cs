using API.AppResponse;
using API.DTOs;
using API.Services.OrderServiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<AppResponse<List<OrderDto>>> GetAllOrders()
        {
            return await _orderManagementService.GetAllOrdersAsync();
        }

        
    }
}