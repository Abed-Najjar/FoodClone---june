using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Argon;
using API.Services.TokenServiceFolder.AuthServiceFolder;

namespace API.Services.TokenServiceFolder.AuthService
{
    public class AuthService(
        AppDbContext context,
         ITokenService tokenService,
          IArgonHashing argonHashing,
          IUserRepository userRepository) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly AppDbContext _context = context;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IArgonHashing _argonHashing = argonHashing;

        public async Task<AppResponse<PasswordResetDto>> ForgotPassword(PasswordResetRequestDto dto)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(dto.Email);
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
                var user = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (user == null || !await _argonHashing.VerifyHashedPasswordAsync(user.PasswordHash, dto.Password))
                {
                    return new AppResponse<UserDto>(null, "Invalid credentials", 401, false);
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user),
                    Address = user.Address,
                    Status = user.Status.ToString(),
                    IsActive = user.Status == API.Enums.UserStatus.Active
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
                // Validate input
                if (dto == null)
                {
                    return new AppResponse<UserDto>(null, "Registration data is missing", 400, false);
                }

                if (string.IsNullOrEmpty(dto.FirstName))
                {
                    return new AppResponse<UserDto>(null, "First name is required", 400, false);
                }

                if (string.IsNullOrEmpty(dto.LastName))
                {
                    return new AppResponse<UserDto>(null, "Last name is required", 400, false);
                }

                if (string.IsNullOrEmpty(dto.Email))
                {
                    return new AppResponse<UserDto>(null, "Email is required", 400, false);
                }

                if (string.IsNullOrEmpty(dto.Password))
                {
                    return new AppResponse<UserDto>(null, "Password is required", 400, false);
                }

                // Address is now optional - no validation needed

                var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return new AppResponse<UserDto>(null, "User with this email already exists", 409, false);
                }

                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Address = dto.Address ?? new List<string>(),
                    Status = dto.IsActive.HasValue && dto.IsActive.Value ? API.Enums.UserStatus.Active : API.Enums.UserStatus.Active,
                    PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password),
                    CreatedAt = DateTime.UtcNow
                };

                // Save user first to ensure it gets an ID
                await _userRepository.CreateUserAsync(user);
                await _context.SaveChangesAsync();

                // Check if user was created successfully
                if (user.Id <= 0)
                {
                    return new AppResponse<UserDto>(null, "Failed to create user account", 500, false);
                }

                // Create DTO after user is saved
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user),
                    Address = user.Address,
                    Createdat = user.CreatedAt,
                    Status = user.Status.ToString(),
                    IsActive = user.Status == API.Enums.UserStatus.Active
                };
                return new AppResponse<UserDto>(userDto, "User registered successfully", 200, true);
            }
            catch (Exception ex)
            {
                return new AppResponse<UserDto>(null, ex.Message, 500, false);
            }
        }
    }
}