using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.EntityFrameworkCore;

public class CategoryManagementService : ICategoryManagementService
{
    private readonly AppDbContext _context;

    public CategoryManagementService(AppDbContext context)
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
                ImageUrl = categoryDto.ImageUrl
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            
            var restaurantCategory = new RestaurantsCategories
            {
                RestaurantId = categoryDto.RestaurantId,
                CategoryId = category.Id,
                Restaurant = restaurant,
                Category = category
            };

            await _context.RestaurantsCategories.AddAsync(restaurantCategory);
            await _context.SaveChangesAsync();

            var responseDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                RestaurantId = restaurant.Id,
                RestaurantName = restaurant.Name
            };

            return new AppResponse<CategoryDto>(responseDto, "Category created successfully", 201, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
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

            var restaurantCategories = await _context.RestaurantsCategories
                .Where(rc => rc.CategoryId == category.Id)
                .ToListAsync();
            
            if (restaurantCategories.Any())
            {
                _context.RestaurantsCategories.RemoveRange(restaurantCategories);
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

    public async Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId)
    {
        try
        {
            var categories = await _context.RestaurantsCategories
                .Include(rc => rc.Category)
                .ThenInclude(c => c.Dishes)
                .Include(rc => rc.Restaurant)
                .Where(rc => rc.RestaurantId == restaurantId)
                .ToListAsync();

            if (!categories.Any())
            {
                return new AppResponse<List<CategoryDto>>(null, "No categories found for this restaurant", 404, false);
            }

            var categoryDtos = categories.Select(rc => new CategoryDto
            {
                Id = rc.Category.Id,
                Name = rc.Category.Name,
                Description = rc.Category.Description,
                ImageUrl = rc.Category.ImageUrl,
                RestaurantId = rc.Restaurant.Id,
                RestaurantName = rc.Restaurant.Name,
                Dishes = rc.Category.Dishes.Select(d => new DishDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ImageUrl = d.ImageUrl,
                    RestaurantId = d.RestaurantId,
                    RestaurantName = d.Restaurant?.Name ?? string.Empty,
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

    public async Task<AppResponse<CategoryDto>> GetCategoriesByRestaurantId(int restaurantId)
    {
        try
        {
            var categories = await _context.RestaurantsCategories
                .Include(rc => rc.Category)
                .Include(rc => rc.Restaurant)
                .Where(rc => rc.RestaurantId == restaurantId)
                .ToListAsync();

            if (!categories.Any())
            {
                return new AppResponse<CategoryDto>(null, "No categories found for this restaurant", 404, false);
            }

            var categoryDto = categories.Select(rc => new CategoryDto
            {
                Id = rc.Category.Id,
                Name = rc.Category.Name,
                Description = rc.Category.Description,
                ImageUrl = rc.Category.ImageUrl,
                RestaurantId = rc.Restaurant.Id,
                RestaurantName = rc.Restaurant.Name
            }).ToList();

            return new AppResponse<CategoryDto>(categoryDto.FirstOrDefault(), "Categories retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<CategoryDto>> GetCategoryById(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return new AppResponse<CategoryDto>(null, "Category not found", 404, false);
            }

            var restaurantCategory = await _context.RestaurantsCategories
                .Include(rc => rc.Restaurant)
                .FirstOrDefaultAsync(rc => rc.CategoryId == category.Id);

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                RestaurantId = restaurantCategory?.Restaurant?.Id ?? 0,
                RestaurantName = restaurantCategory?.Restaurant?.Name ?? string.Empty
            };

            return new AppResponse<CategoryDto>(categoryDto, "Category retrieved successfully", 200, true); 
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto categoryDto)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return new AppResponse<CategoryDto>(null, "Category not found", 404, false);
            }

            if (categoryDto.Name != null) category.Name = categoryDto.Name;
            if (categoryDto.Description != null) category.Description = categoryDto.Description;
            if (categoryDto.ImageUrl != null) category.ImageUrl = categoryDto.ImageUrl;

            var restaurantCategory = await _context.RestaurantsCategories
                .Include(rc => rc.Restaurant)
                .FirstOrDefaultAsync(rc => rc.CategoryId == category.Id);

            await _context.SaveChangesAsync();

            var responseDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                RestaurantId = restaurantCategory?.Restaurant?.Id ?? 0,
                RestaurantName = restaurantCategory?.Restaurant?.Name ?? string.Empty
            };

            return new AppResponse<CategoryDto>(responseDto, "Category updated successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
        }
    }
}
