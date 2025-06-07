using API.Models;
using API.UoW;
using Microsoft.Extensions.Logging;

namespace API.Services.PricingServiceFolder
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PricingService> _logger;

        // Configuration constants - should be moved to appsettings in production
        private const decimal TAX_RATE = 0.085m; // 8.5% tax rate
        private const decimal FREE_DELIVERY_THRESHOLD = 50.00m; // Free delivery for orders over 50 JOD
        private const decimal REDUCED_DELIVERY_THRESHOLD = 30.00m; // Reduced delivery for orders over 30 JOD
        private const decimal REDUCED_DELIVERY_FEE = 1.99m;
        private const decimal STANDARD_DELIVERY_FEE = 2.99m;

        public PricingService(IUnitOfWork unitOfWork, ILogger<PricingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PricingCalculationResult> CalculateOrderTotalsAsync(
            List<PricingItemDto> items, 
            int restaurantId, 
            string? promoCode = null, 
            int? deliveryAddressId = null, 
            int? userId = null)
        {
            try
            {
                _logger.LogInformation($"CalculateOrderTotalsAsync called for restaurant {restaurantId} with {items.Count} items");

                var result = new PricingCalculationResult();

                // Validate restaurant exists and is open
                var restaurant = await _unitOfWork.CartRepository.GetRestaurantByIdAsync(restaurantId);
                if (restaurant == null)
                {
                    _logger.LogWarning($"Restaurant not found with ID: {restaurantId}");
                    result.IsValid = false;
                    result.ErrorMessage = "Restaurant not found.";
                    return result;
                }

                if (!restaurant.IsOpen)
                {
                    _logger.LogWarning($"Restaurant {restaurantId} is closed");
                    result.IsValid = false;
                    result.ErrorMessage = "Restaurant is currently closed.";
                    return result;
                }

                // Calculate subtotal and validate items
                decimal subtotal = 0;
                var validatedItems = new List<PricingItemDto>();

                foreach (var item in items)
                {
                    var dish = await _unitOfWork.OrderRepository.GetDishByIdAsync(item.DishId);
                    if (dish == null)
                    {
                        _logger.LogWarning($"Dish not found with ID: {item.DishId}");
                        result.IsValid = false;
                        result.ErrorMessage = $"Dish with ID {item.DishId} not found.";
                        return result;
                    }

                    if (!dish.IsAvailable)
                    {
                        _logger.LogWarning($"Dish {dish.Name} is not available");
                        result.IsValid = false;
                        result.ErrorMessage = $"Dish '{dish.Name}' is currently unavailable.";
                        return result;
                    }

                    if (dish.RestaurantId != restaurantId)
                    {
                        _logger.LogWarning($"Dish {dish.Name} does not belong to restaurant {restaurantId}");
                        result.IsValid = false;
                        result.ErrorMessage = $"Dish '{dish.Name}' does not belong to the selected restaurant.";
                        return result;
                    }

                    if (item.Quantity <= 0 || item.Quantity > 10)
                    {
                        _logger.LogWarning($"Invalid quantity {item.Quantity} for dish {dish.Name}");
                        result.IsValid = false;
                        result.ErrorMessage = $"Invalid quantity for dish '{dish.Name}'. Must be between 1 and 10.";
                        return result;
                    }

                    // Use current database price (security measure)
                    var itemTotal = dish.Price * item.Quantity;
                    subtotal += itemTotal;

                    _logger.LogInformation($"Dish processed: {dish.Name}, Price={dish.Price:F2} JOD, Quantity={item.Quantity}, Total={itemTotal:F2} JOD");

                    validatedItems.Add(new PricingItemDto
                    {
                        DishId = dish.Id,
                        DishName = dish.Name,
                        UnitPrice = dish.Price,
                        Quantity = item.Quantity,
                        IsAvailable = dish.IsAvailable
                    });
                }

                _logger.LogInformation($"Subtotal calculated: {subtotal:F2} JOD");

                // Calculate delivery fee
                decimal deliveryFee = await CalculateDeliveryFeeAsync(subtotal, restaurantId);
                _logger.LogInformation($"Delivery fee calculated: {deliveryFee:F2} JOD");

                // Calculate tax based on subtotal (before discounts)
                decimal taxAmount = Math.Round(subtotal * TAX_RATE, 2);
                _logger.LogInformation($"Tax amount calculated: {taxAmount:F2} JOD (Rate: {TAX_RATE:P})");

                // Handle promo code validation and discounts (if provided)
                decimal discountAmount = 0;
                bool freeDeliveryApplied = false;
                string? promoCodeApplied = null;
                string? promoCodeMessage = null;

                if (!string.IsNullOrEmpty(promoCode))
                {
                    _logger.LogInformation($"Processing promo code: {promoCode}");
                    var promoResult = await _unitOfWork.CartRepository.GetPromoCodeByCodeAsync(promoCode);
                    
                                         if (promoResult != null && promoResult.IsActive && 
                         (promoResult.ExpiryDate == null || promoResult.ExpiryDate >= DateTime.UtcNow) &&
                         subtotal >= promoResult.MinimumOrderAmount)
                    {
                        promoCodeApplied = promoCode.ToUpper();
                        promoCodeMessage = promoResult.Description;

                        // Apply free delivery
                        if (promoResult.FreeDelivery)
                        {
                            deliveryFee = 0;
                            freeDeliveryApplied = true;
                            _logger.LogInformation("Free delivery applied via promo code");
                        }

                        // Apply discount
                        if (promoResult.DiscountPercentage > 0)
                        {
                            discountAmount = Math.Round(subtotal * (promoResult.DiscountPercentage / 100), 2);
                            _logger.LogInformation($"Percentage discount applied: {promoResult.DiscountPercentage}%, Amount: {discountAmount:F2} JOD");
                        }
                        else if (promoResult.DiscountAmount > 0)
                        {
                            discountAmount = Math.Min(promoResult.DiscountAmount, subtotal); // Don't exceed subtotal
                            _logger.LogInformation($"Fixed discount applied: {discountAmount:F2} JOD");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Promo code validation failed: {promoCode}");
                    }
                }

                // Calculate grand total
                decimal grandTotal = Math.Round(subtotal + deliveryFee + taxAmount - discountAmount, 2);
                
                // Ensure total is never negative
                if (grandTotal < 0)
                {
                    grandTotal = 0;
                    _logger.LogWarning("Grand total was negative, adjusted to 0");
                }

                _logger.LogInformation($"Final calculation - Subtotal: {subtotal:F2}, Delivery: {deliveryFee:F2}, Tax: {taxAmount:F2}, Discount: {discountAmount:F2}, Total: {grandTotal:F2} JOD");

                // Populate result
                result.Subtotal = subtotal;
                result.DeliveryFee = deliveryFee;
                result.TaxAmount = taxAmount;
                result.TaxRate = TAX_RATE;
                result.DiscountAmount = discountAmount;
                result.GrandTotal = grandTotal;
                result.FreeDeliveryApplied = freeDeliveryApplied;
                result.PromoCodeApplied = promoCodeApplied;
                result.PromoCodeMessage = promoCodeMessage;
                result.ItemDetails = validatedItems;
                result.Currency = "JOD";
                result.IsValid = true;

                _logger.LogInformation("Pricing calculation completed successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while calculating pricing for restaurant {restaurantId}");
                return new PricingCalculationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Internal server error: {ex.Message}"
                };
            }
        }

        public async Task<decimal> CalculateDeliveryFeeAsync(decimal subtotal, int restaurantId)
        {
            try
            {
                var restaurant = await _unitOfWork.CartRepository.GetRestaurantByIdAsync(restaurantId);
                decimal restaurantDeliveryFee = restaurant?.DeliveryFee ?? STANDARD_DELIVERY_FEE;

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
                
                // Use standard delivery fee
                var standardFee = Math.Max(STANDARD_DELIVERY_FEE, restaurantDeliveryFee);
                _logger.LogInformation($"Standard delivery fee applied: {standardFee:F2} JOD");
                return standardFee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating delivery fee for restaurant {restaurantId}");
                return STANDARD_DELIVERY_FEE; // Fallback to standard fee
            }
        }
    }
} 