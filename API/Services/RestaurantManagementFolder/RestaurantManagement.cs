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

    public async Task<AppResponse<AdminRestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto)
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
                    var category = new Category
                    {
                        Name = categoryName
                    };
                    _context.Categories.Add(category);

                    // Fix the object initialization by including required properties
                    var restaurantCategory = new RestaurantsCategories
                    {
                        RestaurantId = restaurant.Id,
                        CategoryId = category.Id,
                        Restaurant = restaurant,
                        Category = category
                    };
                    _context.RestaurantsCategories.Add(restaurantCategory);
                }
            }

            await _context.Restaurants.AddAsync(restaurant);
            await _context.SaveChangesAsync();

            var AdminRestaurantDto = new AdminRestaurantDto
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

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant created successfully", 201, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDto>> DeleteRestaurant(int id)
    {
        try
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            // Create the DTO before removing the restaurant
            var AdminRestaurantDto = new AdminRestaurantDto
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

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant deleted successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<List<AdminRestaurantDto>>> GetAllRestaurants()
    {
        try
        {
            var restaurants = await _context.Restaurants
                .Include(r => r.Categories)
                .ToListAsync();

            if (restaurants == null || !restaurants.Any())
            {
                return new AppResponse<List<AdminRestaurantDto>>(null, "No restaurants found", 404, false);
            }

            var AdminRestaurantDtos = restaurants.Select(restaurant => new AdminRestaurantDto
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

            return new AppResponse<List<AdminRestaurantDto>>(AdminRestaurantDtos, "Restaurants retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<List<AdminRestaurantDto>>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDto>> GetRestaurant(int id)
    {
        try
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            var AdminRestaurantDto = new AdminRestaurantDto
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

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
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

    public async Task<AppResponse<AdminRestaurantDto>> UpdateRestaurant(int id, RestaurantUpdateDto dto)
    {
        try
        {
            if (dto == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Invalid data", 400, false);
            }
            var restaurant = await _context.Restaurants
                .Include(r => r.Categories)
                .ThenInclude(c => c.Dishes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
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

            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();

            var AdminRestaurantDto = new AdminRestaurantDto
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

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant updated successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<List<int>>> GetCategoryRestaurants(int categoryId)
    {
        try
        {
            // Replace direct Category.RestaurantId access
            var categoryRestaurants = await _context.RestaurantsCategories
                .Where(rc => rc.CategoryId == categoryId)
                .Select(rc => rc.RestaurantId)
                .ToListAsync();

            if (categoryRestaurants == null || !categoryRestaurants.Any())
            {
                return new AppResponse<List<int>>(null, "No restaurants found for the category", 404, false);
            }

            return new AppResponse<List<int>>(categoryRestaurants, "Category restaurants retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<List<int>>(null, ex.Message, 500, false);
        }
    }
    
    public async Task<AppResponse<AdminRestaurantDto>> OpenRestaurant(int id)
    {
        try
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            restaurant.IsOpen = true;
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();

            return new AppResponse<AdminRestaurantDto>(new AdminRestaurantDto { Id = restaurant.Id }, "Restaurant opened successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDto>> CloseRestaurant(int id)
    {
        try
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            restaurant.IsOpen = false;
            _context.Restaurants.Update(restaurant);
            await _context.SaveChangesAsync();

            return new AppResponse<AdminRestaurantDto>(new AdminRestaurantDto { Id = restaurant.Id }, "Restaurant closed successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }
}

