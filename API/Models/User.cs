using API.Enums;
using System.Collections.Generic; // Added for ICollection
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Address { get; set; } = new List<string>();
        public string PhoneNumber { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.User;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for many-to-many relationship with Restaurant
        public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

        // Navigation property for orders placed by the user (as a customer)
        public virtual ICollection<Order> OrdersPlaced { get; set; } = new List<Order>();

        // Navigation property for orders delivered by the user (as an employee)
        public virtual ICollection<Order> OrdersDelivered { get; set; } = new List<Order>();
    }
}