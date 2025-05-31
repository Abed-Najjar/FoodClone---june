using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Otp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public OtpType Type { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UsedAt { get; set; }

        // Optional: User ID if linked to a specific user
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }

    public enum OtpType
    {
        Registration = 1,
        ForgotPassword = 2,
        ResetPassword = 3,
        EmailVerification = 4,
        TwoFactorAuthentication = 5
    }
}
