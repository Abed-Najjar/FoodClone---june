using API.Models;
using API.Enums;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> FindAsync(int id);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<List<User>> GetUsersByRoleAsync(Roles role);
        Task<List<User>> GetAllUsersExceptRoleAsync(Roles excludeRole);
    }
}