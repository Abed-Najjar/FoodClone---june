using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IDishRepository
    {
        Task<Dish> AddDishAsync(Dish dish);
        Task<Dish> GetDishByIdAsync(int id);
        Task<List<Dish>> GetDisheshByRestaurantIdAsync(int restaurantId);
        Task<List<Dish>> GetDishesByCategoryIdAsync(int categoryId);
        Task<Dish> GetDishByIdWithIncludesAsync(int id);
        Task<List<Dish>> GetAllDishesWithIncludesAsync(); // Added method
        Task<Dish> UpdateDishAsync(Dish dish);
        Task<Dish> DeleteDishAsync(int id);
        Task<bool> SaveChangesAsync();
        


    }

    
}
