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
                    Dishes = [.. c.Dishes.Select(d => new DishDto
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
                    })]
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
                var categories = await _context.Categories
                    .Include(c => c.Restaurant)
                    .Where(c => c.RestaurantId == restaurantId)
                    .ToListAsync();

                if (!categories.Any())
                {
                    return new AppResponse<CategoryDto>(null, "No categories found for this restaurant", 404, false);
                }

                var categoryDto = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    RestaurantId = c.RestaurantId,
                    RestaurantName = c.Restaurant.Name
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

                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    RestaurantId = category.RestaurantId,
                    RestaurantName = category.Restaurant.Name
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

}
