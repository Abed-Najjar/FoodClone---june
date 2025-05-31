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
        public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpDto generateOtpDto)
        {
            try
            {
                var result = await _otpService.GenerateOtpAsync(generateOtpDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while generating OTP", error = ex.Message });            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            try
            {
                var result = await _otpService.VerifyOtpAsync(verifyOtpDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while verifying OTP", error = ex.Message });
            }
        }        [HttpPost("register-with-otp")]
        public async Task<IActionResult> RegisterWithOtp([FromBody] RegistrationWithOtpDto registrationDto)
        {
            try
            {
                // Debug logging
                Console.WriteLine($"Received registration data:");
                Console.WriteLine($"  Username: {registrationDto?.Username ?? "[NULL]"}");
                Console.WriteLine($"  Email: {registrationDto?.Email ?? "[NULL]"}");
                Console.WriteLine($"  Password: {(string.IsNullOrEmpty(registrationDto?.Password) ? "[EMPTY]" : "[SET]")}");
                Console.WriteLine($"  Address: [{string.Join(", ", registrationDto?.Address ?? new List<string>())}]");
                Console.WriteLine($"  OtpCode: {registrationDto?.OtpCode ?? "[NULL]"}");
                
                // Check if dto is null
                if (registrationDto == null)
                {
                    return BadRequest(new { success = false, message = "Registration data is required" });
                }                // Check model state
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                    return BadRequest(new { success = false, message = "Invalid form data", errors = errors });
                }
                
                var result = await _otpService.RegisterWithOtpAsync(registrationDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in RegisterWithOtp: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred during registration", error = ex.Message });
            }
        }        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] PasswordResetWithOtpDto resetDto)
        {
            try
            {
                // Debug logging
                Console.WriteLine($"Received password reset data:");
                Console.WriteLine($"  Email: {resetDto?.Email ?? "[NULL]"}");
                Console.WriteLine($"  NewPassword: {(string.IsNullOrEmpty(resetDto?.NewPassword) ? "[EMPTY]" : "[SET]")}");
                Console.WriteLine($"  OtpCode: {resetDto?.OtpCode ?? "[NULL]"}");
                
                // Check if dto is null
                if (resetDto == null)
                {
                    return BadRequest(new { success = false, message = "Password reset data is required" });
                }
                
                // Check model state
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    Console.WriteLine($"Model validation errors: {string.Join(", ", errors)}");
                    return BadRequest(new { success = false, message = "Invalid form data", errors = errors });
                }
                
                var result = await _otpService.ResetPasswordWithOtpAsync(resetDto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ResetPasswordWithOtp: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An error occurred while resetting password", error = ex.Message });
            }
        }

        [HttpDelete("cleanup-expired")]
        public async Task<IActionResult> CleanupExpiredOtps()
        {
            try
            {
                await _otpService.CleanupExpiredOtpsAsync();
                return Ok(new { success = true, message = "Expired OTPs cleaned up successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while cleaning up expired OTPs", error = ex.Message });
            }
        }
    }
}
