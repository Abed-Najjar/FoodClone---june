using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(int id);
        Task<List<Address>> GetUserAddressesAsync(int userId);
        Task<Address> AddAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task<bool> DeleteAsync(int id);
    }
}
