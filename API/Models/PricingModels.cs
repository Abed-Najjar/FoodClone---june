namespace API.Models
{
    public class PricingItemDto
    {
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string DishName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    public class PricingCalculationResult
    {
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public bool FreeDeliveryApplied { get; set; }
        public string? PromoCodeApplied { get; set; }
        public string? PromoCodeMessage { get; set; }
        public List<PricingItemDto> ItemDetails { get; set; } = new List<PricingItemDto>();
        public string Currency { get; set; } = "JOD";
        public bool IsValid { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }


} 