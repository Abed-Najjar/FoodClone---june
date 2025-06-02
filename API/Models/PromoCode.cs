using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class PromoCode
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; } = 0;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; } = 0;
        
        public bool FreeDelivery { get; set; } = false;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumOrderAmount { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExpiryDate { get; set; }
        
        public int? UsageLimit { get; set; }
        
        public int TimesUsed { get; set; } = 0;
        
        // Navigation properties
        public int? RestaurantId { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
    }
} 