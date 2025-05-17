using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
    }
}