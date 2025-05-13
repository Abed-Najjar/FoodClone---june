using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public virtual ICollection<OrderDish> OrderItems { get; set; } = new List<OrderDish>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; } = 0;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Set a default status

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!; // Navigation property to User (Customer)
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;  // Navigation property

        public int? EmployeeId { get; set; } // Nullable to allow order creation without delivery person
        public virtual User? Employee { get; set; } // Fixed the nullable reference

        [NotMapped]
        public decimal CalculatedTotalAmount => OrderItems.Sum(item => item.Quantity * item.UnitPrice);
    }
}