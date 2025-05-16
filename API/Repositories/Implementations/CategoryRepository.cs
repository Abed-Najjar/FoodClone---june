using System;
using API.Data;
using API.Models;
using API.Repositories.Interfaces;

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
}
