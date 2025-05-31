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
        public async Task<AppResponse<List<AdminRestaurantDto>>> GetAllRestaurants()
        {
            try
            {
                _logger.LogInformation("Fetching all restaurants for home page");
                return await _restaurantManagementService.GetAllRestaurants();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching restaurants for home page");
                return new AppResponse<List<AdminRestaurantDto>>(null, "Error fetching restaurants", 500, false);
            }
        }

        /// <summary>
        /// Get all dishes for the home page
        /// </summary>
        /// <returns>List of dishes</returns>
        [HttpGet("dishes")]
        public async Task<AppResponse<List<DishDto>>> GetAllDishes()
        {
            try
            {
                _logger.LogInformation("Fetching all dishes for home page");
                return await _dishManagementService.GetAllDishesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dishes for home page");
                return new AppResponse<List<DishDto>>(null, "Error fetching dishes", 500, false);
            }
        }

        /// <summary>
        /// Get featured/popular restaurants (first 6 restaurants)
        /// </summary>
        /// <returns>List of featured restaurants</returns>
        [HttpGet("featured-restaurants")]
        public async Task<AppResponse<List<AdminRestaurantDto>>> GetFeaturedRestaurants()
        {
            try
            {
                _logger.LogInformation("Fetching featured restaurants for home page");
                var allRestaurantsResponse = await _restaurantManagementService.GetAllRestaurants();
                
                if (!allRestaurantsResponse.Success || allRestaurantsResponse.Data == null)
                {
                    return allRestaurantsResponse;
                }

                // Take first 6 restaurants as featured
                var featuredRestaurants = allRestaurantsResponse.Data.Take(6).ToList();
                
                return new AppResponse<List<AdminRestaurantDto>>(
                    featuredRestaurants, 
                    "Featured restaurants retrieved successfully", 
                    200, 
                    true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching featured restaurants for home page");
                return new AppResponse<List<AdminRestaurantDto>>(null, "Error fetching featured restaurants", 500, false);
            }
        }        /// <summary>
        /// Get dishes for a specific restaurant
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <returns>List of dishes for the restaurant</returns>
        [HttpGet("restaurants/{restaurantId}/dishes")]
        public async Task<AppResponse<List<DishDto>>> GetRestaurantDishes(int restaurantId)
        {
            try
            {
                _logger.LogInformation("Fetching dishes for restaurant ID: {RestaurantId}", restaurantId);
                
                // Get all dishes and filter by restaurant ID
                var allDishesResponse = await _dishManagementService.GetAllDishesAsync();
                
                if (!allDishesResponse.Success || allDishesResponse.Data == null)
                {
                    return allDishesResponse;
                }

                // Filter dishes by restaurant ID
                var restaurantDishes = allDishesResponse.Data
                    .Where(d => d.RestaurantId == restaurantId)
                    .ToList();
                
                return new AppResponse<List<DishDto>>(
                    restaurantDishes, 
                    "Restaurant dishes retrieved successfully", 
                    200, 
                    true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching dishes for restaurant ID: {RestaurantId}", restaurantId);
                return new AppResponse<List<DishDto>>(null, "Error fetching restaurant dishes", 500, false);
            }        }

        /// <summary>
        /// Get categories for a specific restaurant
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <returns>List of categories for the restaurant</returns>
        [HttpGet("restaurants/{restaurantId}/categories")]
        public async Task<AppResponse<List<CategoryDto>>> GetRestaurantCategories(int restaurantId)
        {
            try
            {
                _logger.LogInformation("Fetching categories for restaurant ID: {RestaurantId}", restaurantId);
                return await _categoryManagementService.GetCategories(restaurantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories for restaurant ID: {RestaurantId}", restaurantId);
                return new AppResponse<List<CategoryDto>>(null, "Error fetching restaurant categories", 500, false);
            }
        }

        /// <summary>
        /// Get popular dishes (first 8 available dishes)
        /// </summary>
        /// <returns>List of popular dishes</returns>
        [HttpGet("popular-dishes")]
        public async Task<AppResponse<List<DishDto>>> GetPopularDishes()
        {
            try
            {
                _logger.LogInformation("Fetching popular dishes for home page");
                var allDishesResponse = await _dishManagementService.GetAllDishesAsync();
                
                if (!allDishesResponse.Success || allDishesResponse.Data == null)
                {
                    return allDishesResponse;
                }

                // Filter available dishes and take first 8 as popular
                var popularDishes = allDishesResponse.Data
                    .Where(d => d.IsAvailable)
                    .Take(8)
                    .ToList();
                
                return new AppResponse<List<DishDto>>(
                    popularDishes, 
                    "Popular dishes retrieved successfully", 
                    200, 
                    true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching popular dishes for home page");
                return new AppResponse<List<DishDto>>(null, "Error fetching popular dishes", 500, false);
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
                
                var restaurantsResponse = await _restaurantManagementService.GetAllRestaurants();
                var dishesResponse = await _dishManagementService.GetAllDishesAsync();
                
                var stats = new
                {
                    TotalRestaurants = restaurantsResponse.Success && restaurantsResponse.Data != null 
                        ? restaurantsResponse.Data.Count 
                        : 0,
                    TotalDishes = dishesResponse.Success && dishesResponse.Data != null 
                        ? dishesResponse.Data.Count 
                        : 0,
                    AvailableDishes = dishesResponse.Success && dishesResponse.Data != null 
                        ? dishesResponse.Data.Count(d => d.IsAvailable) 
                        : 0,
                    OpenRestaurants = restaurantsResponse.Success && restaurantsResponse.Data != null 
                        ? restaurantsResponse.Data.Count(r => r.IsOpen) 
                        : 0
                };
                
                return new AppResponse<object>(stats, "Statistics retrieved successfully", 200, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching home page statistics");
                return new AppResponse<object>(null, "Error fetching statistics", 500, false);
            }
        }
    }
}
