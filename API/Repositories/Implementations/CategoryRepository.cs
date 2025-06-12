using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        var result = await _context.Categories.FindAsync(id);
        if (result == null)
        {
            throw new Exception($"Category with id {id} not found");
        }
        return result;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories.Include(c => c.Dishes).ToListAsync();
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }    
    
    public async Task<List<Category>> GetCategoriesByRestaurantAsync(int restaurantId)
    {
        return await _context.RestaurantsCategories
            .Where(rc => rc.RestaurantId == restaurantId)
            .Include(rc => rc.Category)
            .ThenInclude(c => c.Dishes)
            .Select(rc => rc.Category)
            .ToListAsync();
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        return category;
    }    
    
    public Task<Category> UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        return Task.FromResult(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }
        _context.Categories.Remove(category);
        return true;
    }

    public async Task<Category?> FindAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<RestaurantsCategories> CreateRestaurantCategoryAsync(RestaurantsCategories restaurantCategory)
    {
        await _context.RestaurantsCategories.AddAsync(restaurantCategory);
        return restaurantCategory;
    }

    public async Task<List<RestaurantsCategories>> GetRestaurantCategoriesByCategoryIdAsync(int categoryId)
    {
        return await _context.RestaurantsCategories
            .Where(rc => rc.CategoryId == categoryId)
            .ToListAsync();
    }

    public Task DeleteRestaurantCategoriesAsync(List<RestaurantsCategories> restaurantCategories)
    {
        _context.RestaurantsCategories.RemoveRange(restaurantCategories);
        return Task.CompletedTask;
    }

    public async Task<Restaurant?> FindRestaurantAsync(int restaurantId)
    {
        return await _context.Restaurants.FindAsync(restaurantId);
    }
}
