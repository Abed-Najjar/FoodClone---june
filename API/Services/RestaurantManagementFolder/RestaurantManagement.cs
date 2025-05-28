using API.AppResponse;
using API.DTOs;
using API.Models;
using API.UoW;
using Microsoft.AspNetCore.Mvc;

public class RestaurantManagement : IRestaurantManagement
{
    private readonly IUnitOfWork _unitOfWork;

    public RestaurantManagement(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AppResponse<AdminRestaurantDto>> CreateRestaurant([FromBody] RestaurantCreateDto dto)
    {
        try
        {
            if (dto == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Invalid data", 400, false);
            }

            var restaurant = new Restaurant
            {
                Name = dto.Name,
                Description = dto.Description,
                LogoUrl = dto.LogoUrl,
                CoverImageUrl = dto.CoverImageUrl,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                OpeningHours = dto.OpeningHours,
            };

            await _unitOfWork.RestaurantRepository.CreateRestaurantAsync(restaurant);
            await _unitOfWork.CompleteAsync();

            var AdminRestaurantDto = new AdminRestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                LogoUrl = restaurant.LogoUrl,
                CoverImageUrl = restaurant.CoverImageUrl,
                Categories = restaurant.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                Address = restaurant.Address,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                OpeningHours = restaurant.OpeningHours,
                Rating = restaurant.Rating,
                ReviewCount = restaurant.ReviewCount,
                IsOpen = restaurant.IsOpen,
                DeliveryFee = restaurant.DeliveryFee,
                Suspended = restaurant.IsSupended
            };

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant created successfully", 201, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDto>> DeleteRestaurant(int id)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.GetRestaurantWithCategoriesAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            // Create the DTO before removing the restaurant
            var AdminRestaurantDto = new AdminRestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                LogoUrl = restaurant.LogoUrl,
                CoverImageUrl = restaurant.CoverImageUrl,
                Categories = restaurant.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                Address = restaurant.Address,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                OpeningHours = restaurant.OpeningHours,
                Rating = restaurant.Rating,
                ReviewCount = restaurant.ReviewCount,
                IsOpen = restaurant.IsOpen,
                DeliveryFee = restaurant.DeliveryFee,
                Suspended = restaurant.IsSupended
            };

            await _unitOfWork.RestaurantRepository.DeleteRestaurantAsync(id);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant deleted successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<List<AdminRestaurantDto>>> GetAllRestaurants()
    {
        try
        {
            var restaurants = await _unitOfWork.RestaurantRepository.GetAllRestaurantsWithCategoriesAsync();

            if (restaurants == null || !restaurants.Any())
            {
                return new AppResponse<List<AdminRestaurantDto>>(null, "No restaurants found", 404, false);
            }

            var AdminRestaurantDtos = restaurants.Select(restaurant => new AdminRestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                LogoUrl = restaurant.LogoUrl,
                CoverImageUrl = restaurant.CoverImageUrl,
                Categories = restaurant.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                Address = restaurant.Address,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                OpeningHours = restaurant.OpeningHours,
                Rating = restaurant.Rating,
                ReviewCount = restaurant.ReviewCount,
                IsOpen = restaurant.IsOpen,
                DeliveryFee = restaurant.DeliveryFee,
                Suspended = restaurant.IsSupended
            }).ToList();

            return new AppResponse<List<AdminRestaurantDto>>(AdminRestaurantDtos, "Restaurants retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<List<AdminRestaurantDto>>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDto>> GetRestaurant(int id)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.GetRestaurantWithCategoriesAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            var AdminRestaurantDto = new AdminRestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                LogoUrl = restaurant.LogoUrl,
                CoverImageUrl = restaurant.CoverImageUrl,
                Categories = restaurant.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                Address = restaurant.Address,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                OpeningHours = restaurant.OpeningHours,
                Rating = restaurant.Rating,
                ReviewCount = restaurant.ReviewCount,
                IsOpen = restaurant.IsOpen,
                DeliveryFee = restaurant.DeliveryFee,
                Suspended = restaurant.IsSupended
            };

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<bool>> SuspendRestaurant(int id)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.FindAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<bool>(false, "Restaurant not found", 404, false);
            }

            restaurant.IsSupended = true;
            await _unitOfWork.RestaurantRepository.UpdateRestaurantAsync(restaurant);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>(true, "Restaurant suspended successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<bool>(false, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<bool>> UnsuspendRestaurant(int id)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.FindAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<bool>(false, "Restaurant not found", 404, false);
            }

            restaurant.IsSupended = false;
            await _unitOfWork.RestaurantRepository.UpdateRestaurantAsync(restaurant);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>(true, "Restaurant unsuspended successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<bool>(false, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDto>> UpdateRestaurant(int id, RestaurantUpdateDto dto)
    {
        try
        {
            if (dto == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Invalid data", 400, false);
            }
            
            var restaurant = await _unitOfWork.RestaurantRepository.GetRestaurantWithCategoriesAndDishesAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }
            
            restaurant.Name = dto.Name ?? restaurant.Name;
            restaurant.Description = dto.Description ?? restaurant.Description;
            restaurant.LogoUrl = dto.LogoUrl ?? restaurant.LogoUrl;
            restaurant.CoverImageUrl = dto.CoverImageUrl ?? restaurant.CoverImageUrl;
            restaurant.Address = dto.Address ?? restaurant.Address;
            restaurant.PhoneNumber = dto.PhoneNumber ?? restaurant.PhoneNumber;
            restaurant.Email = dto.Email ?? restaurant.Email;
            restaurant.OpeningHours = dto.OpeningHours ?? restaurant.OpeningHours;
            
            // Update IsOpen if provided in DTO
            if (dto.IsOpen.HasValue)
            {
                restaurant.IsOpen = dto.IsOpen.Value;
            }
            
            restaurant.IsSupended = dto.IsSuspended;

            await _unitOfWork.RestaurantRepository.UpdateRestaurantAsync(restaurant);
            await _unitOfWork.CompleteAsync();

            var AdminRestaurantDto = new AdminRestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                LogoUrl = restaurant.LogoUrl,
                CoverImageUrl = restaurant.CoverImageUrl,
                Categories = restaurant.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                Address = restaurant.Address,
                PhoneNumber = restaurant.PhoneNumber,
                Email = restaurant.Email,
                OpeningHours = restaurant.OpeningHours,
                Rating = restaurant.Rating,
                ReviewCount = restaurant.ReviewCount,
                IsOpen = restaurant.IsOpen,
                DeliveryFee = restaurant.DeliveryFee,
                Suspended = restaurant.IsSupended
            };

            return new AppResponse<AdminRestaurantDto>(AdminRestaurantDto, "Restaurant updated successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<List<int>>> GetCategoryRestaurants(int categoryId)
    {
        try
        {
            var categoryRestaurants = await _unitOfWork.RestaurantRepository.GetCategoryRestaurantsAsync(categoryId);

            if (categoryRestaurants == null || !categoryRestaurants.Any())
            {
                return new AppResponse<List<int>>(null, "No restaurants found for the category", 404, false);
            }

            return new AppResponse<List<int>>(categoryRestaurants, "Category restaurants retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<List<int>>(null, ex.Message, 500, false);
        }
    }
    
    public async Task<AppResponse<AdminRestaurantDto>> OpenRestaurant(int id)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.FindAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            restaurant.IsOpen = true;
            await _unitOfWork.RestaurantRepository.UpdateRestaurantAsync(restaurant);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<AdminRestaurantDto>(new AdminRestaurantDto { Id = restaurant.Id }, "Restaurant opened successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDto>> CloseRestaurant(int id)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.FindAsync(id);

            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDto>(null, "Restaurant not found", 404, false);
            }

            restaurant.IsOpen = false;
            await _unitOfWork.RestaurantRepository.UpdateRestaurantAsync(restaurant);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<AdminRestaurantDto>(new AdminRestaurantDto { Id = restaurant.Id }, "Restaurant closed successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDto>(null, ex.Message, 500, false);
        }
    }
}

