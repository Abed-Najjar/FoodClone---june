using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public int RestaurantId { get; set; } // Foreign key to Restaurant
        public required virtual Restaurant Restaurant { get; set; } // Navigation property to Restaurant

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Navigation property for join table
    }
}