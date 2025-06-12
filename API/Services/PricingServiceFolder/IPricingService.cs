using API.AppResponse;
using API.Models;

namespace API.Services.PricingServiceFolder
{
    public interface IPricingService
    {

        /// Calculate complete pricing breakdown for items from a restaurant
        Task<AppResponse<PricingCalculationResult>> CalculateOrderTotalsAsync(
            List<PricingItemDto> items, 
            int restaurantId, 
            string? promoCode = null,
            int? deliveryAddressId = null,
            int? userId = null
        );

        /// Calculate delivery fee based on subtotal and restaurant settings
        Task<AppResponse<decimal>> CalculateDeliveryFeeAsync(decimal subtotal, int restaurantId);
    }
} 