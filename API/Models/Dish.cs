namespace API.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;



        // Foreign key to Section
        public int SectionId { get; set; } 

        // Navigation property
        public Category Category { get; set; } = null!;
    }
}