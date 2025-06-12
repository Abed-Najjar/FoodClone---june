using API.AppResponse;
using API.DTOs;

namespace API.Services.AddressServiceFolder
{
    public interface IAddressService
    {
        Task<AppResponse<IEnumerable<AddressDto>>> GetMyAddressesAsync(int userId);
        Task<AppResponse<AddressDto>> GetAddressAsync(int id, int userId);
        Task<AppResponse<AddressDto>> CreateAddressAsync(AddressCreateDto addressDto, int userId);
        Task<AppResponse<AddressDto>> UpdateAddressAsync(int id, AddressUpdateDto addressDto, int userId);
        Task<AppResponse<bool>> DeleteAddressAsync(int id, int userId);
        Task<AppResponse<AddressDto>> GetDefaultAddressAsync(int userId);
        Task<AppResponse<bool>> SetDefaultAddressAsync(int id, int userId);
    }
} 