using API.AppResponse;
using API.DTOs;
using API.Services.TokenServiceFolder.AuthServiceFolder;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<AppResponse<UserDto>> Register(UserInputDto dto)
        {
            return await _authService.Register(dto);
        }

        [HttpPost("login")]
        public async Task<AppResponse<UserDto>> Login(UserLoginDto dto)
        {
            return await _authService.Login(dto);
        }
      
        [HttpPost("forgotpassword")]
        public async Task<AppResponse<PasswordResetDto>> ForgotPassword(PasswordResetRequestDto dto)
        {
            return await _authService.ForgotPassword(dto);
        }

        // Add a ping endpoint for connection testing
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "API is running", timestamp = DateTime.UtcNow });
        }
    }
}