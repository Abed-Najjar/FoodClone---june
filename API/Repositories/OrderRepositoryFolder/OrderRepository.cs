using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories.OrderRepositoryFolder // New folder
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(AppDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AppResponse<List<OrderDto>>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found.", 200, false);
            }

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),
            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }
        
        public async Task<AppResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return new AppResponse<OrderDto>(null, "Order not found.", 404, false);
            }

            var orderDto = new OrderDto
            {
                 Id = order.Id,
                UserId = order.UserId,
                RestaurantId = order.RestaurantId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.CreatedAt,
                UserName = order.User.UserName,
                RestaurantName = order.Restaurant.Name,
                EmployeeId = order.EmployeeId ?? 0,
                EmployeeName = order.Employee != null ? order.Employee.UserName : "Not Assigned",
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),
            };

            return new AppResponse<OrderDto>(orderDto);
        }        public async Task<AppResponse<List<OrderDto>>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();
            
            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found for this user.", 200, false);
            }

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),
            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }        public async Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId)
        {
            var orders = await _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found for this restaurant.", 200, false);
            }

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),
            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }        public async Task<AppResponse<List<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId)
        {
            var orders = await _context.Orders
                .Where(o => o.EmployeeId == employeeId)
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return new AppResponse<List<OrderDto>>(null, "No orders found for this employee.", 200, false);
            }

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                RestaurantId = o.RestaurantId,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.CreatedAt,
                UserName = o.User.UserName,
                RestaurantName = o.Restaurant.Name,
                EmployeeId = o.EmployeeId ?? 0,
                EmployeeName = o.Employee != null ? o.Employee.UserName : "Not Assigned",
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    DishId = oi.DishId,
                    DishName = oi.Dish.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.Quantity * oi.UnitPrice
                }).ToList(),
            }).ToList();

            return new AppResponse<List<OrderDto>>(orderDtos);
        }

        public async Task<AppResponse<bool>> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new AppResponse<bool>(false, "Order not found.", 404, false);
            }

            order.Status = status;
            _context.Orders.Update(order);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                return new AppResponse<bool>(false, "Failed to update order status.", 500, false);
            }

            return new AppResponse<bool>(true, "Order status updated successfully.", 200, true);
        }
    }
}
