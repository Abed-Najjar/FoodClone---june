using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using API.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }



        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> FindAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }        public Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return Task.FromResult(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Restaurants)
                .Include(u => u.OrdersPlaced)
                .Include(u => u.OrdersDelivered)
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (user == null)
            {
                return false;
            }

            // Check if user has any orders as customer or employee
            if (user.OrdersPlaced.Any() || user.OrdersDelivered.Any())
            {
                throw new InvalidOperationException($"Cannot delete user {user.FirstName} {user.LastName} because they have associated orders. Consider deactivating the user instead.");
            }

            // Remove associated OTP records
            var otpRecords = await _context.Otps.Where(o => o.UserId == id).ToListAsync();
            if (otpRecords.Any())
            {
                _context.Otps.RemoveRange(otpRecords);
            }

            // Remove associated addresses
            if (user.Addresses.Any())
            {
                _context.Addresses.RemoveRange(user.Addresses);
            }

            // Clear restaurant relationships (many-to-many)
            user.Restaurants.Clear();
            
            // Now remove the user
            _context.Users.Remove(user);
            return true;
        }

        public async Task<List<User>> GetUsersByRoleAsync(Roles role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllUsersExceptRoleAsync(Roles excludeRole)
        {
            return await _context.Users
                .Where(u => u.Role != excludeRole)
                .ToListAsync();
        }
    }
}