namespace API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        
        // Navigation property - relationship to Restaurant
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;     

        // Navigation property - relationship to Dishes
        public List<Dish> Dishes { get; set; } = new List<Dish>();
    }
}