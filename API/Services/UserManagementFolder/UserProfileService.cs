using API.AppResponse;
using API.DTOs;
using API.Models;
using API.Services.Argon;
using API.Services.TokenServiceFolder;
using API.Services.CmsServiceFolder;
using API.UoW;
using Microsoft.AspNetCore.Mvc;

public class UserProfileService : IUserProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IArgonHashing _argonHashing;
    private readonly ITokenService _tokenService;
    private readonly ICloudinaryService _cloudinaryService;

    public UserProfileService(IUnitOfWork unitOfWork, IArgonHashing argonHashing, ITokenService tokenService, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _argonHashing = argonHashing;
        _tokenService = tokenService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<AppResponse<UserDto>> GetUserProfile(int userId)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.FindAsync(userId);

            if (user == null)
            {
                return new AppResponse<UserDto>(null, "User not found", 404, false);
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Rolename = user.Role.ToString(),
                Token = _tokenService.CreateToken(user),
                Address = user.Address,
                Createdat = user.CreatedAt,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Bio = user.Bio,
                LastLogin = user.LastLogin,
                ProfileImageUrl = user.ImageUrl
            };

            return new AppResponse<UserDto>(userDto, "User profile retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<UserDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<UserDto>> UpdateUserProfile(int userId, UserProfileUpdateDto dto)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.FindAsync(userId);

            if (user == null)
            {
                return new AppResponse<UserDto>(null, "User not found", 404, false);
            }

            // Check if email already exists for other users
            // Note: We no longer check for username uniqueness since we're using FirstName + LastName

            var existingUserByEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != userId)
            {
                return new AppResponse<UserDto>(null, "Email already exists", 409, false);
            }

            // Update user fields
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.Phone ?? string.Empty;
            
            // Parse DateOfBirth from string
            if (!string.IsNullOrEmpty(dto.DateOfBirth))
            {
                if (DateTime.TryParse(dto.DateOfBirth, out DateTime parsedDate))
                {
                    user.DateOfBirth = parsedDate;
                }
            }
            else
            {
                user.DateOfBirth = null;
            }
            
            user.Gender = dto.Gender;
            user.Bio = dto.Bio;

            // Update address if provided
            if (!string.IsNullOrEmpty(dto.Address))
            {
                user.Address = new List<string> { dto.Address };
            }

            await _unitOfWork.UserRepository.UpdateUserAsync(user);
            await _unitOfWork.CompleteAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Rolename = user.Role.ToString(),
                Token = _tokenService.CreateToken(user),
                Createdat = user.CreatedAt,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Bio = user.Bio,
                LastLogin = user.LastLogin,
                ProfileImageUrl = user.ImageUrl
            };

            return new AppResponse<UserDto>(userDto, "User profile updated successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<UserDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<string>> UploadProfileImage(int userId, IFormFile imageFile)
    {
        try
        {
            Console.WriteLine($"UserProfileService: Starting image upload for user {userId}");
            
            var user = await _unitOfWork.UserRepository.FindAsync(userId);

            if (user == null)
            {
                Console.WriteLine($"UserProfileService: User {userId} not found");
                return new AppResponse<string>(null, "User not found", 404, false);
            }

            Console.WriteLine($"UserProfileService: User found, uploading to Cloudinary...");
            
            // Upload image to Cloudinary
            var uploadResult = await _cloudinaryService.UploadImageAsync(imageFile, "user-profiles");

            Console.WriteLine($"UserProfileService: Cloudinary upload result: {uploadResult ?? "NULL"}");

            if (string.IsNullOrEmpty(uploadResult))
            {
                Console.WriteLine("UserProfileService: Cloudinary upload returned null or empty");
                return new AppResponse<string>(null, "Failed to upload image", 500, false);
            }

            Console.WriteLine($"UserProfileService: Updating user image URL to: {uploadResult}");
            
            // Update user's image URL
            user.ImageUrl = uploadResult;
            await _unitOfWork.UserRepository.UpdateUserAsync(user);
            await _unitOfWork.CompleteAsync();

            Console.WriteLine($"UserProfileService: Successfully updated user image URL");

            return new AppResponse<string>(user.ImageUrl, "Profile image uploaded successfully", 200, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UserProfileService: Exception during image upload: {ex.Message}");
            Console.WriteLine($"UserProfileService: Stack trace: {ex.StackTrace}");
            return new AppResponse<string>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<bool>> ChangePassword(int userId, PasswordChangeDto dto)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.FindAsync(userId);

            if (user == null)
            {
                return new AppResponse<bool>(false, "User not found", 404, false);
            }

            // Verify current password
            var isCurrentPasswordValid = await _argonHashing.VerifyHashedPasswordAsync(user.PasswordHash, dto.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                return new AppResponse<bool>(false, "Current password is incorrect", 400, false);
            }

            // Check if new password and confirm password match
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return new AppResponse<bool>(false, "New password and confirm password do not match", 400, false);
            }

            // Hash and update new password
            user.PasswordHash = await _argonHashing.HashPasswordAsync(dto.NewPassword);
            await _unitOfWork.UserRepository.UpdateUserAsync(user);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>(true, "Password changed successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<bool>(false, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<bool>> DeleteUserAccount(int userId)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.FindAsync(userId);

            if (user == null)
            {
                return new AppResponse<bool>(false, "User not found", 404, false);
            }

            await _unitOfWork.UserRepository.DeleteUserAsync(userId);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>(true, "User account deleted successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<bool>(false, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<UserStatisticsDto>> GetUserStatistics(int userId)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.FindAsync(userId);

            if (user == null)
            {
                return new AppResponse<UserStatisticsDto>(null, "User not found", 404, false);
            }

            // Get user statistics
            var totalOrders = await _unitOfWork.OrderRepository.GetOrderCountByUserIdAsync(userId);
            var savedAddresses = user.Addresses?.Count ?? user.Address?.Count ?? 0;

            var statistics = new UserStatisticsDto
            {
                TotalOrders = totalOrders,
                FavoriteRestaurants = 0, // TODO: Implement when favorites feature is added
                ReviewsWritten = 0, // TODO: Implement when reviews feature is added
                SavedAddresses = savedAddresses
            };

            return new AppResponse<UserStatisticsDto>(statistics, "User statistics retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<UserStatisticsDto>(null, ex.Message, 500, false);
        }
    }
} 