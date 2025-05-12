namespace API.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public List<Category> Categories { get; set; } = new List<Category>();
        public string Address { get; set; } = string.Empty;
        public decimal Rating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public string OpeningHours { get; set; } = string.Empty;
        public decimal DeliveryFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSupended { get; set; } = false;
    }

        
}