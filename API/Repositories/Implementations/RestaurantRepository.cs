using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly AppDbContext _context;
    public RestaurantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
    {
        await _context.Restaurants.AddAsync(restaurant);
        return restaurant;
    }

    public async Task<Restaurant> DeleteRestaurantAsync(int id)
    {
        var restaurant = await _context.Restaurants
            .Include(r => r.Categories)
            .FirstOrDefaultAsync(r => r.Id == id);
        
        if (restaurant == null)
        {
            throw new Exception($"Restaurant with id {id} not found");
        }
        
        _context.Restaurants.Remove(restaurant);
        return restaurant;
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
    {
        return await _context.Restaurants.ToListAsync();
    }

    public async Task<List<Restaurant>> GetAllRestaurantsWithCategoriesAsync()
    {
        return await _context.Restaurants
            .Include(r => r.Categories)
            .ToListAsync();
    }

    public async Task<Restaurant> GetRestaurantByIdAsync(int id)
    {
        var result = await _context.Restaurants.FindAsync(id);
        if (result == null)
        {
            throw new Exception($"Restaurant with id {id} not found");
        }
        return result;
    }    public async Task<Restaurant?> GetRestaurantWithCategoriesAsync(int id)
    {
        return await _context.Restaurants
            .Include(r => r.Categories)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Restaurant?> GetRestaurantWithCategoriesAndDishesAsync(int id)
    {
        return await _context.Restaurants
            .Include(r => r.Categories)
            .ThenInclude(c => c.Dishes)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<int>> GetCategoryRestaurantsAsync(int categoryId)
    {
        return await _context.RestaurantsCategories
            .Where(rc => rc.CategoryId == categoryId)
            .Select(rc => rc.RestaurantId)
            .ToListAsync();
    }

    public async Task<Restaurant?> FindAsync(int id)
    {
        return await _context.Restaurants.FindAsync(id);
    }

    public Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
        return Task.FromResult(restaurant);
    }
}
