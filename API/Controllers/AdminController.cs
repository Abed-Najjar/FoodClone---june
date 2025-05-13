using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Migrations;
using API.Models;
using API.Services.Argon;
using API.Services.TokenServiceFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IArgonHashing _argonHashing;
        private readonly ITokenService _tokenService;
        public AdminController(AppDbContext context, IArgonHashing argonHashing, ITokenService tokenService)
        {
            _context = context;
            _argonHashing = argonHashing;
            _tokenService = tokenService;
        }

        [HttpPost("createUser")]
        public async Task<AppResponse<UserDto>> CreateUser([FromBody] UserInputDto dto)
        {

            try
            {
                var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

                if (existingUser != null)
                {
                    return new AppResponse<UserDto>(null, "User already exists", 404, false);
                }

                var user = new User
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password), // Hash the password here
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();


                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                };

                return new AppResponse<UserDto>(userDto, "User created successfully", 201, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }

        }

        [HttpGet("getAllUsers")]
        public async Task<AppResponse<List<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                .Where(u => u.Role != Enums.Roles.Admin)
                .ToListAsync();

                if (users == null || !users.Any())
                {
                    return new AppResponse<List<UserDto>>(null, "No users found", 404, false);
                }

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                }).ToList();

                return new AppResponse<List<UserDto>>(userDtos, "Users retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<UserDto>>(null, ex.Message, 500, false);
            }

        }

        [HttpGet("getUser/{id}")]
        public async Task<AppResponse<UserDto>> GetUser(int id)
        {

            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return new AppResponse<UserDto>(null, "User not found", 404, false);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                };

                return new AppResponse<UserDto>(userDto, "User retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }

        }

        [HttpDelete("deleteUser/{id}")]
        public async Task<AppResponse<bool>> DeleteUser(int id)
        {

            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return new AppResponse<bool>(false, "User not found", 404, false);
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new AppResponse<bool>(true, "User deleted successfully", 200, true);

            }
            catch (Exception ex)
            {
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }

        }

        [HttpPut("updateUser/{id}")]
        public async Task<AppResponse<UserDto>> UpdateUser(int id, [FromBody] UserInputDto dto)
        {

            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return new AppResponse<UserDto>(null, "User not found", 404, false);
                }

                user.UserName = dto.Username;
                user.Email = dto.Email;
                user.PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password); // Hash the password here

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                };

                return new AppResponse<UserDto>(userDto, "User updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }

        }


        [HttpPost("createRestaurant")]
        public async Task<AppResponse<RestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto)
        {
            try
            {
                var restaurant = new Restaurant
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    LogoUrl = dto.LogoUrl,
                    CoverImageUrl = dto.CoverImageUrl,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    OpeningHours = dto.OpeningHours,
                    CreatedAt = DateTime.UtcNow
                };

                // Convert List<string> Categories to List<Section>
                if (dto.Categories != null && dto.Categories.Any())
                {
                    foreach (var categoryName in dto.Categories)
                    {
                        restaurant.Categories.Add(new Category
                        {
                            Name = categoryName,
                            RestaurantId = restaurant.Id,
                            Restaurant = restaurant
                        });
                    }
                }

                await _context.Restaurants.AddAsync(restaurant);
                await _context.SaveChangesAsync();

                var restaurantDto = new RestaurantDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    LogoUrl = restaurant.LogoUrl,
                    CoverImageUrl = restaurant.CoverImageUrl,
                    Categories = restaurant.Categories.Select(s => s.Name).ToList(),
                    Address = restaurant.Address,
                    PhoneNumber = restaurant.PhoneNumber,
                    Email = restaurant.Email,
                    OpeningHours = restaurant.OpeningHours,
                    Rating = restaurant.Rating,
                    ReviewCount = restaurant.ReviewCount,
                    IsOpen = restaurant.IsOpen,
                    DeliveryFee = restaurant.DeliveryFee,
                    Suspended = restaurant.IsSupended
                };

                return new AppResponse<RestaurantDto>(restaurantDto, "Restaurant created successfully", 201, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<RestaurantDto>(null, ex.Message, 500, false);
            }
        }

        [HttpGet("getAllRestaurants")]
        public async Task<AppResponse<List<RestaurantDto>>> GetAllRestaurants()
        {
            try
            {
                var restaurants = await _context.Restaurants
                    .Include(r => r.Categories)
                    .ToListAsync();

                if (restaurants == null || !restaurants.Any())
                {
                    return new AppResponse<List<RestaurantDto>>(null, "No restaurants found", 404, false);
                }

                var restaurantDtos = restaurants.Select(restaurant => new RestaurantDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    LogoUrl = restaurant.LogoUrl,
                    CoverImageUrl = restaurant.CoverImageUrl,
                    Categories = restaurant.Categories.Select(c => c.Name).ToList(),
                    Address = restaurant.Address,
                    PhoneNumber = restaurant.PhoneNumber,
                    Email = restaurant.Email,
                    OpeningHours = restaurant.OpeningHours,
                    Rating = restaurant.Rating,
                    ReviewCount = restaurant.ReviewCount,
                    IsOpen = restaurant.IsOpen,
                    DeliveryFee = restaurant.DeliveryFee,
                    Suspended = restaurant.IsSupended
                }).ToList();

                return new AppResponse<List<RestaurantDto>>(restaurantDtos, "Restaurants retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<RestaurantDto>>(null, ex.Message, 500, false);
            }

        }

        [HttpGet("getRestaurant/{id}")]
        public async Task<AppResponse<RestaurantDto>> GetRestaurant(int id)
        {
            try
            {
                var restaurant = await _context.Restaurants
                    .Include(r => r.Categories)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurant == null)
                {
                    return new AppResponse<RestaurantDto>(null, "Restaurant not found", 404, false);
                }

                var restaurantDto = new RestaurantDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    LogoUrl = restaurant.LogoUrl,
                    CoverImageUrl = restaurant.CoverImageUrl,
                    Categories = restaurant.Categories.Select(c => c.Name).ToList(),
                    Address = restaurant.Address,
                    PhoneNumber = restaurant.PhoneNumber,
                    Email = restaurant.Email,
                    OpeningHours = restaurant.OpeningHours,
                    Rating = restaurant.Rating,
                    ReviewCount = restaurant.ReviewCount,
                    IsOpen = restaurant.IsOpen,
                    DeliveryFee = restaurant.DeliveryFee,
                    Suspended = restaurant.IsSupended
                };

                return new AppResponse<RestaurantDto>(restaurantDto, "Restaurant retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<RestaurantDto>(null, ex.Message, 500, false);
            }
        }

        [HttpPut("updateRestaurant/{id}")]
        public async Task<AppResponse<RestaurantDto>> UpdateRestaurant(int id, [FromBody] RestaurantUpdateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return new AppResponse<RestaurantDto>(null, "Invalid data", 400, false);
                }
                var restaurant = await _context.Restaurants
                    .Include(r => r.Categories)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (restaurant == null)
                {
                    return new AppResponse<RestaurantDto>(null, "Restaurant not found", 404, false);
                }

                restaurant.Name = dto.Name ?? restaurant.Name;
                restaurant.Description = dto.Description ?? restaurant.Description;
                restaurant.LogoUrl = dto.LogoUrl ?? restaurant.LogoUrl;
                restaurant.CoverImageUrl = dto.CoverImageUrl ?? restaurant.CoverImageUrl;
                restaurant.Address = dto.Address ?? restaurant.Address;
                restaurant.PhoneNumber = dto.PhoneNumber ?? restaurant.PhoneNumber;
                restaurant.Email = dto.Email ?? restaurant.Email;
                restaurant.OpeningHours = dto.OpeningHours ?? restaurant.OpeningHours;
                restaurant.IsSupended = dto.Issuspended;

                // Handle categories update
                if (dto.Categories != null)
                {
                    // Clear existing categories
                    restaurant.Categories.Clear();

                    // Add new categories
                    foreach (var categoryName in dto.Categories)
                    {
                        restaurant.Categories.Add(new Category
                        {
                            Name = categoryName,
                            RestaurantId = restaurant.Id,
                            Restaurant = restaurant
                        });
                    }
                }

                _context.Restaurants.Update(restaurant);
                await _context.SaveChangesAsync();

                var restaurantDto = new RestaurantDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    LogoUrl = restaurant.LogoUrl,
                    CoverImageUrl = restaurant.CoverImageUrl,
                    Categories = restaurant.Categories.Select(c => c.Name).ToList(),
                    Address = restaurant.Address,
                    PhoneNumber = restaurant.PhoneNumber,
                    Email = restaurant.Email,
                    OpeningHours = restaurant.OpeningHours,
                    Rating = restaurant.Rating,
                    ReviewCount = restaurant.ReviewCount,
                    IsOpen = restaurant.IsOpen,
                    DeliveryFee = restaurant.DeliveryFee,
                    Suspended = restaurant.IsSupended
                };

                return new AppResponse<RestaurantDto>(restaurantDto, "Restaurant updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<RestaurantDto>(null, ex.Message, 500, false);
            }
        }

        [HttpDelete("deleteRestaurant/{id}")]
        public async Task<AppResponse<bool>> DeleteRestaurant(int id)
        {

            try
            {
                var restaurant = await _context.Restaurants.FindAsync(id);

                if (restaurant == null)
                {
                    return new AppResponse<bool>(false, "Restaurant not found", 404, false);
                }

                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();

                return new AppResponse<bool>(true, "Restaurant deleted successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }

        }

        [HttpPut("suspendRestaurant/{id}")]
        public async Task<AppResponse<bool>> SuspendRestaurant(int id)
        {
            try
            {
                var restaurant = await _context.Restaurants.FindAsync(id);

                if (restaurant == null)
                {
                    return new AppResponse<bool>(false, "Restaurant not found", 404, false);
                }

                restaurant.IsSupended = true;
                _context.Restaurants.Update(restaurant);
                await _context.SaveChangesAsync();

                return new AppResponse<bool>(true, "Restaurant suspended successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }
        }

        [HttpPut("unsuspendRestaurant/{id}")]
        public async Task<AppResponse<bool>> UnsuspendRestaurant(int id)
        {
            try
            {
                var restaurant = await _context.Restaurants.FindAsync(id);

                if (restaurant == null)
                {
                    return new AppResponse<bool>(false, "Restaurant not found", 404, false);
                }

                restaurant.IsSupended = false;
                _context.Restaurants.Update(restaurant);
                await _context.SaveChangesAsync();

                return new AppResponse<bool>(true, "Restaurant unsuspended successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }
        }

        [HttpGet("getAllOrders")]
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
                    EmployeeId = order.EmployeeId,
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

        [HttpPost("createOrder")]
        public async Task<AppResponse<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                // Validate input
                if (dto == null)
                {
                    return new AppResponse<OrderDto>(null, "Invalid order data", 400, false);
                }

                // Check if user exists
                var user = await _context.Users.FindAsync(dto.UserId);
                if (user == null)
                {
                    return new AppResponse<OrderDto>(null, "User not found", 404, false);
                }

                // Check if restaurant exists
                var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
                if (restaurant == null)
                {
                    return new AppResponse<OrderDto>(null, "Restaurant not found", 404, false);
                }

                // Initialize order with calculated total amount
                decimal calculatedTotal = 0;
                var orderItems = new List<OrderItem>();

                // Calculate total from menu items
                if (dto.OrderItems != null && dto.OrderItems.Any())
                {
                    foreach (var itemDto in dto.OrderItems)
                    {
                        var menuItem = await _context.OrderItems.FindAsync(itemDto.DishId);
                        if (menuItem == null)
                        {
                            return new AppResponse<OrderDto>(null, $"Menu item with ID {itemDto.DishId} not found", 404, false);
                        }

                        // Use the price from database for security
                        decimal itemPrice = menuItem.DishId;
                        calculatedTotal += itemPrice * itemDto.Quantity;

                        var orderItem = new OrderItem
                        {
                            DishId = itemDto.DishId,
                            Quantity = itemDto.Quantity,
                            UnitPrice = itemPrice,
                            TotalPrice = itemPrice * itemDto.Quantity
                                
                        };
                        orderItems.Add(orderItem);
                    }
                }

                // Create new order with calculated total
                var order = new Order
                {
                    TotalAmount = calculatedTotal,
                    PaymentMethod = dto.PaymentMethod,
                    Status = "Pending",
                    UserId = dto.UserId,
                    RestaurantId = dto.RestaurantId,
                    EmployeeId = dto.EmployeeId ?? 0,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = orderItems
                };

                // Save to database
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                // Return response with OrderDto format matching GetAllOrders
                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    UserId = order.UserId,
                    RestaurantId = order.RestaurantId,
                    EmployeeId = order.EmployeeId,
                    UserName = user.UserName,
                    RestaurantName = restaurant.Name
                };

                return new AppResponse<OrderDto>(orderDto, "Order created successfully", 201, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<OrderDto>(null, ex.Message, 500, false);
            }
        }
    }
}


