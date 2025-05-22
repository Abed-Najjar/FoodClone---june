using System;
using API.Models;

namespace API.Repositories.Interfaces;

public interface ICmsRepository
{
    // Methods for Categories
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category?> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);

    // Methods for Restaurants
    Task<List<Restaurant>> GetAllRestaurantsAsync();
    Task<Restaurant?> GetRestaurantByIdAsync(int id);
    Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);
    Task<Restaurant?> UpdateRestaurantAsync(Restaurant restaurant);
    Task<bool> DeleteRestaurantAsync(int id);    // Methods for Dishes
    Task<List<Dish>> GetAllDishesAsync();
    Task<Dish?> GetDishByIdAsync(int id);
    Task<Dish> CreateDishAsync(Dish dish);
    Task<Dish?> UpdateDishAsync(Dish dish);
    Task<bool> DeleteDishAsync(int id);

    // Methods for Users (if needed in CMS)
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> DeleteUserAsync(int id);

    // Methods for Orders (if needed in CMS)
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order?> UpdateOrderStatusAsync(int id, string status);

    // Additional methods for complex queries
    Task<List<Restaurant>> GetRestaurantsByCategoryAsync(int categoryId);
    Task<List<Dish>> GetDishesByRestaurantAsync(int restaurantId);
    Task<List<Order>> GetOrdersByRestaurantAsync(int restaurantId);
    Task<List<Order>> GetOrdersByUserAsync(int userId);
    Task<List<Order>> GetOrdersByEmployeeAsync(int employeeId);
}
