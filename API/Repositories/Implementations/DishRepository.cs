using System;
using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class DishRepository : IDishRepository
{
    private readonly AppDbContext _context;

    public DishRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Dish> AddDishAsync(Dish dish)
    {
        await _context.Dishes.AddAsync(dish);
        return dish;
    }

    public async Task<Dish> GetDishByIdAsync(int id)
    {
        var result = await _context.Dishes.FindAsync(id);
        if (result == null)
        {
            throw new Exception($"Dish with id {id} not found");
        }
        return result;
    }

    public async Task<List<Dish>> GetDisheshByRestaurantIdAsync(int restaurantId)
    {
        var result = await _context.Dishes
            .Include(d => d.Restaurant)
            .ThenInclude(r => r.Categories)
            .Include(d => d.Category)
            .Where(d => d.RestaurantId == restaurantId)
            .ToListAsync();

        if (result == null)
        {
            throw new Exception($"Dish with restaurant id {restaurantId} not found");
        }

        return result;
    }

    public async Task<List<Dish>> GetDishesByCategoryIdAsync(int categoryId)
    {
        var result = await _context.Dishes
                .Include(d => d.Restaurant)
                .Include(d => d.Category)
                .Where(d => d.CategoryId == categoryId)
                .ToListAsync();

        if (result == null)
        {
            throw new Exception($"Dish with category id {categoryId} not found");
        }

        return result;
    }

    public async Task<Dish> GetDishByIdWithIncludesAsync(int id)
    {
        var result = await _context.Dishes
                .Include(d => d.Restaurant)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

        if (result == null)
        {
            throw new Exception($"Dish with id {id} not found");
        }

        return result;
    }

    public async Task<Dish> UpdateDishAsync(Dish dish)
    {
        var existingDish = await _context.Dishes.FindAsync(dish.Id);
        if (existingDish == null)
        {
            throw new Exception($"Dish with id {dish.Id} not found");
        }

        existingDish.Name = dish.Name;
        existingDish.Description = dish.Description;
        existingDish.Price = dish.Price;
        existingDish.ImageUrl = dish.ImageUrl;
        existingDish.RestaurantId = dish.RestaurantId;
        existingDish.CategoryId = dish.CategoryId;

        return existingDish;
    }

    public async Task<Dish> DeleteDishAsync(int id)
    {
        var dish = await _context.Dishes.FindAsync(id);
        if (dish == null)
        {
            throw new Exception($"Dish with id {id} not found");
        }

        _context.Dishes.Remove(dish);
        return dish;
    }
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
