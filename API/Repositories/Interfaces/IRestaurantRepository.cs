using API.Models;

namespace API.Repositories.Interfaces;

public interface IRestaurantRepository
{
    Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);
    Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant);
    Task<Restaurant> DeleteRestaurantAsync(int id);
    Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
    Task<Restaurant> GetRestaurantByIdAsync(int id);
    Task<bool> SaveChangesAsync();
}
