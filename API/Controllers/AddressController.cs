using API.AppResponse;
using API.DTOs;
using API.Services.AddressServiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // GET: api/Address
        [HttpGet]
        [Authorize]
        public async Task<AppResponse<IEnumerable<AddressDto>>> GetMyAddresses()
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");            
            
            return await _addressService.GetMyAddressesAsync(userId);
        }

        // GET: api/Address/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<AppResponse<AddressDto>> GetAddress(int id)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _addressService.GetAddressAsync(id, userId);
        }

        // POST: api/Address
        [HttpPost]
        [Authorize]
        public async Task<AppResponse<AddressDto>> CreateAddress(AddressCreateDto addressDto)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _addressService.CreateAddressAsync(addressDto, userId);
        }

        // PUT: api/Address/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<AppResponse<AddressDto>> UpdateAddress(int id, AddressUpdateDto addressDto)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _addressService.UpdateAddressAsync(id, addressDto, userId);
        }

        // DELETE: api/Address/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<AppResponse<bool>> DeleteAddress(int id)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _addressService.DeleteAddressAsync(id, userId);
        }

        // GET: api/Address/default
        [HttpGet("default")]
        [Authorize]
        public async Task<AppResponse<AddressDto>> GetDefaultAddress()
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _addressService.GetDefaultAddressAsync(userId);
        }

        // PUT: api/Address/setDefault/5
        [HttpPut("setDefault/{id}")]
        [Authorize]
        public async Task<AppResponse<bool>> SetDefaultAddress(int id)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            return await _addressService.SetDefaultAddressAsync(id, userId);
        }
    }
}
