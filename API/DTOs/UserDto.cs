using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    // DTO for user registration and input operations
    public class UserInputDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        [StringLength(10, MinimumLength = 3)]
        public string? Role { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; } = string.Empty;
        public List<string>? Address { get; set; }
    }

    // DTO for user profile update
    public class UserProfileUpdateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public string? Address { get; set; }
    }

    // DTO for password change
    public class PasswordChangeDto
    {
        [Required]
        public required string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string NewPassword { get; set; } = string.Empty;

        [Required]
        public required string ConfirmPassword { get; set; } = string.Empty;
    }

    // DTO for user statistics
    public class UserStatisticsDto
    {
        public int TotalOrders { get; set; }
        public int FavoriteRestaurants { get; set; }
        public int ReviewsWritten { get; set; }
        public int SavedAddresses { get; set; }
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
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Rolename { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Createdat { get; set; }
        public List<string> Address { get; set; } = new List<string>();
        
        // Additional profile fields
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? ProfileImageUrl { get; set; }
    }

    
}