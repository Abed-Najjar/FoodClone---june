using API.Enums;

namespace API.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.Admin;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        

    }


 
}