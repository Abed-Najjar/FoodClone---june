using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AdminRestaurantDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<string>? Categories { get; set; }
        public string? Address { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsOpen { get; set; }
        public string? OpeningHours { get; set; }
        public decimal DeliveryFee { get; set; }
        public bool Suspended { get; set; }
    }

    // Simplified DTO for list views that don't need all AdminRestaurant details
    public class AdminRestaurantSummaryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Address { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
    }

    // DTO for creating a new AdminRestaurant
    public class AdminRestaurantCreateDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        
        [Required]
        [StringLength(500)]
        public required string Description { get; set; } 
        
        [Required]
        public required string LogoUrl { get; set; }
        [Required]
        public required string CoverImageUrl { get; set; }

        public List<string> Categories { get; set; } = new List<string>();
        
        [Required]
        public required string Address { get; set; }
        
        [Required]
        [Phone]
        public required string PhoneNumber { get; set; }
        
        public required string Email { get; set; }
        
        [Required]
        public required string OpeningHours { get; set; }

    }

    // DTO for updating an existing AdminRestaurant
    public class AdminRestaurantUpdateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
         public List<string>? Categories { get; set; }
        public string? Address { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }

        public string? OpeningHours { get; set; }

        public bool Issuspended { get; set; }
        
    }
  
}