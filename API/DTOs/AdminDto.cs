using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Enums;

namespace API.DTOs
{
    // DTO for admin registration and input operations
    public class AdminInputDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string AdminName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }

    public class AdminUpdateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string AdminName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int MyProperty { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }

    // DTO for admin login
    public class AdminLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // DTO for admin output operations (excludes sensitive info like password)
    public class AdminDto
    {
        public int Id { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}