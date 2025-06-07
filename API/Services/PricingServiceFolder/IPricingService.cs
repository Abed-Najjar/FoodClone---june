using API.Models;

namespace API.Services.PricingServiceFolder
{
    public interface IPricingService
    {
        /// <summary>
        /// Calculate complete pricing breakdown for items from a restaurant
        /// </summary>
        Task<PricingCalculationResult> CalculateOrderTotalsAsync(
            List<PricingItemDto> items, 
            int restaurantId, 
            string? promoCode = null,
            int? deliveryAddressId = null,
            int? userId = null
        );



        /// <summary>
        /// Calculate delivery fee based on subtotal and restaurant settings
        /// </summary>
        Task<decimal> CalculateDeliveryFeeAsync(decimal subtotal, int restaurantId);
    }
} 