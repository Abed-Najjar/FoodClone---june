using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Requires authentication for all endpoints
    public class OrderController : ControllerBase
    {
        private readonly IOrderManagementService _orderManagementService;

        public OrderController(IOrderManagementService orderManagementService)
        {
            _orderManagementService = orderManagementService;
        }

        [HttpGet("{id}")]
        public async Task<AppResponse<OrderDto>> GetOrder(int id)
        {
            return await _orderManagementService.GetOrder(id);
        }

        [HttpGet]
        public async Task<AppResponse<List<OrderDto>>> GetAllOrders()
        {
            return await _orderManagementService.GetAllOrders();
        }
    }
}