using API.AppResponse;
using API.DTOs;
using API.Models;
using API.UoW;

public class DishManagementService : IDishManagementService
{
    private readonly IUnitOfWork _unitOfWork;

    public DishManagementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AppResponse<AdminRestaurantDishDto>> CreateDish(CreateDishDto dishDto)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.GetRestaurantByIdAsync(dishDto.RestaurantId);
            if (restaurant == null)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Restaurant not found", 404, false);
            }

            if (dishDto.CategoryId.HasValue)
            {
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(dishDto.CategoryId.Value);
                if (category == null)
                {
                    return new AppResponse<AdminRestaurantDishDto>(null, "Category not found", 404, false);
                }
            }

            var dish = new Dish
            {
                Name = dishDto.Name,
                Description = dishDto.Description,
                Price = dishDto.Price,
                ImageUrl = dishDto.ImageUrl,
                RestaurantId = dishDto.RestaurantId,
                Restaurant = restaurant,
                CategoryId = dishDto.CategoryId,
                IsAvailable = dishDto.IsAvailable
            };
            
            await _unitOfWork.DishRepository.AddDishAsync(dish);
            await _unitOfWork.CompleteAsync();

            var responseDto = new AdminRestaurantDishDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                ImageUrl = dish.ImageUrl,
                RestaurantId = dish.RestaurantId,
                RestaurantName = restaurant.Name,
                CategoryId = dish.CategoryId,
                CategoryName = dish.Category?.Name,
                IsAvailable = dish.IsAvailable
            };

            return new AppResponse<AdminRestaurantDishDto>(responseDto, "Dish created successfully", 201, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDishDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<bool>> DeleteDish(int id)
    {
        try
        {
            var dish = await _unitOfWork.DishRepository.GetDishByIdAsync(id);
            if (dish == null)
            {
                return new AppResponse<bool>(false, "Dish not found", 404, false);
            }
            
            await _unitOfWork.DishRepository.DeleteDishAsync(dish.Id);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>(true, "Dish deleted successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<bool>(false, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDishDto>> GetDishById(int id)
    {
        try
        {
            var dish = await _unitOfWork.DishRepository.GetDishByIdWithIncludesAsync(id);

            if (dish == null)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Dish not found", 404, false);
            }
            
            var dishDto = new AdminRestaurantDishDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                ImageUrl = dish.ImageUrl,
                RestaurantId = dish.RestaurantId,
                RestaurantName = dish.Restaurant.Name,
                CategoryId = dish.CategoryId,
                CategoryName = dish.Category?.Name,
                IsAvailable = dish.IsAvailable
            };

            return new AppResponse<AdminRestaurantDishDto>(dishDto, "Dish retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDishDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetDishesInRestaurant(int restaurantId, PaginationDto? paginationDto = null)
    {
        try
        {
            Console.WriteLine($"[DEBUG] Querying dishes for restaurant ID: {restaurantId}");
            var dishes = await _unitOfWork.DishRepository.GetDishesByRestaurantIdAsync(restaurantId);
            Console.WriteLine($"[DEBUG] Raw dishes found: {dishes.Count}");

            var dishDtos = dishes.Select(d => new AdminRestaurantDishDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                RestaurantId = d.RestaurantId,
                RestaurantName = d.Restaurant.Name,
                CategoryId = d.CategoryId,
                CategoryName = d.Category?.Name,
                IsAvailable = d.IsAvailable
            }).ToList();
            
            Console.WriteLine($"[DEBUG] Processed dish DTOs: {dishDtos.Count}");

            // Always return paginated result
            var totalItems = dishDtos.Count;
            var pageNumber = paginationDto?.PageNumber ?? 1;
            var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
            
            var paginatedData = dishDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResultDto<AdminRestaurantDishDto>(paginatedData, totalItems, pageNumber, pageSize);
            return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(pagedResult, "Dishes retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetDishesByCategory(int categoryId, PaginationDto? paginationDto = null)
    {
        try
        {
            var dishes = await _unitOfWork.DishRepository.GetDishesByCategoryIdAsync(categoryId);

            var dishDtos = dishes.Select(d => new AdminRestaurantDishDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                RestaurantId = d.RestaurantId,
                RestaurantName = d.Restaurant.Name,
                CategoryId = d.CategoryId,
                CategoryName = d.Category?.Name,
                IsAvailable = d.IsAvailable
            }).ToList();

            // Always return paginated result
            var totalItems = dishDtos.Count;
            var pageNumber = paginationDto?.PageNumber ?? 1;
            var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
            
            var paginatedData = dishDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResultDto<AdminRestaurantDishDto>(paginatedData, totalItems, pageNumber, pageSize);
            return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(pagedResult, "Dishes retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<AdminRestaurantDishDto>> UpdateDish(int id, UpdateDishDto dishDto)
    {
        try
        {
            var dish = await _unitOfWork.DishRepository.GetDishByIdWithIncludesAsync(id);

            if (dish == null)
            {
                return new AppResponse<AdminRestaurantDishDto>(null, "Dish not found", 404, false);
            }

            // Validate restaurant exists if restaurantId is being changed
            if (dishDto.RestaurantId != dish.RestaurantId)
            {
                var restaurant = await _unitOfWork.RestaurantRepository.GetRestaurantByIdAsync(dishDto.RestaurantId);
                if (restaurant == null)
                {
                    return new AppResponse<AdminRestaurantDishDto>(null, "Restaurant not found", 404, false);
                }
                dish.RestaurantId = dishDto.RestaurantId;
            }

            if (dishDto.Name != null) dish.Name = dishDto.Name;
            if (dishDto.Description != null) dish.Description = dishDto.Description;
            dish.Price = dishDto.Price;
            if (dishDto.ImageUrl != null) dish.ImageUrl = dishDto.ImageUrl;
            dish.IsAvailable = dishDto.IsAvailable;
            
            if (dishDto.CategoryId.HasValue)
            {
                var category = await _unitOfWork.CategoryRepository.GetCategoryByIdAsync(dishDto.CategoryId.Value);
                if (category == null)
                {
                    return new AppResponse<AdminRestaurantDishDto>(null, "Category not found", 404, false);
                }
                dish.CategoryId = dishDto.CategoryId;
            }

            await _unitOfWork.CompleteAsync();

            var responseDto = new AdminRestaurantDishDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Description = dish.Description,
                Price = dish.Price,
                ImageUrl = dish.ImageUrl,
                RestaurantId = dish.RestaurantId,
                RestaurantName = dish.Restaurant.Name,
                CategoryId = dish.CategoryId,
                CategoryName = dish.Category?.Name,
                IsAvailable = dish.IsAvailable
            };

            return new AppResponse<AdminRestaurantDishDto>(responseDto, "Dish updated successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<AdminRestaurantDishDto>(null, ex.Message, 500, false);
        }
    }
    
    public async Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetAllDishesAsync(PaginationDto? paginationDto = null)
    {
        try
        {
            var dishes = await _unitOfWork.DishRepository.GetAllDishesWithIncludesAsync();
            
            var dishDtos = dishes.Select(d => new AdminRestaurantDishDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                RestaurantId = d.RestaurantId,
                RestaurantName = d.Restaurant.Name,
                CategoryId = d.CategoryId,
                CategoryName = d.Category?.Name,
                IsAvailable = d.IsAvailable
            }).ToList();

            // Always return paginated result
            var totalItems = dishDtos.Count;
            var pageNumber = paginationDto?.PageNumber ?? 1;
            var pageSize = paginationDto?.PageSize ?? totalItems; // If no pagination, return all items
            
            var paginatedData = dishDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PagedResultDto<AdminRestaurantDishDto>(paginatedData, totalItems, pageNumber, pageSize);
            return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(pagedResult, "Dishes retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(null, ex.Message, 500, false);
        }
    }
}



