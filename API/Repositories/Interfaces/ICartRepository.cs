using API.Models;

namespace API.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId);
        Task<Dish?> GetDishByIdAsync(int dishId);
        Task<List<Dish>> GetDishesByIdsAsync(List<int> dishIds);
        Task<Address?> GetAddressByIdAsync(int addressId, int userId);
        Task<bool> ValidateRestaurantIsOpenAsync(int restaurantId);
        Task<List<Dish>> GetAvailableDishesByRestaurantAsync(int restaurantId);
        Task<decimal> GetRestaurantDeliveryFeeAsync(int restaurantId);
        Task<bool> ValidateDishAvailabilityAsync(int dishId);
        Task<List<PromoCode>> GetActivePromoCodesAsync();
        Task<PromoCode?> GetPromoCodeByCodeAsync(string code);
    }
} 