using System;

namespace API.Models
{
    public class RestaurantsCategories
    {
        public int RestaurantId { get; set; }
        public required Restaurant Restaurant { get; set; }
        
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
    }
}
