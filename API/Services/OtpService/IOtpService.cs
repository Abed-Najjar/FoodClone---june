using API.AppResponse;
using API.DTOs;
using API.Models;

namespace API.Services.OtpService
{
    public interface IOtpService
    {
        Task<AppResponse<OtpResponseDto>> GenerateOtpAsync(GenerateOtpDto dto);
        Task<AppResponse<OtpResponseDto>> VerifyOtpAsync(VerifyOtpDto dto);
        Task<AppResponse<bool>> ResendOtpAsync(string email, OtpType type);
        Task<AppResponse<UserDto>> RegisterWithOtpAsync(RegistrationWithOtpDto dto);
        Task<AppResponse<bool>> ResetPasswordWithOtpAsync(PasswordResetWithOtpDto dto);
        Task<AppResponse<bool>> CleanupExpiredOtpsAsync();
    }
}
