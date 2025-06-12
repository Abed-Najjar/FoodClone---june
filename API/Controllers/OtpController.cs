using API.AppResponse;
using API.DTOs;
using API.Services;
using API.Services.OtpService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }
        
        [HttpPost("generate")]
        public async Task<AppResponse<OtpResponseDto>> GenerateOtp([FromBody] GenerateOtpDto generateOtpDto)
        {
            return await _otpService.GenerateOtpAsync(generateOtpDto);
        }

        [HttpPost("verify")]
        public async Task<AppResponse<OtpResponseDto>> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            return await _otpService.VerifyOtpAsync(verifyOtpDto);
        }        
        
        [HttpPost("register-with-otp")]
        public async Task<AppResponse<UserDto>> RegisterWithOtp([FromBody] RegistrationWithOtpDto registrationDto)
        {
            return await _otpService.RegisterWithOtpAsync(registrationDto);
        }        
        
        [HttpPost("reset-password")]
        public async Task<AppResponse<bool>> ResetPasswordWithOtp([FromBody] PasswordResetWithOtpDto resetDto)
        {
            return await _otpService.ResetPasswordWithOtpAsync(resetDto);
        }

        [HttpDelete("cleanup-expired")]
        public async Task<AppResponse<bool>> CleanupExpiredOtps()
        {
            return await _otpService.CleanupExpiredOtpsAsync();
        }
    }
}
