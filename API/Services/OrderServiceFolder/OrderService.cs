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

                var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();                if (orders == null || !orders.Any())
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
                    DeliveryAddressId = o.DeliveryAddressId,
                    DeliveryAddress = o.DeliveryAddress?.FormattedAddress ?? "",
                    DeliveryInstructions = o.DeliveryInstructions ?? "",
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

        public async Task<AppResponse<OrderDto>> CreateOrderAsync(OrderCreateDto orderCreateDto, int userId)
        {
            try
            {
                _logger.LogInformation($"CreateOrderAsync called for user ID: {userId}");

                // Validate input
                if (orderCreateDto.OrderItems == null || !orderCreateDto.OrderItems.Any())
                {
                    return new AppResponse<OrderDto>(null, "Order must contain at least one item.", 400, false);
                }

                // Verify restaurant exists
                var restaurant = await _unitOfWork.OrderRepository.GetRestaurantByIdAsync(orderCreateDto.RestaurantId);
                if (restaurant == null)
                {
                    return new AppResponse<OrderDto>(null, "Restaurant not found.", 404, false);
                }

                // Check if restaurant is open
                if (!restaurant.IsOpen)
                {
                    return new AppResponse<OrderDto>(null, "Restaurant is currently closed.", 400, false);
                }

                var orderItems = new List<Models.OrderDish>();
                decimal subtotal = 0;

                // Validate each order item and calculate subtotal
                foreach (var item in orderCreateDto.OrderItems)
                {
                    var dish = await _unitOfWork.OrderRepository.GetDishByIdAsync(item.DishId);
                    if (dish == null)
                    {
                        return new AppResponse<OrderDto>(null, $"Dish with ID {item.DishId} not found.", 404, false);
                    }

                    if (!dish.IsAvailable)
                    {
                        return new AppResponse<OrderDto>(null, $"Dish '{dish.Name}' is currently unavailable.", 400, false);
                    }

                    if (dish.RestaurantId != orderCreateDto.RestaurantId)
                    {
                        return new AppResponse<OrderDto>(null, $"Dish '{dish.Name}' does not belong to the selected restaurant.", 400, false);
                    }

                    if (item.Quantity <= 0)
                    {
                        return new AppResponse<OrderDto>(null, $"Invalid quantity for dish '{dish.Name}'.", 400, false);
                    }

                    var orderItem = new Models.OrderDish
                    {
                        DishId = item.DishId,
                        Quantity = item.Quantity,
                        UnitPrice = dish.Price
                    };

                    orderItems.Add(orderItem);
                    subtotal += dish.Price * item.Quantity;
                }                // Calculate order totals
                decimal deliveryFee = restaurant.DeliveryFee;
                decimal taxRate = 0.15m; // 15% tax rate - could be configurable
                decimal taxAmount = subtotal * taxRate;
                decimal totalAmount = subtotal + deliveryFee + taxAmount;

                // Validate delivery address if provided
                string? deliveryAddressText = null;
                if (orderCreateDto.DeliveryAddressId.HasValue)
                {
                    var address = await _unitOfWork.OrderRepository.GetAddressByIdAsync(orderCreateDto.DeliveryAddressId.Value, userId);
                    if (address == null)
                    {
                        return new AppResponse<OrderDto>(null, "Invalid delivery address selected.", 400, false);
                    }
                    deliveryAddressText = address.FormattedAddress;
                }

                // Create the order
                var order = new Models.Order
                {
                    UserId = userId,
                    RestaurantId = orderCreateDto.RestaurantId,
                    PaymentMethod = orderCreateDto.PaymentMethod ?? "Cash",
                    Status = "Pending",
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                    DeliveryAddressId = orderCreateDto.DeliveryAddressId,
                    DeliveryInstructions = orderCreateDto.DeliveryInstructions,
                    OrderItems = orderItems
                };

                // Save the order
                var createdOrder = await _unitOfWork.OrderRepository.CreateOrderAsync(order);
                await _unitOfWork.CompleteAsync();                // Convert to DTO
                var orderDto = new OrderDto
                {
                    Id = createdOrder.Id,
                    UserId = createdOrder.UserId,
                    RestaurantId = createdOrder.RestaurantId,
                    TotalAmount = createdOrder.TotalAmount,
                    Status = createdOrder.Status,
                    PaymentMethod = createdOrder.PaymentMethod,
                    OrderDate = createdOrder.CreatedAt,
                    UserName = createdOrder.User?.UserName ?? "Unknown",
                    RestaurantName = createdOrder.Restaurant?.Name ?? restaurant.Name,
                    EmployeeId = createdOrder.EmployeeId ?? 0,
                    EmployeeName = createdOrder.Employee?.UserName ?? "Not Assigned",
                    DeliveryAddressId = createdOrder.DeliveryAddressId,
                    DeliveryAddress = deliveryAddressText ?? "",
                    DeliveryInstructions = createdOrder.DeliveryInstructions ?? "",
                    OrderItems = createdOrder.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        DishId = oi.DishId,
                        DishName = oi.Dish?.Name ?? "Unknown",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                };

                _logger.LogInformation($"Order created successfully with ID: {createdOrder.Id}");
                return new AppResponse<OrderDto>(orderDto, "Order created successfully.", 201, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating order for user ID: {userId}");
                return new AppResponse<OrderDto>(null, ex.Message, 500, false);
            }
        }
    }
}
