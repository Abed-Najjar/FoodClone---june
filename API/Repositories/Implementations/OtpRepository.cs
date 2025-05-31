using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class OtpRepository : IOtpRepository
    {
        private readonly AppDbContext _context;

        public OtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Otp?> GetValidOtpAsync(string email, string code, OtpType type)
        {
            return await _context.Otps
                .Where(o => o.Email == email && 
                           o.Code == code && 
                           o.Type == type && 
                           !o.IsUsed && 
                           o.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Otp?> GetLatestOtpAsync(string email, OtpType type)
        {
            return await _context.Otps
                .Where(o => o.Email == email && o.Type == type)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Otp> CreateOtpAsync(Otp otp)
        {
            _context.Otps.Add(otp);
            await _context.SaveChangesAsync();
            return otp;
        }

        public async Task<bool> InvalidateOtpAsync(int otpId)
        {
            var otp = await _context.Otps.FindAsync(otpId);
            if (otp == null) return false;

            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InvalidateAllUserOtpsAsync(string email, OtpType type)
        {
            var otps = await _context.Otps
                .Where(o => o.Email == email && o.Type == type && !o.IsUsed)
                .ToListAsync();

            foreach (var otp in otps)
            {
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteExpiredOtpsAsync()
        {
            var expiredOtps = await _context.Otps
                .Where(o => o.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            _context.Otps.RemoveRange(expiredOtps);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetOtpAttemptsCountAsync(string email, OtpType type, TimeSpan timeWindow)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(timeWindow);
            return await _context.Otps
                .Where(o => o.Email == email && 
                           o.Type == type && 
                           o.CreatedAt >= cutoffTime)
                .CountAsync();
        }
    }
}
