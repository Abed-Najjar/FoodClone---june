using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class OrderDish
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        
        public int DishId { get; set; }
        public virtual Dish Dish { get; set; } = null!;
        
        public int Quantity { get; set; } = 1;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
