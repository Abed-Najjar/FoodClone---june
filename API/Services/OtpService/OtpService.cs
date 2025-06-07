using System.Security.Cryptography;
using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Argon;
using API.Services.EmailService;
using API.Services.TokenServiceFolder;

namespace API.Services.OtpService
{
    public class OtpService : IOtpService
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IArgonHashing _argonHashing;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OtpService> _logger;

        // Configuration constants
        private const int OTP_LENGTH = 6;
        private const int OTP_EXPIRY_MINUTES = 15;
        private const int MAX_OTP_ATTEMPTS_PER_HOUR = 5;
        private const int RATE_LIMIT_MINUTES = 60;

        public OtpService(
            IOtpRepository otpRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            ITokenService tokenService,
            IArgonHashing argonHashing,
            AppDbContext context,
            IConfiguration configuration,
            ILogger<OtpService> logger)
        {
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _tokenService = tokenService;
            _argonHashing = argonHashing;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AppResponse<OtpResponseDto>> GenerateOtpAsync(GenerateOtpDto dto)
        {
            try
            {
                _logger.LogInformation("Generating OTP for email: {Email}, type: {Type}", dto.Email, dto.Type);

                // Rate limiting check
                var attemptsCount = await _otpRepository.GetOtpAttemptsCountAsync(
                    dto.Email, dto.Type, TimeSpan.FromMinutes(RATE_LIMIT_MINUTES));

                if (attemptsCount >= MAX_OTP_ATTEMPTS_PER_HOUR)
                {
                    return new AppResponse<OtpResponseDto>(null, 
                        $"Too many OTP requests. Please try again after {RATE_LIMIT_MINUTES} minutes.", 429, false);
                }

                // For registration, check if user already exists
                if (dto.Type == OtpType.Registration)
                {
                    var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
                    if (existingUser != null)
                    {
                        return new AppResponse<OtpResponseDto>(null, 
                            "An account with this email already exists.", 409, false);
                    }
                }

                // For password reset, ensure user exists
                if (dto.Type == OtpType.ForgotPassword || dto.Type == OtpType.ResetPassword)
                {
                    var user = await _userRepository.GetUserByEmailAsync(dto.Email);
                    if (user == null)
                    {
                        return new AppResponse<OtpResponseDto>(null, 
                            "No account found with this email address.", 404, false);
                    }
                    dto.UserId = user.Id;
                }

                // Invalidate any existing OTPs for this email and type
                await _otpRepository.InvalidateAllUserOtpsAsync(dto.Email, dto.Type);

                // Generate OTP code
                var otpCode = GenerateRandomOtpCode();
                
                // Create OTP record
                var otp = new Otp
                {
                    Email = dto.Email,
                    Code = otpCode,
                    Type = dto.Type,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(OTP_EXPIRY_MINUTES),
                    UserId = dto.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                await _otpRepository.CreateOtpAsync(otp);

                // Send email
                var emailSent = await SendOtpEmailAsync(dto.Email, otpCode, dto.Type);
                if (!emailSent)
                {
                    return new AppResponse<OtpResponseDto>(null, 
                        "Failed to send OTP email. Please try again.", 500, false);
                }

                var response = new OtpResponseDto
                {
                    Email = dto.Email,
                    Type = dto.Type,
                    ExpiryDate = otp.ExpiryDate,
                    IsSuccess = true,
                    Message = $"OTP sent successfully to {dto.Email}. Valid for {OTP_EXPIRY_MINUTES} minutes."
                };

                _logger.LogInformation("OTP generated and sent successfully for email: {Email}", dto.Email);

                return new AppResponse<OtpResponseDto>(response, "OTP sent successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP for email: {Email}", dto.Email);
                return new AppResponse<OtpResponseDto>(null, "An error occurred while generating OTP", 500, false);
            }
        }

        public async Task<AppResponse<OtpResponseDto>> VerifyOtpAsync(VerifyOtpDto dto)
        {
            try
            {
                _logger.LogInformation("Verifying OTP for email: {Email}, type: {Type}", dto.Email, dto.Type);

                var otp = await _otpRepository.GetValidOtpAsync(dto.Email, dto.Code, dto.Type);
                
                if (otp == null)
                {
                    return new AppResponse<OtpResponseDto>(null, 
                        "Invalid or expired OTP code.", 400, false);
                }

                // Mark OTP as used
                await _otpRepository.InvalidateOtpAsync(otp.Id);

                var response = new OtpResponseDto
                {
                    Email = dto.Email,
                    Type = dto.Type,
                    ExpiryDate = otp.ExpiryDate,
                    IsSuccess = true,
                    Message = "OTP verified successfully"
                };

                _logger.LogInformation("OTP verified successfully for email: {Email}", dto.Email);

                return new AppResponse<OtpResponseDto>(response, "OTP verified successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for email: {Email}", dto.Email);
                return new AppResponse<OtpResponseDto>(null, "An error occurred while verifying OTP", 500, false);
            }
        }

        public async Task<AppResponse<bool>> ResendOtpAsync(string email, OtpType type)
        {
            try
            {
                var generateDto = new GenerateOtpDto
                {
                    Email = email,
                    Type = type
                };                
                
                var result = await GenerateOtpAsync(generateDto);
                return new AppResponse<bool>(result.Success, result.ErrorMessage ?? "OTP resent successfully", 
                    result.StatusCode, result.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending OTP for email: {Email}", email);
                return new AppResponse<bool>(false, "An error occurred while resending OTP", 500, false);
            }
        }

        public async Task<AppResponse<UserDto>> RegisterWithOtpAsync(RegistrationWithOtpDto dto)
        {
            try
            {
                _logger.LogInformation("Processing registration with OTP for email: {Email}", dto.Email);

                // First verify OTP
                var otpVerifyDto = new VerifyOtpDto
                {
                    Email = dto.Email,
                    Code = dto.OtpCode,
                    Type = OtpType.Registration                };

                var otpResult = await VerifyOtpAsync(otpVerifyDto);
                if (!otpResult.Success)
                {
                    return new AppResponse<UserDto>(null, otpResult.ErrorMessage!, otpResult.StatusCode, false);
                }

                // Check if user already exists (double check)
                var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return new AppResponse<UserDto>(null, "User with this email already exists", 409, false);
                }

                // Create new user
                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Address = dto.Address ?? new List<string>(),
                    PasswordHash = await _argonHashing.HashPasswordAsync(dto.Password),
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.CreateUserAsync(user);
                await _context.SaveChangesAsync();

                // Generate token
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Rolename = user.Role.ToString(),
                    Token = _tokenService.CreateToken(user),
                    Address = user.Address,
                    Createdat = user.CreatedAt
                };

                _logger.LogInformation("User registered successfully with OTP verification for email: {Email}", dto.Email);

                return new AppResponse<UserDto>(userDto, "User registered successfully", 201, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with OTP for email: {Email}", dto.Email);
                return new AppResponse<UserDto>(null, "An error occurred during registration", 500, false);
            }
        }

        public async Task<AppResponse<bool>> ResetPasswordWithOtpAsync(PasswordResetWithOtpDto dto)
        {
            try
            {
                _logger.LogInformation("Processing password reset with OTP for email: {Email}", dto.Email);                // First verify OTP - Use ForgotPassword type since that's what was generated
                var otpVerifyDto = new VerifyOtpDto
                {
                    Email = dto.Email,
                    Code = dto.OtpCode,
                    Type = OtpType.ForgotPassword // Changed from ResetPassword to ForgotPassword
                };var otpResult = await VerifyOtpAsync(otpVerifyDto);
                if (!otpResult.Success)
                {
                    return new AppResponse<bool>(false, otpResult.ErrorMessage!, otpResult.StatusCode, false);
                }

                // Get user
                var user = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (user == null)
                {
                    return new AppResponse<bool>(false, "User not found", 404, false);
                }

                // Update password
                user.PasswordHash = await _argonHashing.HashPasswordAsync(dto.NewPassword);
                await _userRepository.UpdateUserAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Password reset successfully with OTP verification for email: {Email}", dto.Email);

                return new AppResponse<bool>(true, "Password reset successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password with OTP for email: {Email}", dto.Email);
                return new AppResponse<bool>(false, "An error occurred while resetting password", 500, false);
            }
        }

        public async Task<AppResponse<bool>> CleanupExpiredOtpsAsync()
        {
            try
            {
                await _otpRepository.DeleteExpiredOtpsAsync();
                return new AppResponse<bool>(true, "Expired OTPs cleaned up successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired OTPs");
                return new AppResponse<bool>(false, "An error occurred while cleaning up expired OTPs", 500, false);
            }
        }

        private string GenerateRandomOtpCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = BitConverter.ToUInt32(bytes, 0) % 1000000;
            return randomNumber.ToString("D6");
        }

        private async Task<bool> SendOtpEmailAsync(string email, string otpCode, OtpType type)
        {
            try
            {
                var subject = GetEmailSubject(type);
                var body = GetEmailBody(otpCode, type);

                await _emailService.SendEmailAsync(email, subject, body);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to: {Email}", email);
                return false;
            }
        }

        private string GetEmailSubject(OtpType type)
        {
            var appName = _configuration.GetValue<string>("AppSettings:AppName") ?? "FoodClone";
            
            return type switch
            {
                OtpType.Registration => $"{appName} - Email Verification",
                OtpType.ForgotPassword => $"{appName} - Password Reset Request",
                OtpType.ResetPassword => $"{appName} - Password Reset Verification",
                OtpType.EmailVerification => $"{appName} - Email Verification",
                OtpType.TwoFactorAuthentication => $"{appName} - Two-Factor Authentication",
                _ => $"{appName} - Verification Code"
            };
        }

        private string GetEmailBody(string otpCode, OtpType type)
        {
            var appName = _configuration.GetValue<string>("AppSettings:AppName") ?? "FoodClone";
            var websiteUrl = _configuration.GetValue<string>("AppSettings:WebsiteUrl") ?? "http://localhost:4200";

            return type switch
            {
                OtpType.Registration => $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>Welcome to {appName}!</h2>
                            <p>Thank you for registering with us. To complete your registration, please use the verification code below:</p>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0;'>
                                <h1 style='color: #007bff; font-size: 32px; margin: 0; letter-spacing: 5px;'>{otpCode}</h1>
                            </div>
                            <p><strong>This code will expire in {OTP_EXPIRY_MINUTES} minutes.</strong></p>
                            <p>If you didn't request this verification, please ignore this email.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666;'>
                                This is an automated message from {appName}. Please do not reply to this email.
                            </p>
                        </div>
                    </body>
                    </html>",

                OtpType.ForgotPassword => $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>Password Reset Request</h2>
                            <p>We received a request to reset your password for your {appName} account.</p>
                            <p>Please use the verification code below to proceed with resetting your password:</p>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0;'>
                                <h1 style='color: #dc3545; font-size: 32px; margin: 0; letter-spacing: 5px;'>{otpCode}</h1>
                            </div>
                            <p><strong>This code will expire in {OTP_EXPIRY_MINUTES} minutes.</strong></p>
                            <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666;'>
                                This is an automated message from {appName}. Please do not reply to this email.
                            </p>
                        </div>
                    </body>
                    </html>",

                OtpType.ResetPassword => $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>Password Reset Verification</h2>
                            <p>You are about to reset your password for your {appName} account.</p>
                            <p>Please use the verification code below to confirm the password reset:</p>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0;'>
                                <h1 style='color: #28a745; font-size: 32px; margin: 0; letter-spacing: 5px;'>{otpCode}</h1>
                            </div>
                            <p><strong>This code will expire in {OTP_EXPIRY_MINUTES} minutes.</strong></p>
                            <p>If you didn't initiate this password reset, please contact support immediately.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666;'>
                                This is an automated message from {appName}. Please do not reply to this email.
                            </p>
                        </div>
                    </body>
                    </html>",

                _ => $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>{appName} - Verification Code</h2>
                            <p>Your verification code is:</p>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; text-align: center; margin: 20px 0;'>
                                <h1 style='color: #007bff; font-size: 32px; margin: 0; letter-spacing: 5px;'>{otpCode}</h1>
                            </div>
                            <p><strong>This code will expire in {OTP_EXPIRY_MINUTES} minutes.</strong></p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                            <p style='font-size: 12px; color: #666;'>
                                This is an automated message from {appName}. Please do not reply to this email.
                            </p>
                        </div>
                    </body>
                    </html>"
            };
        }
    }
}
