using API.AppResponse;
using API.DTOs;

namespace API.Services.CartServiceFolder
{
    public interface ICartService
    {
        /// <summary>
        /// Calculate all cart totals securely on the backend
        /// </summary>
        /// <param name="request">Cart calculation request with items and restaurant info</param>
        /// <param name="userId">User ID for personalized calculations (optional)</param>
        /// <returns>Complete cart breakdown with subtotal, fees, tax, and total</returns>
        Task<AppResponse<CartCalculationResponseDto>> CalculateCartTotalsAsync(CartCalculationRequestDto request, int? userId = null);
        
        /// <summary>
        /// Validate promo code and return discount details
        /// </summary>
        /// <param name="promoCode">Promo code to validate</param>
        /// <param name="subtotal">Current subtotal for minimum order validation</param>
        /// <returns>Promo code details if valid</returns>
        Task<AppResponse<PromoCodeDto>> ValidatePromoCodeAsync(string promoCode, decimal subtotal);
        
        /// <summary>
        /// Get available promo codes for the user
        /// </summary>
        /// <param name="subtotal">Current subtotal to filter applicable promos</param>
        /// <returns>List of applicable promo codes</returns>
        Task<AppResponse<List<PromoCodeDto>>> GetAvailablePromoCodesAsync(decimal subtotal);
    }
} 