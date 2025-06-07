using API.AppResponse;
using API.DTOs;
using API.Models;
using API.UoW;
using API.Services.PricingServiceFolder;
using Microsoft.Extensions.Logging;

namespace API.Services.OrderServiceFolder
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly IPricingService _pricingService;

        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger, IPricingService pricingService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _pricingService = pricingService;
        }

        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetAllOrdersAsync(PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation("GetAllOrdersAsync called");

                var orders = await _unitOfWork.OrderRepository.GetAllOrdersAsync();

                var orderDtos = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    RestaurantId = o.RestaurantId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    OrderDate = o.CreatedAt,
                    UserName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee != null ? $"{o.Employee.FirstName} {o.Employee.LastName}" : "Not Assigned",
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

                // Always return paginated result
                var totalItems = orderDtos.Count;
                var pageNumber = paginationDto?.PageNumber ?? 1;
                var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
                
                var paginatedData = orderDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultDto<OrderDto>(paginatedData, totalItems, pageNumber, pageSize);
                return new AppResponse<PagedResultDto<OrderDto>>(pagedResult, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all orders");
                return new AppResponse<PagedResultDto<OrderDto>>(null, ex.Message, 500, false);
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
                    UserName = order.User != null ? $"{order.User.FirstName} {order.User.LastName}" : "Unknown",
                    RestaurantName = order.Restaurant?.Name ?? "Unknown",
                    EmployeeId = order.EmployeeId ?? 0,
                    EmployeeName = order.Employee != null ? $"{order.Employee.FirstName} {order.Employee.LastName}" : "Not Assigned",
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

        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByUserAsync(int userId, PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation($"GetOrdersByUserAsync called for user ID: {userId}");

                var orders = await _unitOfWork.OrderRepository.GetOrdersByUserAsync(userId);

                var orderDtos = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    RestaurantId = o.RestaurantId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    OrderDate = o.CreatedAt,
                    UserName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee != null ? $"{o.Employee.FirstName} {o.Employee.LastName}" : "Not Assigned",
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

                // Always return paginated result
                var totalItems = orderDtos.Count;
                var pageNumber = paginationDto?.PageNumber ?? 1;
                var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
                
                var paginatedData = orderDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultDto<OrderDto>(paginatedData, totalItems, pageNumber, pageSize);
                return new AppResponse<PagedResultDto<OrderDto>>(pagedResult, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting orders for user ID: {userId}");
                return new AppResponse<PagedResultDto<OrderDto>>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByRestaurantAsync(int restaurantId, PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation($"GetOrdersByRestaurantAsync called for restaurant ID: {restaurantId}");

                var orders = await _unitOfWork.OrderRepository.GetOrdersByRestaurantAsync(restaurantId);

                var orderDtos = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    RestaurantId = o.RestaurantId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    OrderDate = o.CreatedAt,
                    UserName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee != null ? $"{o.Employee.FirstName} {o.Employee.LastName}" : "Not Assigned",
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

                // Always return paginated result
                var totalItems = orderDtos.Count;
                var pageNumber = paginationDto?.PageNumber ?? 1;
                var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
                
                var paginatedData = orderDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultDto<OrderDto>(paginatedData, totalItems, pageNumber, pageSize);
                return new AppResponse<PagedResultDto<OrderDto>>(pagedResult, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting orders for restaurant ID: {restaurantId}");
                return new AppResponse<PagedResultDto<OrderDto>>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<PagedResultDto<OrderDto>>> GetOrdersByEmployeeAsync(int employeeId, PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation($"GetOrdersByEmployeeAsync called for employee ID: {employeeId}");

                var orders = await _unitOfWork.OrderRepository.GetOrdersByEmployeeAsync(employeeId);

                var orderDtos = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    RestaurantId = o.RestaurantId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    OrderDate = o.CreatedAt,
                    UserName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Unknown",
                    RestaurantName = o.Restaurant?.Name ?? "Unknown",
                    EmployeeId = o.EmployeeId ?? 0,
                    EmployeeName = o.Employee != null ? $"{o.Employee.FirstName} {o.Employee.LastName}" : "Not Assigned",
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

                // Always return paginated result
                var totalItems = orderDtos.Count;
                var pageNumber = paginationDto?.PageNumber ?? 1;
                var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
                
                var paginatedData = orderDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResult = new PagedResultDto<OrderDto>(paginatedData, totalItems, pageNumber, pageSize);
                return new AppResponse<PagedResultDto<OrderDto>>(pagedResult, "Orders retrieved successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting orders for employee ID: {employeeId}");
                return new AppResponse<PagedResultDto<OrderDto>>(null, ex.Message, 500, false);
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

        public async Task<AppResponse<bool>> DeleteOrderAsync(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteOrderAsync called for order ID: {id}");

                var result = await _unitOfWork.OrderRepository.DeleteOrderAsync(id);

                if (!result)
                {
                    return new AppResponse<bool>(false, "Order not found or failed to delete.", 404, false);
                }

                await _unitOfWork.CompleteAsync();

                return new AppResponse<bool>(true, "Order deleted successfully.", 200, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting order with ID: {id}");
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

                // Convert order items to pricing items format for validation and calculation
                var pricingItems = orderCreateDto.OrderItems.Select(item => new PricingItemDto
                {
                    DishId = item.DishId,
                    Quantity = item.Quantity
                }).ToList();

                // Use the centralized pricing service for consistent calculation
                var pricingResult = await _pricingService.CalculateOrderTotalsAsync(
                    pricingItems,
                    orderCreateDto.RestaurantId,
                    null, // No promo code for now (can be added later)
                    orderCreateDto.DeliveryAddressId,
                    userId
                );

                // Handle pricing service errors
                if (!pricingResult.IsValid)
                {
                    _logger.LogWarning($"Pricing calculation failed: {pricingResult.ErrorMessage}");
                    return new AppResponse<OrderDto>(null, pricingResult.ErrorMessage, 400, false);
                }

                // Create order items using validated data from pricing service
                var orderItems = pricingResult.ItemDetails.Select(item => new Models.OrderDish
                {
                    DishId = item.DishId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList();

                decimal totalAmount = pricingResult.GrandTotal;

                // Create the order
                var order = new Models.Order
                {
                    UserId = userId,
                    RestaurantId = orderCreateDto.RestaurantId,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    PaymentMethod = orderCreateDto.PaymentMethod,
                    DeliveryAddressId = orderCreateDto.DeliveryAddressId,
                    DeliveryInstructions = orderCreateDto.DeliveryInstructions,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = orderItems
                };

                // Save the order
                var createdOrder = await _unitOfWork.OrderRepository.CreateOrderAsync(order);
                await _unitOfWork.CompleteAsync();

                // Fetch the created order with related data for response
                var orderWithDetails = await _unitOfWork.OrderRepository.GetOrderByIdAsync(createdOrder.Id);

                var orderDto = new OrderDto
                {
                    Id = orderWithDetails.Id,
                    UserId = orderWithDetails.UserId,
                    RestaurantId = orderWithDetails.RestaurantId,
                    TotalAmount = orderWithDetails.TotalAmount,
                    Status = orderWithDetails.Status,
                    PaymentMethod = orderWithDetails.PaymentMethod,
                    OrderDate = orderWithDetails.CreatedAt,
                    UserName = orderWithDetails.User != null ? $"{orderWithDetails.User.FirstName} {orderWithDetails.User.LastName}" : "Unknown",
                    RestaurantName = orderWithDetails.Restaurant?.Name ?? "Unknown",
                    EmployeeId = orderWithDetails.EmployeeId ?? 0,
                    EmployeeName = orderWithDetails.Employee != null ? $"{orderWithDetails.Employee.FirstName} {orderWithDetails.Employee.LastName}" : "Not Assigned",
                    OrderItems = orderWithDetails.OrderItems?.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        DishId = oi.DishId,
                        DishName = oi.Dish?.Name ?? "Unknown",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList() ?? new List<OrderItemDto>()
                };

                return new AppResponse<OrderDto>(orderDto, "Order created successfully.", 201, true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating order for user ID: {userId}");
                return new AppResponse<OrderDto>(null, ex.Message, 500, false);
            }
        }
    }
}
