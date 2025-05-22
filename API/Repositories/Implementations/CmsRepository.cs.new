using System;
using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations;

public class CmsRepository : ICmsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<CmsRepository> _logger;
    
    public CmsRepository(AppDbContext context, ILogger<CmsRepository> logger)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category?> UpdateCategoryAsync(Category category)
    {
        var existingCategory = await _context.Categories.FindAsync(category.Id);
        if (existingCategory == null) return null;
        
        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;
        existingCategory.ImageUrl = category.ImageUrl;

        _context.Categories.Update(existingCategory);
        await _context.SaveChangesAsync();
        return existingCategory;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Restaurant>> GetAllRestaurantsAsync()
    {
        return await _context.Restaurants.ToListAsync();
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
    {
        return await _context.Restaurants.FindAsync(id);
    }

    public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
    {
        await _context.Restaurants.AddAsync(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }

    public async Task<Restaurant?> UpdateRestaurantAsync(Restaurant restaurant)
    {
        var existingRestaurant = await _context.Restaurants.FindAsync(restaurant.Id);
        if (existingRestaurant == null) return null;

        existingRestaurant.Name = restaurant.Name;
        existingRestaurant.Description = restaurant.Description;
        existingRestaurant.LogoUrl = restaurant.LogoUrl;
        existingRestaurant.CoverImageUrl = restaurant.CoverImageUrl;
        existingRestaurant.Address = restaurant.Address;
        existingRestaurant.PhoneNumber = restaurant.PhoneNumber;
        existingRestaurant.Email = restaurant.Email;

        _context.Restaurants.Update(existingRestaurant);
        await _context.SaveChangesAsync();
        return existingRestaurant;
    }

    public async Task<bool> DeleteRestaurantAsync(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null) return false;

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Dish>> GetAllDishesAsync()
    {
        return await _context.Dishes
            .Include(d => d.Restaurant)
            .Include(d => d.Category)
            .ToListAsync();
    }

    public async Task<Dish?> GetDishByIdAsync(int id)
    {
        return await _context.Dishes
            .Include(d => d.Restaurant)
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Dish> CreateDishAsync(Dish dish)
    {
        await _context.Dishes.AddAsync(dish);
        await _context.SaveChangesAsync();
        return dish;
    }

    public async Task<Dish?> UpdateDishAsync(Dish dish)
    {
        var existingDish = await _context.Dishes.FindAsync(dish.Id);
        if (existingDish == null) return null;

        existingDish.Name = dish.Name;
        existingDish.Description = dish.Description;
        existingDish.Price = dish.Price;
        existingDish.ImageUrl = dish.ImageUrl;
        existingDish.CategoryId = dish.CategoryId;

        _context.Dishes.Update(existingDish);
        await _context.SaveChangesAsync();
        return await GetDishByIdAsync(existingDish.Id);
    }

    public async Task<bool> DeleteDishAsync(int id)
    {
        var dish = await _context.Dishes.FindAsync(id);
        if (dish == null) return false;

        _context.Dishes.Remove(dish);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return null;

        order.Status = status;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return await GetOrderByIdAsync(id);
    }

    public async Task<List<Restaurant>> GetRestaurantsByCategoryAsync(int categoryId)
    {
        return await _context.RestaurantsCategories
            .Where(rc => rc.CategoryId == categoryId)
            .Include(rc => rc.Restaurant)
            .Select(rc => rc.Restaurant)
            .ToListAsync();
    }

    public async Task<List<Dish>> GetDishesByRestaurantAsync(int restaurantId)
    {
        return await _context.Dishes
            .Where(d => d.RestaurantId == restaurantId)
            .Include(d => d.Restaurant)
            .Include(d => d.Category)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByRestaurantAsync(int restaurantId)
    {
        return await _context.Orders
            .Where(o => o.RestaurantId == restaurantId)
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByUserAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
            .ToListAsync();
    }

    public async Task<List<Order>> GetOrdersByEmployeeAsync(int employeeId)
    {
        return await _context.Orders
            .Where(o => o.EmployeeId == employeeId)
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Dish)
            .ToListAsync();
    }
}
