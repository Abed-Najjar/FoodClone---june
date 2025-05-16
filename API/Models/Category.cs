namespace API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        
        // Navigation property for many-to-many relationship with Restaurant
        public ICollection<RestaurantsCategories> RestaurantsCategories { get; set; } = new List<RestaurantsCategories>();

        // Navigation property - relationship to Dishes
        public List<Dish> Dishes { get; set; } = new List<Dish>();
    }
}