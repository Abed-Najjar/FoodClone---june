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
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            
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