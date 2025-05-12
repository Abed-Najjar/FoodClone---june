using API.Enums;

namespace API.Models
{    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<string> Address { get; set; } = new List<string>();
        public string PhoneNumber { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.User;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    }

}