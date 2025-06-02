using API.AppResponse;
using API.DTOs;
using API.UoW;
using Microsoft.Extensions.Logging;

namespace API.Services.CartServiceFolder
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;

        // Secure configuration constants - should be moved to appsettings in production
        private const decimal TAX_RATE = 0.085m; // 8.5% tax rate
        private const decimal FREE_DELIVERY_THRESHOLD = 50.00m; // Free delivery for orders over 50 JOD
        private const decimal REDUCED_DELIVERY_THRESHOLD = 30.00m; // Reduced delivery for orders over 30 JOD
        private const decimal REDUCED_DELIVERY_FEE = 1.99m;
        private const decimal STANDARD_DELIVERY_FEE = 2.99m;

        public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AppResponse<CartCalculationResponseDto>> CalculateCartTotalsAsync(CartCalculationRequestDto request, int? userId = null)
        {
            try
            {
                _logger.LogInformation($"CalculateCartTotalsAsync called for user ID: {userId}");
                _logger.LogInformation($"Request: RestaurantId={request.RestaurantId}, CartItems={request.CartItems.Count}, PromoCode={request.PromoCode}");

                // Validate restaurant exists and is open
                var restaurant = await _unitOfWork.CartRepository.GetRestaurantByIdAsync(request.RestaurantId);
                if (restaurant == null)
                {
                    _logger.LogWarning($"Restaurant not found with ID: {request.RestaurantId}");
                    return new AppResponse<CartCalculationResponseDto>(null, "Restaurant not found.", 404, false);
                }

                if (!restaurant.IsOpen)
                {
                    _logger.LogWarning($"Restaurant {restaurant.Name} is currently closed");
                    return new AppResponse<CartCalculationResponseDto>(null, "Restaurant is currently closed.", 400, false);
                }

                _logger.LogInformation($"Restaurant found: {restaurant.Name} (Open: {restaurant.IsOpen})");

                // Validate and calculate cart items securely
                var itemDetails = new List<CartItemDetailDto>();
                decimal subtotal = 0;

                foreach (var cartItem in request.CartItems)
                {
                    _logger.LogInformation($"Processing cart item: DishId={cartItem.DishId}, Quantity={cartItem.Quantity}");

                    // Fetch current dish price from database (prevent price manipulation)
                    var dish = await _unitOfWork.CartRepository.GetDishByIdAsync(cartItem.DishId);
                    if (dish == null)
                    {
                        _logger.LogWarning($"Dish not found with ID: {cartItem.DishId}");
                        return new AppResponse<CartCalculationResponseDto>(null, $"Dish with ID {cartItem.DishId} not found.", 404, false);
                    }

                    if (!dish.IsAvailable)
                    {
                        _logger.LogWarning($"Dish {dish.Name} is not available");
                        return new AppResponse<CartCalculationResponseDto>(null, $"Dish '{dish.Name}' is currently unavailable.", 400, false);
                    }

                    if (dish.RestaurantId != request.RestaurantId)
                    {
                        _logger.LogWarning($"Dish {dish.Name} does not belong to restaurant {request.RestaurantId}");
                        return new AppResponse<CartCalculationResponseDto>(null, $"Dish '{dish.Name}' does not belong to the selected restaurant.", 400, false);
                    }

                    // Validate quantity
                    if (cartItem.Quantity <= 0 || cartItem.Quantity > 10)
                    {
                        _logger.LogWarning($"Invalid quantity {cartItem.Quantity} for dish {dish.Name}");
                        return new AppResponse<CartCalculationResponseDto>(null, $"Invalid quantity for dish '{dish.Name}'. Must be between 1 and 10.", 400, false);
                    }

                    // Use current database price (security measure)
                    var itemTotal = dish.Price * cartItem.Quantity;
                    subtotal += itemTotal;

                    _logger.LogInformation($"Dish processed: {dish.Name}, Price={dish.Price:F2} JOD, Quantity={cartItem.Quantity}, Total={itemTotal:F2} JOD");

                    itemDetails.Add(new CartItemDetailDto
                    {
                        DishId = dish.Id,
                        DishName = dish.Name,
                        UnitPrice = dish.Price,
                        Quantity = cartItem.Quantity,
                        TotalPrice = itemTotal,
                        IsAvailable = dish.IsAvailable
                    });
                }

                _logger.LogInformation($"Subtotal calculated: {subtotal:F2} JOD");

                // Calculate delivery fee securely based on business rules
                decimal deliveryFee = CalculateDeliveryFee(subtotal, restaurant.DeliveryFee);
                _logger.LogInformation($"Delivery fee calculated: {deliveryFee:F2} JOD");

                // Calculate tax based on subtotal (before discounts)
                decimal taxAmount = Math.Round(subtotal * TAX_RATE, 2);
                _logger.LogInformation($"Tax amount calculated: {taxAmount:F2} JOD (Rate: {TAX_RATE:P})");

                // Handle promo code validation and discounts
                decimal discountAmount = 0;
                bool freeDeliveryApplied = false;
                string? promoCodeApplied = null;
                string? promoCodeMessage = null;

                if (!string.IsNullOrEmpty(request.PromoCode))
                {
                    _logger.LogInformation($"Processing promo code: {request.PromoCode}");
                    var promoResult = await ValidatePromoCodeAsync(request.PromoCode, subtotal);
                    if (promoResult.Success && promoResult.Data != null)
                    {
                        var promo = promoResult.Data;
                        promoCodeApplied = request.PromoCode.ToUpper();
                        promoCodeMessage = promo.Description;

                        // Apply free delivery
                        if (promo.FreeDelivery)
                        {
                            deliveryFee = 0;
                            freeDeliveryApplied = true;
                            _logger.LogInformation("Free delivery applied via promo code");
                        }

                        // Apply discount
                        if (promo.DiscountPercentage > 0)
                        {
                            discountAmount = Math.Round(subtotal * (promo.DiscountPercentage / 100), 2);
                            _logger.LogInformation($"Percentage discount applied: {promo.DiscountPercentage}%, Amount: {discountAmount:F2} JOD");
                        }
                        else if (promo.DiscountAmount > 0)
                        {
                            discountAmount = Math.Min(promo.DiscountAmount, subtotal); // Don't exceed subtotal
                            _logger.LogInformation($"Fixed discount applied: {discountAmount:F2} JOD");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Promo code validation failed: {promoResult.ErrorMessage}");
                    }
                }

                // Calculate grand total securely
                decimal grandTotal = Math.Round(subtotal + deliveryFee + taxAmount - discountAmount, 2);
                
                // Ensure total is never negative
                if (grandTotal < 0)
                {
                    grandTotal = 0;
                    _logger.LogWarning("Grand total was negative, adjusted to 0");
                }

                _logger.LogInformation($"Final calculation - Subtotal: {subtotal:F2}, Delivery: {deliveryFee:F2}, Tax: {taxAmount:F2}, Discount: {discountAmount:F2}, Total: {grandTotal:F2} JOD");

                var response = new CartCalculationResponseDto
                {
                    Subtotal = subtotal,
                    DeliveryFee = deliveryFee,
                    TaxAmount = taxAmount,
                    TaxRate = TAX_RATE,
                    DiscountAmount = discountAmount,
                    GrandTotal = grandTotal,
                    FreeDeliveryApplied = freeDeliveryApplied,
                    PromoCodeApplied = promoCodeApplied,
                    PromoCodeMessage = promoCodeMessage,
                    ItemDetails = itemDetails,
                    Currency = "JOD"
                };

                _logger.LogInformation("Cart calculation completed successfully");
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

        private decimal CalculateDeliveryFee(decimal subtotal, decimal restaurantDeliveryFee)
        {
            // Free delivery for orders over the threshold
            if (subtotal >= FREE_DELIVERY_THRESHOLD)
            {
                _logger.LogInformation($"Free delivery applied: subtotal {subtotal:F2} >= threshold {FREE_DELIVERY_THRESHOLD:F2}");
                return 0;
            }
            
            // Reduced delivery fee for orders over reduced threshold
            if (subtotal >= REDUCED_DELIVERY_THRESHOLD)
            {
                var reducedFee = Math.Min(REDUCED_DELIVERY_FEE, restaurantDeliveryFee);
                _logger.LogInformation($"Reduced delivery fee applied: {reducedFee:F2} JOD");
                return reducedFee;
            }
            
            // Use standard delivery fee (2.99 JOD)
            var standardFee = Math.Max(STANDARD_DELIVERY_FEE, restaurantDeliveryFee);
            _logger.LogInformation($"Standard delivery fee applied: {standardFee:F2} JOD");
            return standardFee;
        }
    }
} 