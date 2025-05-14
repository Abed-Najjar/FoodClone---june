using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class RestaurantManagement : IRestaurantManagement
{
    private readonly AppDbContext _context;

    public RestaurantManagement(AppDbContext context)
    {
        _context = context;
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

}

