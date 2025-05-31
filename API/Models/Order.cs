using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{    public class Order
    {
        public int Id { get; set; }
        public virtual ICollection<OrderDish> OrderItems { get; set; } = new List<OrderDish>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; } = 0;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Change to ENUM Later.

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!; // Navigation property to User (Customer)
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;  // Navigation property

        public int? EmployeeId { get; set; } // Nullable to allow order creation without delivery person
        public virtual User? Employee { get; set; } // Fixed the nullable reference

        // Delivery address information
        public int? DeliveryAddressId { get; set; }
        public virtual Address? DeliveryAddress { get; set; }
        public string? DeliveryInstructions { get; set; } = string.Empty;

        [NotMapped]
        public decimal CalculatedTotalAmount => OrderItems.Sum(item => item.Quantity * item.UnitPrice);
    }
}