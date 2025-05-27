using System;
using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public async Task<List<Category>> GetCategoriesByRestaurantAsync(int restaurantId)
    {
        return await _context.RestaurantsCategories
            .Where(rc => rc.RestaurantId == restaurantId)
            .Select(rc => rc.Category)
            .Include(c => c.Dishes)
            .ToListAsync();
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return false;
        }
        _context.Categories.Remove(category);
        return await _context.SaveChangesAsync() > 0;
    }
}
