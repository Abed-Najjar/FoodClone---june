using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using API.Services.Argon;
using API.Services.TokenServiceFolder.AuthServiceFolder;
using Microsoft.EntityFrameworkCore;

namespace API.Services.TokenServiceFolder.AuthService
{
    public class AuthService(AppDbContext context, ITokenService tokenService, IArgonHashing argonHashing) : IAuthService
    {
        private readonly AppDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IArgonHashing _argonHashing = argonHashing;

        public async Task<AppResponse<PasswordResetDto>> ForgotPassword(PasswordResetRequestDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null)
                {
                    return new AppResponse<PasswordResetDto>(null, "User not found", 404, false);
                }
                
                user.PasswordHash = await _argonHashing.HashPasswordAsync("newpassword");
                await _context.SaveChangesAsync();

                var userDto = new PasswordResetDto
                {
                    Email = user.Email,
                    Newpassword = user.PasswordHash
                };

                return new AppResponse<PasswordResetDto>(userDto, "Password reset successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<PasswordResetDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<UserDto>> Login(UserLoginDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null || !await _argonHashing.VerifyHashedPasswordAsync(user.PasswordHash, dto.Password))
                {
                    return new AppResponse<UserDto>(null, "Invalid credentials", 401, false);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user) 
                };

                return new AppResponse<UserDto>(userDto, "Login successful", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<UserDto>> Register(UserInputDto dto)
        {
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (existingUser != null)
                {
                    return new AppResponse<UserDto>(null, "User already exists", 404, false);
                }

                var user = new User
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password), 
                    CreatedAt = DateTime.UtcNow
                };

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user)
                };
                
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return new AppResponse<UserDto>(userDto, "User registered successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
        }
    }
}