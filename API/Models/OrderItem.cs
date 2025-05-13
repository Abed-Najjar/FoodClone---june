using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public required virtual Order Order { get; set; } // Navigation property to Order

        public int DishId { get; set; }
        public required virtual Dish Dish { get; set; } // Navigation property to Dish

        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; } // Price of the dish at the time of order

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
