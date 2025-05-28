using API.AppResponse;
using API.DTOs;
using API.Models;
using API.UoW;




public class CategoryManagementService : ICategoryManagementService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryManagementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AppResponse<List<CategoriesDto>>> GetAllCategoriesAsync()
    {
        try
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllCategoriesAsync();

            if (!categories.Any())
            {
                return new AppResponse<List<CategoriesDto>>(null, "No categories found", 404, false);
            }

            var categoriesDto = categories.Select(c => new CategoriesDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl
            }).ToList();

            return new AppResponse<List<CategoriesDto>>(categoriesDto, "Categories retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<List<CategoriesDto>>(null, ex.Message, 500, false);
        }
    }    public async Task<AppResponse<CategoryDto>> CreateCategory(CreateCategoryDto categoryDto)
    {
        try
        {
            var restaurant = await _unitOfWork.RestaurantRepository.GetRestaurantByIdAsync(categoryDto.RestaurantId);
            if (restaurant == null)
            {
                return new AppResponse<CategoryDto>(null, "Restaurant not found", 404, false);
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                ImageUrl = categoryDto.ImageUrl
            };

            await _unitOfWork.CategoryRepository.CreateCategoryAsync(category);
            
            var restaurantCategory = new RestaurantsCategories
            {
                RestaurantId = categoryDto.RestaurantId,
                CategoryId = category.Id,
                Restaurant = restaurant,
                Category = category
            };

            await _unitOfWork.CategoryRepository.CreateRestaurantCategoryAsync(restaurantCategory);
            await _unitOfWork.CompleteAsync();

            var responseDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                RestaurantId = restaurant.Id,
                RestaurantName = restaurant.Name
            };

            return new AppResponse<CategoryDto>(responseDto, "Category created successfully", 201, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<bool>> DeleteCategory(int id)
    {
        try
        {
            var category = await _unitOfWork.CategoryRepository.FindAsync(id);
            if (category == null)
            {
                return new AppResponse<bool>(false, "Category not found", 404, false);
            }
            
            var restaurantCategories = await _unitOfWork.CategoryRepository.GetRestaurantCategoriesByCategoryIdAsync(id);
            
            if (restaurantCategories.Any())
            {
                await _unitOfWork.CategoryRepository.DeleteRestaurantCategoriesAsync(restaurantCategories);
            }

            await _unitOfWork.CategoryRepository.DeleteCategoryAsync(id);
            await _unitOfWork.CompleteAsync();

            return new AppResponse<bool>(true, "Category deleted successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<bool>(false, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<List<CategoryDto>>> GetCategories(int restaurantId)
    {
        try
        {
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesByRestaurantAsync(restaurantId);

            if (!categories.Any())
            {
                return new AppResponse<List<CategoryDto>>(null, "No categories found for this restaurant", 404, false);
            }

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                RestaurantId = restaurantId,
                RestaurantName = "", // Will be populated from context if needed
                Dishes = c.Dishes?.Select(d => new AdminRestaurantDishDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    ImageUrl = d.ImageUrl,
                    RestaurantId = d.RestaurantId,
                    RestaurantName = d.Restaurant?.Name ?? string.Empty,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category?.Name ?? string.Empty,
                    IsAvailable = d.IsAvailable
                }).ToList() ?? new List<AdminRestaurantDishDto>()
            }).ToList();

            return new AppResponse<List<CategoryDto>>(categoryDtos, "Categories retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<List<CategoryDto>>(null, ex.Message, 500, false);
        }
    }
    
    public async Task<AppResponse<CategoryDto>> GetCategoriesByRestaurantId(int restaurantId)
    {
        try
        {
            var categories = await _unitOfWork.CategoryRepository.GetCategoriesByRestaurantAsync(restaurantId);

            if (!categories.Any())
            {
                return new AppResponse<CategoryDto>(null, "No categories found for this restaurant", 404, false);
            }

            var categoryDto = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                RestaurantId = restaurantId,
                RestaurantName = ""
            }).ToList();

            return new AppResponse<CategoryDto>(categoryDto.FirstOrDefault(), "Categories retrieved successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<CategoryDto>> GetCategoryById(int id)
    {
        try
        {
            var category = await _unitOfWork.CategoryRepository.FindAsync(id);
            if (category == null)
            {
                return new AppResponse<CategoryDto>(null, "Category not found", 404, false);
            }

            var restaurantCategories = await _unitOfWork.CategoryRepository.GetRestaurantCategoriesByCategoryIdAsync(id);
            var restaurantCategory = restaurantCategories.FirstOrDefault();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                RestaurantId = restaurantCategory?.RestaurantId ?? 0,
                RestaurantName = ""
            };

            return new AppResponse<CategoryDto>(categoryDto, "Category retrieved successfully", 200, true); 
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
        }
    }

    public async Task<AppResponse<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto categoryDto)
    {
        try
        {
            var category = await _unitOfWork.CategoryRepository.FindAsync(id);

            if (category == null)
            {
                return new AppResponse<CategoryDto>(null, "Category not found", 404, false);
            }

            if (categoryDto.Name != null) category.Name = categoryDto.Name;
            if (categoryDto.Description != null) category.Description = categoryDto.Description;
            if (categoryDto.ImageUrl != null) category.ImageUrl = categoryDto.ImageUrl;

            await _unitOfWork.CategoryRepository.UpdateCategoryAsync(category);

            var restaurantCategories = await _unitOfWork.CategoryRepository.GetRestaurantCategoriesByCategoryIdAsync(id);
            var restaurantCategory = restaurantCategories.FirstOrDefault();

            await _unitOfWork.CompleteAsync();

            var responseDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                RestaurantId = restaurantCategory?.RestaurantId ?? 0,
                RestaurantName = ""
            };

            return new AppResponse<CategoryDto>(responseDto, "Category updated successfully", 200, true);
        }
        catch (Exception ex)
        {
            return new AppResponse<CategoryDto>(null, ex.Message, 500, false);
        }
    }
}
