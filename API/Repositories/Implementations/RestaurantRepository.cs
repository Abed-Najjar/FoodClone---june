using System;
using API.Data;
using API.Models;
using API.Repositories.Interfaces;

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
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null)
        {
            throw new Exception($"Restaurant with id {id} not found");
        }
        _context.Restaurants.Remove(restaurant);
        return restaurant;
        
    }

    public Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Restaurant> GetRestaurantByIdAsync(int id)
    {
        var result = await _context.Restaurants.FindAsync(id);
        if (result == null)
        {
            throw new Exception($"Restaurant with id {id} not found");
        }
        return result;
    }    public Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
        return Task.FromResult(restaurant);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
