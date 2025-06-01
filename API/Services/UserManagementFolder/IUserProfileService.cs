using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface IUserProfileService
{
    Task<AppResponse<UserDto>> GetUserProfile(int userId);
    Task<AppResponse<UserDto>> UpdateUserProfile(int userId, UserProfileUpdateDto dto);
    Task<AppResponse<string>> UploadProfileImage(int userId, IFormFile imageFile);
    Task<AppResponse<bool>> ChangePassword(int userId, PasswordChangeDto dto);
    Task<AppResponse<bool>> DeleteUserAccount(int userId);
    Task<AppResponse<UserStatisticsDto>> GetUserStatistics(int userId);
} 