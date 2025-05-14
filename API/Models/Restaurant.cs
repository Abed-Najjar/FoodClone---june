using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Rating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public string OpeningHours { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSupended { get; set; } = false;

        // Navigation property for many-to-many relationship with Category
        public virtual List<Category> Categories { get; set; } = new List<Category>();

        // Navigation property for many-to-many relationship with User
        public virtual List<User> Users { get; set; } = new List<User>();

        // Collection navigation property for Orders
        public virtual List<Order> Orders { get; set; } = new List<Order>();

        // Collection navigation property for Dishes offered by the restaurant
        public virtual List<Dish> Dishes { get; set; } = new List<Dish>();

        // Collection navigation property for restaurant categories
        // This is the many-to-many relationship with Category
        public ICollection<RestaurantsCategories> RestaurantsCategories { get; set; } = new List<RestaurantsCategories>();
    }
}