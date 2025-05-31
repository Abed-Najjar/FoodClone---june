using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IOtpRepository
    {
        Task<Otp?> GetValidOtpAsync(string email, string code, OtpType type);
        Task<Otp?> GetLatestOtpAsync(string email, OtpType type);
        Task<Otp> CreateOtpAsync(Otp otp);
        Task<bool> InvalidateOtpAsync(int otpId);
        Task<bool> InvalidateAllUserOtpsAsync(string email, OtpType type);
        Task<bool> DeleteExpiredOtpsAsync();
        Task<int> GetOtpAttemptsCountAsync(string email, OtpType type, TimeSpan timeWindow);
    }
}
