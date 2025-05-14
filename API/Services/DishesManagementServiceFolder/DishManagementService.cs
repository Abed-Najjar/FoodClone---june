using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;

public class DishManagementService : IDishManagementService
{
    private readonly AppDbContext _context;

    public DishManagementService(AppDbContext context)
    {
        _context = context; 
    }

    public async Task<AppResponse<DishDto>> CreateDish(CreateDishDto dishDto)
    {
        try
            {
                var restaurant = await _context.Restaurants.FindAsync(dishDto.RestaurantId);
                if (restaurant == null)
                {
                    return new AppResponse<DishDto>(null, "Restaurant not found", 404, false);
                }

                if (dishDto.CategoryId.HasValue)
                {
                    var category = await _context.Categories.FindAsync(dishDto.CategoryId.Value);
                    if (category == null)
                    {
                        return new AppResponse<DishDto>(null, "Category not found", 404, false);
                    }
                }

                var dish = new Dish
                {
                    Name = dishDto.Name,
                    Description = dishDto.Description,
                    Quantity = dishDto.Quantity,
                    Price = dishDto.Price,
                    ImageUrl = dishDto.ImageUrl,
                    RestaurantId = dishDto.RestaurantId,
                    Restaurant = restaurant,
                    CategoryId = dishDto.CategoryId
                };

                await _context.Dishes.AddAsync(dish);
                await _context.SaveChangesAsync();

                var responseDto = new DishDto
                {
                    Id = dish.Id,
                    Name = dish.Name,
                    Description = dish.Description,
                    Quantity = dish.Quantity,
                    Price = dish.Price,
                    ImageUrl = dish.ImageUrl,
                    RestaurantId = dish.RestaurantId,
                    RestaurantName = restaurant.Name,
                    CategoryId = dish.CategoryId,
                    CategoryName = dish.Category?.Name
                };

                return new AppResponse<DishDto>(responseDto, "Dish created successfully", 201, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<DishDto>(null, ex.Message, 500, false);
            }
    }


    public async Task<AppResponse<bool>> DeleteDish(int id)
    {
            try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                {
                    return new AppResponse<bool>(false, "Dish not found", 404, false);
                }

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();

                return new AppResponse<bool>(true, "Dish deleted successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }
    }


    public async Task<AppResponse<DishDto>> GetDishById(int id)
    {
        try
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null)
                {
                    return new AppResponse<DishDto>(null, "Dish not found", 404, false);
                }

                var dishDto = new DishDto
                {
                    Id = dish.Id,
                    Name = dish.Name,
                    Description = dish.Description,
                    Quantity = dish.Quantity,
                    Price = dish.Price,
                    ImageUrl = dish.ImageUrl,
                    RestaurantId = dish.RestaurantId,
                    RestaurantName = dish.Restaurant.Name, 
                    CategoryId = dish.CategoryId,
                    CategoryName = dish.Category?.Name
                };

                return new AppResponse<DishDto>(dishDto, "Dish retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<DishDto>(null, ex.Message, 500, false);
            }
    }
    
    public async Task<AppResponse<List<DishDto>>> GetDishes(int restaurantId)
    {
        try
            {
                var dishes = await _context.Dishes
                    .Include(d => d.Restaurant)
                    .ThenInclude(r => r.Categories)
                    .Include(d => d.Category)
                    .Where(d => d.RestaurantId == restaurantId)
                    .ToListAsync();

                if (!dishes.Any())
                {
                    return new AppResponse<List<DishDto>>(null, "No dishes found for this restaurant", 404, false);
                }

                var dishDtos = dishes.Select(d => new DishDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ImageUrl = d.ImageUrl,
                    RestaurantId = d.RestaurantId,
                    RestaurantName = d.Restaurant.Name,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category?.Name
                }).ToList();

                return new AppResponse<List<DishDto>>(dishDtos, "Dishes retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<DishDto>>(null, ex.Message, 500, false);
            }
    }

    public async Task<AppResponse<List<DishDto>>> GetDishesByCategory(int categoryId)
    {
        try
            {
                var dishes = await _context.Dishes
                    .Include(d => d.Restaurant)
                    .Include(d => d.Category)
                    .Where(d => d.CategoryId == categoryId)
                    .ToListAsync();

                if (!dishes.Any())
                {
                    return new AppResponse<List<DishDto>>(null, "No dishes found for this category", 404, false);
                }

                var dishDtos = dishes.Select(d => new DishDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ImageUrl = d.ImageUrl,
                    RestaurantId = d.RestaurantId,
                    RestaurantName = d.Restaurant.Name,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category?.Name
                }).ToList();

                return new AppResponse<List<DishDto>>(dishDtos, "Dishes retrieved successfully", 200, true);
        
            }
            catch (Exception ex)
            {
                return new AppResponse<List<DishDto>>(null, ex.Message, 500, false);
            }
    }


    public async Task<AppResponse<DishDto>> UpdateDish(int id, UpdateDishDto dishDto)
    {
        try
            {
                var dish = await _context.Dishes
                    .Include(d => d.Restaurant)
                    .Include(d => d.Category)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (dish == null)
                {
                    return new AppResponse<DishDto>(null, "Dish not found", 404, false);
                }

                if (dishDto.Name != null) dish.Name = dishDto.Name;
                if (dishDto.Description != null) dish.Description = dishDto.Description;
                if (dishDto.Quantity.HasValue) dish.Quantity = dishDto.Quantity.Value;
                if (dishDto.Price.HasValue) dish.Price = dishDto.Price.Value;
                if (dishDto.ImageUrl != null) dish.ImageUrl = dishDto.ImageUrl;
                if (dishDto.CategoryId.HasValue)
                {
                    var category = await _context.Categories.FindAsync(dishDto.CategoryId.Value);
                    if (category == null)
                    {
                        return new AppResponse<DishDto>(null, "Category not found", 404, false);
                    }
                    dish.CategoryId = dishDto.CategoryId;
                }

                await _context.SaveChangesAsync();

                var responseDto = new DishDto
                {
                    Id = dish.Id,
                    Name = dish.Name,
                    Description = dish.Description,
                    Quantity = dish.Quantity,
                    Price = dish.Price,
                    ImageUrl = dish.ImageUrl,
                    RestaurantId = dish.RestaurantId,
                    RestaurantName = dish.Restaurant.Name,
                    CategoryId = dish.CategoryId,
                    CategoryName = dish.Category?.Name
                };

                return new AppResponse<DishDto>(responseDto, "Dish updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<DishDto>(null, ex.Message, 500, false);
            }
    }
}

    
        
