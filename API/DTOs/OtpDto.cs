using System.ComponentModel.DataAnnotations;
using API.Models;

namespace API.DTOs
{
    // DTO for generating OTP
    public class GenerateOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public OtpType Type { get; set; }

        public int? UserId { get; set; }
    }

    // DTO for verifying OTP
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public OtpType Type { get; set; }
    }

    // DTO for OTP response
    public class OtpResponseDto
    {
        public string Email { get; set; } = string.Empty;
        public OtpType Type { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // DTO for password reset with OTP
    public class PasswordResetWithOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    // DTO for registration with OTP verification
    public class RegistrationWithOtpDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public List<string> Address { get; set; } = new List<string>();

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OtpCode { get; set; } = string.Empty;
    }
}
