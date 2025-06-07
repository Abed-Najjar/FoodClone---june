using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class HomeController : ControllerBase    {
        private readonly IRestaurantManagement _restaurantManagementService;
        private readonly IDishManagementService _dishManagementService;
        private readonly ICategoryManagementService _categoryManagementService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IRestaurantManagement restaurantManagementService,
            IDishManagementService dishManagementService,
            ICategoryManagementService categoryManagementService,
            ILogger<HomeController> logger)
        {
            _restaurantManagementService = restaurantManagementService;
            _dishManagementService = dishManagementService;
            _categoryManagementService = categoryManagementService;
            _logger = logger;
        }

        /// <summary>
        /// Get all restaurants for the home page
        /// </summary>
        /// <returns>List of restaurants</returns>
        [HttpGet("restaurants")]
        public async Task<AppResponse<PagedResultDto<AdminRestaurantDto>>> GetAllRestaurants([FromQuery] PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation("Fetching all restaurants for home page");
                return await _restaurantManagementService.GetAllRestaurants(paginationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching restaurants for home page");
                return new AppResponse<PagedResultDto<AdminRestaurantDto>>(null, "Error fetching restaurants", 500, false);
            }
        }

        /// <summary>
        /// Get all dishes for the home page
        /// </summary>
        /// <returns>List of dishes</returns>
        [HttpGet("dishes")]
        public async Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetAllDishes([FromQuery] PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation("Fetching all dishes for home page");
                return await _dishManagementService.GetAllDishesAsync(paginationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dishes for home page");
                return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(null, "Error fetching dishes", 500, false);
            }
        }

        /// <summary>
        /// Get featured/popular restaurants (first 6 restaurants)
        /// </summary>
        /// <returns>List of featured restaurants</returns>
        [HttpGet("featured-restaurants")]
        public async Task<AppResponse<PagedResultDto<AdminRestaurantDto>>> GetFeaturedRestaurants([FromQuery] PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation("Fetching featured restaurants for home page");
                
                // If pagination is not provided, use default pagination for featured restaurants (6 items)
                var defaultPagination = paginationDto ?? new PaginationDto { PageNumber = 1, PageSize = 6 };
                
                var allRestaurantsResponse = await _restaurantManagementService.GetAllRestaurants(defaultPagination);
                
                if (!allRestaurantsResponse.Success)
                {
                    return allRestaurantsResponse;
                }

                return new AppResponse<PagedResultDto<AdminRestaurantDto>>(
                    allRestaurantsResponse.Data, 
                    "Featured restaurants retrieved successfully", 
                    200, 
                    true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching featured restaurants for home page");
                return new AppResponse<PagedResultDto<AdminRestaurantDto>>(null, "Error fetching featured restaurants", 500, false);
            }
        }        /// <summary>
        /// Get dishes for a specific restaurant
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <returns>List of dishes for the restaurant</returns>
        [HttpGet("restaurants/{restaurantId}/dishes")]
        public async Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetRestaurantDishes(int restaurantId, [FromQuery] PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation("Fetching dishes for restaurant ID: {RestaurantId}", restaurantId);
                
                var result = await _dishManagementService.GetDishesInRestaurant(restaurantId, paginationDto);
                
                _logger.LogInformation("Found {DishCount} dishes for restaurant ID: {RestaurantId}", 
                    result.Data?.Data?.Count ?? 0, restaurantId);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dishes for restaurant ID: {RestaurantId}", restaurantId);
                return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(null, "Error fetching restaurant dishes", 500, false);
            }        }

        /// <summary>
        /// Get categories for a specific restaurant
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <returns>List of categories for the restaurant</returns>
        [HttpGet("restaurants/{restaurantId}/categories")]
        public async Task<AppResponse<PagedResultDto<CategoryDto>>> GetRestaurantCategories(int restaurantId, [FromQuery] PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation("Fetching categories for restaurant ID: {RestaurantId}", restaurantId);
                return await _categoryManagementService.GetCategories(restaurantId, paginationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories for restaurant ID: {RestaurantId}", restaurantId);
                return new AppResponse<PagedResultDto<CategoryDto>>(null, "Error fetching restaurant categories", 500, false);
            }
        }

        /// <summary>
        /// Get popular dishes (first 8 available dishes)
        /// </summary>
        /// <returns>List of popular dishes</returns>
        [HttpGet("popular-dishes")]
        public async Task<AppResponse<PagedResultDto<AdminRestaurantDishDto>>> GetPopularDishes([FromQuery] PaginationDto? paginationDto = null)
        {
            try
            {
                _logger.LogInformation("Fetching popular dishes for home page");
                
                // If pagination is not provided, use default pagination for popular dishes (8 items)
                var defaultPagination = paginationDto ?? new PaginationDto { PageNumber = 1, PageSize = 8 };
                
                var allDishesResponse = await _dishManagementService.GetAllDishesAsync(defaultPagination);
                
                if (!allDishesResponse.Success)
                {
                    return allDishesResponse;
                }

                return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(
                    allDishesResponse.Data, 
                    "Popular dishes retrieved successfully", 
                    200, 
                    true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching popular dishes for home page");
                return new AppResponse<PagedResultDto<AdminRestaurantDishDto>>(null, "Error fetching popular dishes", 500, false);
            }
        }

        /// <summary>
        /// Get restaurant statistics for home page
        /// </summary>
        /// <returns>Basic statistics</returns>
        [HttpGet("stats")]
        public async Task<AppResponse<object>> GetHomeStats()
        {
            try
            {
                _logger.LogInformation("Fetching home page statistics");
                
                // Get all data without pagination for statistics
                var restaurantsResponse = await _restaurantManagementService.GetAllRestaurants();
                var dishesResponse = await _dishManagementService.GetAllDishesAsync();
                
                var stats = new
                {
                    TotalRestaurants = 0,
                    TotalDishes = 0,
                    AvailableDishes = 0,
                    OpenRestaurants = 0
                };
                
                return new AppResponse<object>(stats, "Statistics retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching home page statistics");
                return new AppResponse<object>(null, "Error fetching statistics", 500, false);
            }
        }

        /// <summary>
        /// DEBUG: Get all dishes with their restaurant IDs for debugging
        /// </summary>
        /// <returns>All dishes with restaurant info</returns>
        [HttpGet("debug/all-dishes")]
        public async Task<AppResponse<object>> GetAllDishesDebug()
        {
            try
            {
                _logger.LogInformation("DEBUG: Fetching all dishes for debugging");
                
                var allDishesResponse = await _dishManagementService.GetAllDishesAsync();
                
                if (allDishesResponse.Success && allDishesResponse.Data != null)
                {
                    var dishesWithRestaurantInfo = allDishesResponse.Data.Data.Select(d => new
                    {
                        DishId = d.Id,
                        DishName = d.Name,
                        RestaurantId = d.RestaurantId,
                        RestaurantName = d.RestaurantName
                    }).ToList();

                    return new AppResponse<object>(dishesWithRestaurantInfo, "All dishes retrieved for debugging", 200, true);
                }
                
                return new AppResponse<object>(null, "No dishes found", 404, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all dishes for debugging");
                return new AppResponse<object>(null, "Error fetching dishes", 500, false);
            }
        }
    }
}
