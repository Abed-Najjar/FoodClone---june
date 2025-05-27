using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{    public class DishDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsAvailable { get; set; } = true;
    }


      public class AdminRestaurantDishDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

        public class UserRestaurantDishesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
    public class CreateDishDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int RestaurantId { get; set; }

        public int? CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }    public class UpdateDishDto
    {
        
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public int RestaurantId { get; set; }
        public int? CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
} 