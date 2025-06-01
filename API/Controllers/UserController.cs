using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        [HttpGet("profile")]
        public async Task<AppResponse<UserDto>> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                return await _userProfileService.GetUserProfile(userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 401, false);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, "Internal server error", 500, false);
            }
        }

        [HttpPut("profile")]
        public async Task<AppResponse<UserDto>> UpdateProfile([FromBody] UserProfileUpdateDto dto)
        {
            try
            {
                // Check if model is valid
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Error = x.Value.Errors.First().ErrorMessage })
                        .ToList();
                    
                    var errorMessage = string.Join("; ", errors.Select(e => $"{e.Field}: {e.Error}"));
                    return new AppResponse<UserDto>(null, $"Validation failed: {errorMessage}", 400, false);
                }

                var userId = GetCurrentUserId();
                return await _userProfileService.UpdateUserProfile(userId, dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 401, false);
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Profile Update Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                return new AppResponse<UserDto>(null, $"Internal server error: {ex.Message}", 500, false);
            }
        }

        [HttpPost("profile/image")]
        public async Task<AppResponse<object>> UploadProfileImage([FromForm] IFormFile image)
        {
            try
            {
                Console.WriteLine($"Image upload request received. File: {image?.FileName}, Size: {image?.Length}");

                if (image == null || image.Length == 0)
                {
                    Console.WriteLine("No image file provided");
                    return new AppResponse<object>(null, "No image file provided", 400, false);
                }

                Console.WriteLine($"Image details - Name: {image.FileName}, Type: {image.ContentType}, Size: {image.Length}");

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(image.ContentType.ToLower()))
                {
                    Console.WriteLine($"Invalid file type: {image.ContentType}");
                    return new AppResponse<object>(null, "Invalid file type. Only JPEG, PNG, and GIF are allowed.", 400, false);
                }

                // Validate file size (max 5MB)
                if (image.Length > 5 * 1024 * 1024)
                {
                    Console.WriteLine($"File size too large: {image.Length} bytes");
                    return new AppResponse<object>(null, "File size exceeds 5MB limit", 400, false);
                }

                var userId = GetCurrentUserId();
                Console.WriteLine($"Uploading image for user ID: {userId}");
                
                var result = await _userProfileService.UploadProfileImage(userId, image);
                
                Console.WriteLine($"Upload result - Success: {result.Success}, Message: {result.ErrorMessage}");
                
                if (result.Success)
                {
                    return new AppResponse<object>(new { imageUrl = result.Data }, result.ErrorMessage, result.StatusCode, result.Success);
                }
                
                return new AppResponse<object>(null, result.ErrorMessage, result.StatusCode, result.Success);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Authorization error: {ex.Message}");
                return new AppResponse<object>(null, ex.Message, 401, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image upload error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new AppResponse<object>(null, $"Internal server error: {ex.Message}", 500, false);
            }
        }

        [HttpPost("change-password")]
        public async Task<AppResponse<object>> ChangePassword([FromBody] PasswordChangeDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _userProfileService.ChangePassword(userId, dto);
                
                return new AppResponse<object>(result.Data, result.ErrorMessage, result.StatusCode, result.Success);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new AppResponse<object>(null, ex.Message, 401, false);
            }
            catch (Exception ex)
            {
                return new AppResponse<object>(null, "Internal server error", 500, false);
            }
        }

        [HttpDelete("account")]
        public async Task<AppResponse<object>> DeleteAccount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _userProfileService.DeleteUserAccount(userId);
                
                return new AppResponse<object>(result.Data, result.ErrorMessage, result.StatusCode, result.Success);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new AppResponse<object>(null, ex.Message, 401, false);
            }
            catch (Exception ex)
            {
                return new AppResponse<object>(null, "Internal server error", 500, false);
            }
        }

        [HttpGet("statistics")]
        public async Task<AppResponse<UserStatisticsDto>> GetStatistics()
        {
            try
            {
                var userId = GetCurrentUserId();
                return await _userProfileService.GetUserStatistics(userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return new AppResponse<UserStatisticsDto>(null, ex.Message, 401, false);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserStatisticsDto>(null, "Internal server error", 500, false);
            }
        }
    }
} 