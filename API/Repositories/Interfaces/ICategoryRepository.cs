using System;
using API.Models;

namespace API.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<Category> GetCategoryByIdAsync(int id);
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetCategoriesByRestaurantAsync(int restaurantId);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);
    Task<Category?> FindAsync(int id);
    Task<List<Category>> GetAllCategoriesAsync();
    Task<RestaurantsCategories> CreateRestaurantCategoryAsync(RestaurantsCategories restaurantCategory);
    Task<List<RestaurantsCategories>> GetRestaurantCategoriesByCategoryIdAsync(int categoryId);
    Task DeleteRestaurantCategoriesAsync(List<RestaurantsCategories> restaurantCategories);
    Task<Restaurant?> FindRestaurantAsync(int restaurantId);
}
