using API.Enums;
using System.Collections.Generic; // Added for ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class User
    {        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Address { get; set; } = new List<string>(); // Legacy field - will be deprecated
        public string PhoneNumber { get; set; } = string.Empty;
        
        // Additional profile fields
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public DateTime? LastLogin { get; set; }
        
        // Navigation property for multiple user addresses
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public string ImageUrl { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.User;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for many-to-many relationship with Restaurant
        public virtual ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();

        // Navigation property for orders placed by the user (as a customer)
        [InverseProperty("User")]
        public virtual ICollection<Order> OrdersPlaced { get; set; } = new List<Order>();

        // Navigation property for orders delivered by the user (as an employee)
        [InverseProperty("Employee")]
        public virtual ICollection<Order> OrdersDelivered { get; set; } = new List<Order>();
    }
}