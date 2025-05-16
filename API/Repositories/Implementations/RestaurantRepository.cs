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
    public async Task<Restaurant> GetRestaurantByIdAsync(int id)
    {
        var result = await _context.Restaurants.FindAsync(id);
        if (result == null)
        {
            throw new Exception($"Restaurant with id {id} not found");
        }
        return result;
    }
}
