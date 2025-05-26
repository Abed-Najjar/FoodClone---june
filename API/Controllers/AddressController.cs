using API.AppResponse;
using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Address
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<AppResponse<IEnumerable<AddressDto>>>> GetMyAddresses()
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");            if (userId == 0)
            {
                return Unauthorized(new AppResponse<IEnumerable<AddressDto>>
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                });
            }

            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    StreetAddress = a.StreetAddress,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    ZipCode = a.ZipCode,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    FormattedAddress = a.FormattedAddress,
                    IsDefault = a.IsDefault,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
            return Ok(new AppResponse<IEnumerable<AddressDto>>
            {
                Success = true,
                Data = addresses
            });
        }

        // GET: api/Address/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppResponse<AddressDto>>> GetAddress(int id)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                });
            }

            var address = await _context.Addresses
                .Where(a => a.Id == id && a.UserId == userId)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    StreetAddress = a.StreetAddress,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    ZipCode = a.ZipCode,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    FormattedAddress = a.FormattedAddress,
                    IsDefault = a.IsDefault,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "Address not found"
                });
            }            return Ok(new AppResponse<AddressDto>
            {
                Success = true,
                Data = address
            });
        }

        // POST: api/Address
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AppResponse<AddressDto>>> CreateAddress(AddressCreateDto addressDto)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "User not found"
                });
            }

            // If this is set as the default address, unset any previous default
            if (addressDto.IsDefault)
            {
                var defaultAddresses = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var defaultAddress in defaultAddresses)
                {
                    defaultAddress.IsDefault = false;
                }
            }
            // If this is the first address, make it default
            else if (!await _context.Addresses.AnyAsync(a => a.UserId == userId))
            {
                addressDto.IsDefault = true;
            }

            var address = new Address
            {
                Name = addressDto.Name,
                StreetAddress = addressDto.StreetAddress,
                City = addressDto.City,
                State = addressDto.State,
                Country = addressDto.Country,
                ZipCode = addressDto.ZipCode,
                Latitude = addressDto.Latitude,
                Longitude = addressDto.Longitude,
                FormattedAddress = addressDto.FormattedAddress,
                IsDefault = addressDto.IsDefault,
                UserId = userId
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            var createdAddress = new AddressDto
            {
                Id = address.Id,
                Name = address.Name,
                StreetAddress = address.StreetAddress,
                City = address.City,
                State = address.State,
                Country = address.Country,
                ZipCode = address.ZipCode,
                Latitude = address.Latitude,
                Longitude = address.Longitude,
                FormattedAddress = address.FormattedAddress,
                IsDefault = address.IsDefault,
                CreatedAt = address.CreatedAt
            };            return CreatedAtAction(nameof(GetAddress), new { id = address.Id },
                new AppResponse<AddressDto>
                {
                    Success = true,
                    Data = createdAddress
                });
        }

        // PUT: api/Address/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<AppResponse<AddressDto>>> UpdateAddress(int id, AddressUpdateDto addressDto)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                });
            }

            var address = await _context.Addresses
                .Where(a => a.Id == id && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "Address not found"
                });
            }

            // Update properties that are not null
            if (!string.IsNullOrEmpty(addressDto.Name))
                address.Name = addressDto.Name;

            if (!string.IsNullOrEmpty(addressDto.StreetAddress))
                address.StreetAddress = addressDto.StreetAddress;

            if (!string.IsNullOrEmpty(addressDto.City))
                address.City = addressDto.City;

            if (!string.IsNullOrEmpty(addressDto.State))
                address.State = addressDto.State;

            if (!string.IsNullOrEmpty(addressDto.Country))
                address.Country = addressDto.Country;

            if (!string.IsNullOrEmpty(addressDto.ZipCode))
                address.ZipCode = addressDto.ZipCode;

            if (addressDto.Latitude.HasValue)
                address.Latitude = addressDto.Latitude.Value;

            if (addressDto.Longitude.HasValue)
                address.Longitude = addressDto.Longitude.Value;            if (!string.IsNullOrEmpty(addressDto.FormattedAddress))
                address.FormattedAddress = addressDto.FormattedAddress;

            // Handle default address logic
            if (addressDto.IsDefault.HasValue && addressDto.IsDefault.Value && !address.IsDefault)
            {
                // Unset any other default addresses
                var defaultAddresses = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault && a.Id != id)
                    .ToListAsync();

                foreach (var defaultAddress in defaultAddresses)
                {
                    defaultAddress.IsDefault = false;
                }

                address.IsDefault = true;
            }
            else if (addressDto.IsDefault.HasValue)
            {
                address.IsDefault = addressDto.IsDefault.Value;
            }

            address.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound(new AppResponse<AddressDto>
                    {
                        Success = false,
                        ErrorMessage = "Address not found"
                    });
                }
                else
                {
                    throw;
                }
            }

            var updatedAddress = new AddressDto
            {
                Id = address.Id,
                Name = address.Name,
                StreetAddress = address.StreetAddress,
                City = address.City,
                State = address.State,
                Country = address.Country,
                ZipCode = address.ZipCode,
                Latitude = address.Latitude,
                Longitude = address.Longitude,
                FormattedAddress = address.FormattedAddress,
                IsDefault = address.IsDefault,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };            return Ok(new AppResponse<AddressDto>
            {
                Success = true,
                Data = updatedAddress
            });
        }

        // DELETE: api/Address/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<AppResponse<bool>>> DeleteAddress(int id)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new AppResponse<bool>
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                });
            }

            var address = await _context.Addresses
                .Where(a => a.Id == id && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound(new AppResponse<bool>
                {
                    Success = false,
                    ErrorMessage = "Address not found"
                });
            }

            _context.Addresses.Remove(address);
            
            // If we're removing a default address and there are other addresses, make another one default
            if (address.IsDefault)
            {
                var anotherAddress = await _context.Addresses
                    .Where(a => a.UserId == userId && a.Id != id)
                    .FirstOrDefaultAsync();
                    
                if (anotherAddress != null)
                {
                    anotherAddress.IsDefault = true;
                }
            }
            
            await _context.SaveChangesAsync();            return Ok(new AppResponse<bool>
            {
                Success = true,
                Data = true
            });
        }

        // GET: api/Address/default
        [HttpGet("default")]
        [Authorize]
        public async Task<ActionResult<AppResponse<AddressDto>>> GetDefaultAddress()
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                });
            }

            var address = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    StreetAddress = a.StreetAddress,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    ZipCode = a.ZipCode,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    FormattedAddress = a.FormattedAddress,
                    IsDefault = a.IsDefault,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound(new AppResponse<AddressDto>
                {
                    Success = false,
                    ErrorMessage = "No default address found"
                });
            }            return Ok(new AppResponse<AddressDto>
            {
                Success = true,
                Data = address
            });
        }

        // PUT: api/Address/setDefault/5
        [HttpPut("setDefault/{id}")]
        [Authorize]
        public async Task<ActionResult<AppResponse<bool>>> SetDefaultAddress(int id)
        {
            // Get the current user's ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new AppResponse<bool>
                {
                    Success = false,
                    ErrorMessage = "User not authenticated"
                });
            }

            var address = await _context.Addresses
                .Where(a => a.Id == id && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (address == null)
            {
                return NotFound(new AppResponse<bool>
                {
                    Success = false,
                    ErrorMessage = "Address not found"
                });
            }

            // Unset any other default addresses
            var defaultAddresses = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && a.Id != id)
                .ToListAsync();

            foreach (var defaultAddress in defaultAddresses)
            {
                defaultAddress.IsDefault = false;
            }

            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();            return Ok(new AppResponse<bool>
            {
                Success = true,
                Data = true
            });
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }
    }
}
