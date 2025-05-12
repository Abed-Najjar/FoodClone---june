using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    // DTO for user registration and input operations
    public class UserInputDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; } = string.Empty;
    }

    // DTO for user login
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        [Required]
        public required string Password { get; set; } = string.Empty;
    }

    // DTO for password reset request
    public class PasswordResetRequestDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;
    }

    // DTO for password reset completion
    public class PasswordResetDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Newpassword { get; set; } = string.Empty;
    }

    // DTO for user output operations (excludes sensitive info like password)
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Rolename { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Createdat { get; set; }
    }
}