using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IDishRepository
    {
        Task<Dish> AddDishAsync(Dish dish);
        Task<Dish> GetDishByIdAsync(int id);
        Task<List<Dish>> GetDishesByRestaurantIdAsync(int restaurantId);
        Task<List<Dish>> GetDishesByCategoryIdAsync(int categoryId);
        Task<Dish> GetDishByIdWithIncludesAsync(int id);
        Task<List<Dish>> GetAllDishesWithIncludesAsync();
        Task<Dish> UpdateDishAsync(Dish dish);
        Task<Dish> DeleteDishAsync(int id);
        Task<Dish?> FindAsync(int id);
        Task<List<Dish>> GetAllDishesAsync();
    }
}
