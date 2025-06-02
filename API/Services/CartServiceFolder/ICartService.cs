using API.AppResponse;
using API.DTOs;

namespace API.Services.CartServiceFolder
{
    public interface ICartService
    {
        // Complete cart breakdown with subtotal, fees, tax, and total
        Task<AppResponse<CartCalculationResponseDto>> CalculateCartTotalsAsync(CartCalculationRequestDto request, int? userId = null);
        
        // Promo code details if valid
        Task<AppResponse<PromoCodeDto>> ValidatePromoCodeAsync(string promoCode, decimal subtotal);
        
        // List of applicable promo codes
        Task<AppResponse<List<PromoCodeDto>>> GetAvailablePromoCodesAsync(decimal subtotal);
    }
} 