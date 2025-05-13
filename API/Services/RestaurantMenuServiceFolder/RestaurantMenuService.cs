using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;


public class RestaurantMenuService : IRestaurantMenuService
{
    private readonly AppDbContext _context;

    public RestaurantMenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AppResponse<CategoryDto>> CreateCategory(CreateCategoryDto categoryDto)
    {
        try
            {
                var restaurant = await _context.Restaurants.FindAsync(categoryDto.RestaurantId);
                if (restaurant == null)
                {
                    return new AppResponse<CategoryDto>(null, "Restaurant not found", 404, false);
                }

                var category = new Category
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    ImageUrl = categoryDto.ImageUrl,
                    RestaurantId = categoryDto.RestaurantId
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                var responseDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    RestaurantId = category.RestaurantId,
                    RestaurantName = restaurant.Name
                };

                return new AppResponse<CategoryDto>(responseDto, "Category created successfully", 201, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
            }
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

    public async Task<AppResponse<bool>> DeleteCategory(int id)
    {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return new AppResponse<bool>(false, "Category not found", 404, false);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return new AppResponse<bool>(true, "Category deleted successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<bool>(false, ex.Message, 500, false);
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

    public async Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId)
    {
        try
            {
                var categories = await _context.Categories
                    .Include(c => c.Restaurant)
                    .Include(c => c.Dishes)
                    .Where(c => c.RestaurantId == restaurantId)
                    .ToListAsync();

                if (!categories.Any())
                {
                    return new AppResponse<List<CategoryDto>>(null, "No categories found for this restaurant", 404, false);
                }

                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    RestaurantId = c.RestaurantId,
                    RestaurantName = c.Restaurant.Name,
                    Dishes = c.Dishes.Select(d => new DishDto
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
                    }).ToList()
                }).ToList();

                return new AppResponse<List<CategoryDto>>(categoryDtos, "Categories retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<List<CategoryDto>>(null, ex.Message, 500, false);
            }
    }

    public Task<AppResponse<CategoryDto>> GetCategoryById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<AppResponse<CategoryDto>> GetCategoryByRestaurantId(int restaurantId)
    {
        throw new NotImplementedException();
    }

    public Task<AppResponse<DishDto>> GetDishById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<AppResponse<List<DishDto>>> GetDishes(int restaurantId)
    {
        try
            {
                var dishes = await _context.Dishes
                    .Include(d => d.Restaurant)
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

    public Task<AppResponse<List<DishDto>>> GetDishesByCategory(int categoryId)
    {
        throw new NotImplementedException();
    }

    public async Task<AppResponse<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto categoryDto)
    {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Restaurant)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return new AppResponse<CategoryDto>(null, "Category not found", 404, false);
                }

                if (categoryDto.Name != null) category.Name = categoryDto.Name;
                if (categoryDto.Description != null) category.Description = categoryDto.Description;
                if (categoryDto.ImageUrl != null) category.ImageUrl = categoryDto.ImageUrl;

                await _context.SaveChangesAsync();

                var responseDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    RestaurantId = category.RestaurantId,
                    RestaurantName = category.Restaurant.Name
                };

                return new AppResponse<CategoryDto>(responseDto, "Category updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
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