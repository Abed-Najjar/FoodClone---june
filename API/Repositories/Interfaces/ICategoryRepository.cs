using System;
using API.Models;

namespace API.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<Category> GetCategoryByIdAsync(int id);
    Task<List<Category>> GetAllAsync(); // Added
    Task<List<Category>> GetCategoriesByRestaurantAsync(int restaurantId); // Added from CategoryManagementController
    Task<Category> CreateCategoryAsync(Category category); // Added from CategoryManagementController
    Task<Category> UpdateCategoryAsync(Category category); // Added from CategoryManagementController
    Task<bool> DeleteCategoryAsync(int id); // Added from CategoryManagementController
 
}
