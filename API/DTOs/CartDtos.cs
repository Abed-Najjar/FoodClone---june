using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    // DTO for cart calculation request
    public class CartCalculationRequestDto
    {
        [Required]
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        
        [Required]
        public int RestaurantId { get; set; }
        
        public string? PromoCode { get; set; }
        
        public int? DeliveryAddressId { get; set; }
    }

    // DTO for individual cart item
    public class CartItemDto
    {
        [Required]
        public int DishId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    // DTO for cart calculation response (matches the Order Summary)
    public class CartCalculationResponseDto
    {
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TaxRate { get; set; }
        public bool FreeDeliveryApplied { get; set; }
        public string? PromoCodeApplied { get; set; }
        public string? PromoCodeMessage { get; set; }
        public List<CartItemDetailDto> ItemDetails { get; set; } = new List<CartItemDetailDto>();
        
        // Additional breakdown for transparency
        public string Currency { get; set; } = "JOD";
        public string FormattedSubtotal => $"{Subtotal:F2} {Currency}";
        public string FormattedDeliveryFee => $"{DeliveryFee:F2} {Currency}";
        public string FormattedTaxAmount => $"{TaxAmount:F2} {Currency}";
        public string FormattedGrandTotal => $"{GrandTotal:F2} {Currency}";
    }

    // DTO for cart item details with price information
    public class CartItemDetailDto
    {
        public int DishId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string FormattedUnitPrice => $"{UnitPrice:F2} JOD";
        public string FormattedTotalPrice => $"{TotalPrice:F2} JOD";
    }

    // DTO for promo code validation
    public class PromoCodeDto
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool FreeDelivery { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    // DTO for promo code validation request
    public class ValidatePromoCodeRequestDto
    {
        [Required]
        public string PromoCode { get; set; } = string.Empty;
        
        [Required]
        public decimal Subtotal { get; set; }
    }
} 