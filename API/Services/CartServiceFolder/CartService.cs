using API.AppResponse;
using API.DTOs;
using API.Models;
using API.UoW;
using API.Services.PricingServiceFolder;
using Microsoft.Extensions.Logging;

namespace API.Services.CartServiceFolder
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;
        private readonly IPricingService _pricingService;

        public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger, IPricingService pricingService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _pricingService = pricingService;
        }

        public async Task<AppResponse<CartCalculationResponseDto>> CalculateCartTotalsAsync(CartCalculationRequestDto request, int? userId = null)
        {
            try
            {
                _logger.LogInformation($"CalculateCartTotalsAsync called for user ID: {userId}");
                _logger.LogInformation($"Request: RestaurantId={request.RestaurantId}, CartItems={request.CartItems.Count}, PromoCode={request.PromoCode}");

                // Convert cart items to pricing items format
                var pricingItems = request.CartItems.Select(item => new PricingItemDto
                {
                    DishId = item.DishId,
                    Quantity = item.Quantity
                }).ToList();

                // Use the centralized pricing service
                var pricingResponse = await _pricingService.CalculateOrderTotalsAsync(
                    pricingItems,
                    request.RestaurantId,
                    request.PromoCode,
                    request.DeliveryAddressId,
                    userId
                );

                // Handle pricing service errors
                if (!pricingResponse.Success)
                {
                    _logger.LogWarning($"Pricing calculation failed: {pricingResponse.ErrorMessage}");
                    return new AppResponse<CartCalculationResponseDto>(null, pricingResponse.ErrorMessage, pricingResponse.StatusCode, false);
                }

                var pricingResult = pricingResponse.Data;

                // Convert pricing result to cart response format
                var itemDetails = pricingResult.ItemDetails.Select(item => new CartItemDetailDto
                {
                    DishId = item.DishId,
                    DishName = item.DishName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.UnitPrice * item.Quantity,
                    IsAvailable = item.IsAvailable
                }).ToList();

                var response = new CartCalculationResponseDto
                {
                    Subtotal = pricingResult.Subtotal,
                    DeliveryFee = pricingResult.DeliveryFee,
                    TaxAmount = pricingResult.TaxAmount,
                    TaxRate = pricingResult.TaxRate,
                    DiscountAmount = pricingResult.DiscountAmount,
                    GrandTotal = pricingResult.GrandTotal,
                    FreeDeliveryApplied = pricingResult.FreeDeliveryApplied,
                    PromoCodeApplied = pricingResult.PromoCodeApplied,
                    PromoCodeMessage = pricingResult.PromoCodeMessage,
                    ItemDetails = itemDetails,
                    Currency = pricingResult.Currency
                };

                _logger.LogInformation($"Cart calculation completed successfully - Total: {response.GrandTotal:F2} {response.Currency}");
                return new AppResponse<CartCalculationResponseDto>(response, "Cart totals calculated successfully.", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while calculating cart totals for user ID: {userId}");
                return new AppResponse<CartCalculationResponseDto>(null, $"Internal server error: {ex.Message}", 500, false);
            }
        }

        public async Task<AppResponse<PromoCodeDto>> ValidatePromoCodeAsync(string promoCode, decimal subtotal)
        {
            try
            {
                _logger.LogInformation($"Validating promo code: {promoCode} for subtotal: {subtotal:F2} JOD");

                // Get promo code from database
                var dbPromoCode = await _unitOfWork.CartRepository.GetPromoCodeByCodeAsync(promoCode);
                if (dbPromoCode == null)
                {
                    _logger.LogWarning($"Invalid promo code: {promoCode}");
                    return new AppResponse<PromoCodeDto>(null, "Invalid, expired, or requirements not met for this promo code.", 400, false);
                }

                // Check minimum order amount
                if (subtotal < dbPromoCode.MinimumOrderAmount)
                {
                    _logger.LogWarning($"Promo code {promoCode} requires minimum order of {dbPromoCode.MinimumOrderAmount:F2} JOD, current: {subtotal:F2} JOD");
                    return new AppResponse<PromoCodeDto>(null, $"This promo code requires a minimum order of {dbPromoCode.MinimumOrderAmount:F2} JOD.", 400, false);
                }

                // Convert to DTO
                var promoDto = new PromoCodeDto
                {
                    Code = dbPromoCode.Code,
                    Description = dbPromoCode.Description,
                    DiscountPercentage = dbPromoCode.DiscountPercentage,
                    DiscountAmount = dbPromoCode.DiscountAmount,
                    FreeDelivery = dbPromoCode.FreeDelivery,
                    MinimumOrderAmount = dbPromoCode.MinimumOrderAmount,
                    IsActive = dbPromoCode.IsActive,
                    ExpiryDate = dbPromoCode.ExpiryDate
                };

                _logger.LogInformation($"Promo code validated successfully: {promoDto.Description}");
                return new AppResponse<PromoCodeDto>(promoDto, "Promo code validated successfully.", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating promo code");
                return new AppResponse<PromoCodeDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<List<PromoCodeDto>>> GetAvailablePromoCodesAsync(decimal subtotal)
        {
            try
            {
                var dbPromoCodes = await _unitOfWork.CartRepository.GetActivePromoCodesAsync();
                var availableCodes = dbPromoCodes
                    .Where(p => subtotal >= p.MinimumOrderAmount)
                    .Select(p => new PromoCodeDto
                    {
                        Code = p.Code,
                        Description = p.Description,
                        DiscountPercentage = p.DiscountPercentage,
                        DiscountAmount = p.DiscountAmount,
                        FreeDelivery = p.FreeDelivery,
                        MinimumOrderAmount = p.MinimumOrderAmount,
                        IsActive = p.IsActive,
                        ExpiryDate = p.ExpiryDate
                    })
                    .ToList();

                return new AppResponse<List<PromoCodeDto>>(availableCodes, "Available promo codes retrieved successfully.", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting available promo codes");
                return new AppResponse<List<PromoCodeDto>>(null, ex.Message, 500, false);
            }
        }


    }
} 