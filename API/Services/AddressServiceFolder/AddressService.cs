using API.AppResponse;
using API.DTOs;
using API.Models;
using API.UoW;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Services.AddressServiceFolder
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddressService> _logger;

        public AddressService(IUnitOfWork unitOfWork, ILogger<AddressService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AppResponse<IEnumerable<AddressDto>>> GetMyAddressesAsync(int userId)
        {
            try
            {
                _logger.LogInformation($"GetMyAddressesAsync called for user ID: {userId}");

                if (userId == 0)
                {
                    return new AppResponse<IEnumerable<AddressDto>>(null, "User not authenticated", 401, false);
                }

                var addresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(userId);
                
                var addressDtos = addresses.Select(a => new AddressDto
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
                }).ToList();

                return new AppResponse<IEnumerable<AddressDto>>(addressDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting addresses for user ID: {userId}");
                return new AppResponse<IEnumerable<AddressDto>>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<AddressDto>> GetAddressAsync(int id, int userId)
        {
            try
            {
                _logger.LogInformation($"GetAddressAsync called for address ID: {id}, user ID: {userId}");

                if (userId == 0)
                {
                    return new AppResponse<AddressDto>(null, "User not authenticated", 401, false);
                }

                var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);

                if (address == null || address.UserId != userId)
                {
                    return new AppResponse<AddressDto>(null, "Address not found", 404, false);
                }

                var addressDto = new AddressDto
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
                };

                return new AppResponse<AddressDto>(addressDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting address ID: {id} for user ID: {userId}");
                return new AppResponse<AddressDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<AddressDto>> CreateAddressAsync(AddressCreateDto addressDto, int userId)
        {
            try
            {
                _logger.LogInformation($"CreateAddressAsync called for user ID: {userId}");

                if (userId == 0)
                {
                    return new AppResponse<AddressDto>(null, "User not authenticated", 401, false);
                }

                var user = await _unitOfWork.UserRepository.FindAsync(userId);
                if (user == null)
                {
                    return new AppResponse<AddressDto>(null, "User not found", 404, false);
                }

                // If this is set as the default address, unset any previous default
                if (addressDto.IsDefault)
                {
                    var defaultAddresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(userId);
                    var currentDefaultAddresses = defaultAddresses.Where(a => a.IsDefault).ToList();

                    foreach (var defaultAddress in currentDefaultAddresses)
                    {
                        defaultAddress.IsDefault = false;
                        await _unitOfWork.AddressRepository.UpdateAsync(defaultAddress);
                    }
                }
                // If this is the first address, make it default
                else
                {
                    var existingAddresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(userId);
                    if (!existingAddresses.Any())
                    {
                        addressDto.IsDefault = true;
                    }
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

                var createdAddress = await _unitOfWork.AddressRepository.AddAsync(address);
                await _unitOfWork.CompleteAsync();

                var createdAddressDto = new AddressDto
                {
                    Id = createdAddress.Id,
                    Name = createdAddress.Name,
                    StreetAddress = createdAddress.StreetAddress,
                    City = createdAddress.City,
                    State = createdAddress.State,
                    Country = createdAddress.Country,
                    ZipCode = createdAddress.ZipCode,
                    Latitude = createdAddress.Latitude,
                    Longitude = createdAddress.Longitude,
                    FormattedAddress = createdAddress.FormattedAddress,
                    IsDefault = createdAddress.IsDefault,
                    CreatedAt = createdAddress.CreatedAt
                };

                return new AppResponse<AddressDto>(createdAddressDto, "Address created successfully", 201, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating address for user ID: {userId}");
                return new AppResponse<AddressDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<AddressDto>> UpdateAddressAsync(int id, AddressUpdateDto addressDto, int userId)
        {
            try
            {
                _logger.LogInformation($"UpdateAddressAsync called for address ID: {id}, user ID: {userId}");

                if (userId == 0)
                {
                    return new AppResponse<AddressDto>(null, "User not authenticated", 401, false);
                }

                var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);

                if (address == null || address.UserId != userId)
                {
                    return new AppResponse<AddressDto>(null, "Address not found", 404, false);
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
                    address.Longitude = addressDto.Longitude.Value;

                if (!string.IsNullOrEmpty(addressDto.FormattedAddress))
                    address.FormattedAddress = addressDto.FormattedAddress;

                // Handle default address logic
                if (addressDto.IsDefault.HasValue && addressDto.IsDefault.Value && !address.IsDefault)
                {
                    // Unset any other default addresses
                    var defaultAddresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(userId);
                    var currentDefaultAddresses = defaultAddresses.Where(a => a.IsDefault && a.Id != id).ToList();

                    foreach (var defaultAddress in currentDefaultAddresses)
                    {
                        defaultAddress.IsDefault = false;
                        await _unitOfWork.AddressRepository.UpdateAsync(defaultAddress);
                    }

                    address.IsDefault = true;
                }
                else if (addressDto.IsDefault.HasValue)
                {
                    address.IsDefault = addressDto.IsDefault.Value;
                }

                address.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.AddressRepository.UpdateAsync(address);
                await _unitOfWork.CompleteAsync();

                var updatedAddressDto = new AddressDto
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
                };

                return new AppResponse<AddressDto>(updatedAddressDto, "Address updated successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating address ID: {id} for user ID: {userId}");
                return new AppResponse<AddressDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<bool>> DeleteAddressAsync(int id, int userId)
        {
            try
            {
                _logger.LogInformation($"DeleteAddressAsync called for address ID: {id}, user ID: {userId}");

                if (userId == 0)
                {
                    return new AppResponse<bool>(false, "User not authenticated", 401, false);
                }

                var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);

                if (address == null || address.UserId != userId)
                {
                    return new AppResponse<bool>(false, "Address not found", 404, false);
                }

                // If we're removing a default address and there are other addresses, make another one default
                if (address.IsDefault)
                {
                    var userAddresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(userId);
                    var anotherAddress = userAddresses.FirstOrDefault(a => a.Id != id);
                    
                    if (anotherAddress != null)
                    {
                        anotherAddress.IsDefault = true;
                        await _unitOfWork.AddressRepository.UpdateAsync(anotherAddress);
                    }
                }

                await _unitOfWork.AddressRepository.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();

                return new AppResponse<bool>(true, "Address deleted successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting address ID: {id} for user ID: {userId}");
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<AddressDto>> GetDefaultAddressAsync(int userId)
        {
            try
            {
                _logger.LogInformation($"GetDefaultAddressAsync called for user ID: {userId}");

                if (userId == 0)
                {
                    return new AppResponse<AddressDto>(null, "User not authenticated", 401, false);
                }

                var addresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(userId);
                var address = addresses.FirstOrDefault(a => a.IsDefault);

                if (address == null)
                {
                    return new AppResponse<AddressDto>(null, "No default address found", 404, false);
                }

                var addressDto = new AddressDto
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
                };

                return new AppResponse<AddressDto>(addressDto, "Default address retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting default address for user ID: {userId}");
                return new AppResponse<AddressDto>(null, ex.Message, 500, false);
            }
        }

        public async Task<AppResponse<bool>> SetDefaultAddressAsync(int id, int userId)
        {
            try
            {
                _logger.LogInformation($"SetDefaultAddressAsync called for address ID: {id}, user ID: {userId}");

                if (userId == 0)
                {
                    return new AppResponse<bool>(false, "User not authenticated", 401, false);
                }

                var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);

                if (address == null || address.UserId != userId)
                {
                    return new AppResponse<bool>(false, "Address not found", 404, false);
                }

                // Unset any other default addresses
                var defaultAddresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(userId);
                var currentDefaultAddresses = defaultAddresses.Where(a => a.IsDefault && a.Id != id).ToList();

                foreach (var defaultAddress in currentDefaultAddresses)
                {
                    defaultAddress.IsDefault = false;
                    await _unitOfWork.AddressRepository.UpdateAsync(defaultAddress);
                }

                address.IsDefault = true;
                address.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.AddressRepository.UpdateAsync(address);
                await _unitOfWork.CompleteAsync();

                return new AppResponse<bool>(true, "Default address set successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while setting default address ID: {id} for user ID: {userId}");
                return new AppResponse<bool>(false, ex.Message, 500, false);
            }
        }
    }
} 