using API.AppResponse;
using API.DTOs;
using API.UoW;

namespace API.Services.OrderServiceFolder
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AppResponse<List<OrderDto>>> GetAllOrdersAsync()
        {
            try
            {
                _logger.LogInformation("GetAllOrdersAsync called");

                var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();

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
                    UserName = o.User?.UserName ?? "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee?.UserName ?? "Not Assigned",
                    OrderItems = o.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        DishId = oi.DishId,
                        DishName = oi.Dish?.Name ?? "Unknown",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                }).ToList();

                return new AppResponse<List<OrderDto>>(orderDtos, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all orders");
                return new AppResponse<List<OrderDto>>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"GetOrderByIdAsync called with ID: {id}");

                var order = await _unitOfWork.OrderRepository.GetOrderByIdAsync(id);

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
                    UserName = order.User?.UserName ?? "Unknown",
                    RestaurantName = order.Restaurant?.Name ?? "Unknown",
                    EmployeeId = order.EmployeeId ?? 0,
                    EmployeeName = order.Employee?.UserName ?? "Not Assigned",
                    OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        DishId = oi.DishId,
                        DishName = oi.Dish?.Name ?? "Unknown",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                };

                return new AppResponse<OrderDto>(orderDto, "Order retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting order with ID: {id}");
                return new AppResponse<OrderDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<List<OrderDto>>> GetOrdersByUserAsync(int userId)
        {
            try
            {
                _logger.LogInformation($"GetOrdersByUserAsync called for user ID: {userId}");

                var orders = await _unitOfWork.OrderRepository.GetOrdersByUserAsync(userId);

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
                    UserName = o.User?.UserName ?? "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee?.UserName ?? "Not Assigned",
                    OrderItems = o.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        DishId = oi.DishId,
                        DishName = oi.Dish?.Name ?? "Unknown",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                }).ToList();

                return new AppResponse<List<OrderDto>>(orderDtos, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting orders for user ID: {userId}");
                return new AppResponse<List<OrderDto>>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<List<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId)
        {
            try
            {
                _logger.LogInformation($"GetOrdersByRestaurantAsync called for restaurant ID: {restaurantId}");

                var orders = await _unitOfWork.OrderRepository.GetOrdersByRestaurantAsync(restaurantId);

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
                    UserName = o.User?.UserName ?? "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee?.UserName ?? "Not Assigned",
                    OrderItems = o.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        DishId = oi.DishId,
                        DishName = oi.Dish?.Name ?? "Unknown",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                }).ToList();

                return new AppResponse<List<OrderDto>>(orderDtos, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting orders for restaurant ID: {restaurantId}");
                return new AppResponse<List<OrderDto>>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<List<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation($"GetOrdersByEmployeeAsync called for employee ID: {employeeId}");

                var orders = await _unitOfWork.OrderRepository.GetOrdersByEmployeeAsync(employeeId);

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
                    UserName = o.User?.UserName ?? "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee?.UserName ?? "Not Assigned",
                    OrderItems = o.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        DishId = oi.DishId,
                        DishName = oi.Dish?.Name ?? "Unknown",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                }).ToList();

                return new AppResponse<List<OrderDto>>(orderDtos, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting orders for employee ID: {employeeId}");
                return new AppResponse<List<OrderDto>>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<bool>> UpdateOrderStatusAsync(int id, string status)
        {
            try
            {
                _logger.LogInformation($"UpdateOrderStatusAsync called for order ID: {id} with status: {status}");

                var result = await _unitOfWork.OrderRepository.UpdateOrderStatusAsync(id, status);

                if (!result)
                {
                    return new AppResponse<bool>(false, "Order not found or failed to update.", 404, false);
                }

                await _unitOfWork.CompleteAsync();

                return new AppResponse<bool>(true, "Order status updated successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating order status for ID: {id}");
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }
        }
    }
}
