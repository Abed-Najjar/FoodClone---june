using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId)
        {
            return await _context.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
        }

        public async Task<Dish?> GetDishByIdAsync(int dishId)
        {
            return await _context.Dishes
                .Include(d => d.Restaurant)
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == dishId);
        }

        public async Task<List<Dish>> GetDishesByIdsAsync(List<int> dishIds)
        {
            return await _context.Dishes
                .Include(d => d.Restaurant)
                .Include(d => d.Category)
                .Where(d => dishIds.Contains(d.Id))
                .ToListAsync();
        }

        public async Task<Address?> GetAddressByIdAsync(int addressId, int userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        }

        public async Task<bool> ValidateRestaurantIsOpenAsync(int restaurantId)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
            
            return restaurant?.IsOpen ?? false;
        }

        public async Task<List<Dish>> GetAvailableDishesByRestaurantAsync(int restaurantId)
        {
            return await _context.Dishes
                .Include(d => d.Category)
                .Where(d => d.RestaurantId == restaurantId && d.IsAvailable)
                .ToListAsync();
        }

        public async Task<decimal> GetRestaurantDeliveryFeeAsync(int restaurantId)
        {
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
            
            return restaurant?.DeliveryFee ?? 3.99m; // Default delivery fee
        }

        public async Task<bool> ValidateDishAvailabilityAsync(int dishId)
        {
            var dish = await _context.Dishes
                .FirstOrDefaultAsync(d => d.Id == dishId);
            
            return dish?.IsAvailable ?? false;
        }

        public async Task<List<PromoCode>> GetActivePromoCodesAsync()
        {
            return await _context.PromoCodes
                .Where(p => p.IsActive && 
                           (p.ExpiryDate == null || p.ExpiryDate > DateTime.UtcNow) &&
                           (p.UsageLimit == null || p.TimesUsed < p.UsageLimit))
                .ToListAsync();
        }

        public async Task<PromoCode?> GetPromoCodeByCodeAsync(string code)
        {
            return await _context.PromoCodes
                .Include(p => p.Restaurant)
                .FirstOrDefaultAsync(p => p.Code.ToLower() == code.ToLower() && 
                                         p.IsActive &&
                                         (p.ExpiryDate == null || p.ExpiryDate > DateTime.UtcNow) &&
                                         (p.UsageLimit == null || p.TimesUsed < p.UsageLimit));
        }
    }
} 