using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using API.Services.Argon;
using API.Services.TokenServiceFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

class AdminService : IAdminService
{
    private readonly AppDbContext _context;
    private readonly IArgonHashing _argonHashing;
    private readonly ITokenService _tokenService;

    public AdminService(AppDbContext context, IArgonHashing argonHashing, ITokenService tokenService)
    {
        _context = context;
        _argonHashing = argonHashing;
        _tokenService = tokenService;
    }

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
                    Address = dto.Address,
                    PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password),
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

    public async Task<AppResponse<RestaurantDto>> DeleteRestaurant(int id)
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

                // Create the DTO before removing the restaurant
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

                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();

                return new AppResponse<RestaurantDto>(restaurantDto, "Restaurant deleted successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<RestaurantDto>(null, ex.Message, 500, false);
            }
    }

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

    public async Task<AppResponse<List<UserDto>>> GetAllUsers()
    {
        try
            {
                var users = await _context.Users
                .Where(u => u.Role != API.Enums.Roles.Admin)
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
                    .ThenInclude(c => c.Dishes)
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
                // First, remove dishes from categories that will be deleted
                foreach (var category in restaurant.Categories)
                {
                    if (!dto.Categories.Contains(category.Name))
                    {
                        // Remove category reference from dishes
                        foreach (var dish in category.Dishes)
                        {
                            dish.CategoryId = null;
                        }
                    }
                }

                // Now remove the categories
                _context.Categories.RemoveRange(restaurant.Categories);
                restaurant.Categories.Clear();

                // Add new categories
                foreach (var categoryName in dto.Categories)
                {
                    var category = new Category
                    {
                        Name = categoryName,
                        Description = $"Category for {categoryName}",
                        ImageUrl = "default-category.jpg",
                        RestaurantId = restaurant.Id
                    };
                    restaurant.Categories.Add(category);
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
                user.Address = dto.Address;
                user.Role = dto.Role != null ? (API.Enums.Roles)Enum.Parse(typeof(API.Enums.Roles), dto.Role) : user.Role;
                user.PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user),
                    Createdat = user.CreatedAt
                    
                };

                return new AppResponse<UserDto>(userDto, "User updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
    }
}
