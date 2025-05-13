using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;


        public int UserId { get; set; }
        public virtual required User User { get; set; } // Navigation property to User (Customer)
        public int RestaurantId { get; set; }
        public virtual required Restaurant Restaurant { get; set; } // Navigation property

        public int EmployeeId { get; set; } // Foreign key for the User delivering the order
        public virtual required User Employee { get; set; } // Navigation property to User (Employee/Delivery Person)
    }
}