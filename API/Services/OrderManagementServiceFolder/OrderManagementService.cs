using API.AppResponse;
using API.Data;
using API.DTOs;
using Microsoft.EntityFrameworkCore;

public class OrderManagementService : IOrderManagementService
{
    private readonly AppDbContext _context;

    public OrderManagementService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<OrderDto>> GetOrder(int id)
    {
        try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Restaurant)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return new AppResponse<OrderDto>(null, "Order not found", 404, false);
                }

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    UserId = order.UserId,
                    RestaurantId = order.RestaurantId,
                    EmployeeId = order.EmployeeId ?? 0,
                    UserName = order.User.UserName,
                    RestaurantName = order.Restaurant.Name
                };

                return new AppResponse<OrderDto>(orderDto, "Order retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<OrderDto>(null, ex.Message, 500, false);
            }
    }

    public async Task<AppResponse<List<OrderDto>>> GetAllOrders()
    {
        try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Restaurant)
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return new AppResponse<List<OrderDto>>(null, "No orders found", 404, false);
                }

                var orderDtos = orders.Select(order => new OrderDto
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    UserId = order.UserId,
                    RestaurantId = order.RestaurantId,
                    EmployeeId = order.EmployeeId ?? 0,
                    UserName = order.User.UserName,
                    RestaurantName = order.Restaurant.Name
                }).ToList();

                return new AppResponse<List<OrderDto>>(orderDtos, "Orders retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<OrderDto>>(null, ex.Message, 500, false);
            }

    }

}