using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using API.Services.CmsServiceFolder;
using API.Repositories.Interfaces;
using API.AppResponse;
using API.Data;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IDishRepository _dishRepository;
        private readonly AppDbContext _context;

        public CloudinaryController(ICloudinaryService cloudinaryService, IRestaurantRepository restaurantRepository, IDishRepository dishRepository, AppDbContext context)
        {
            _cloudinaryService = cloudinaryService;
            _restaurantRepository = restaurantRepository;
            _dishRepository = dishRepository;
            _context = context;
        }
        
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _cloudinaryService.UploadImageAsync(file);
            return Ok(new { url });
        }
        
        [HttpPost("upload-restaurant-logo/{restaurantId}")]
        public async Task<IActionResult> UploadRestaurantLogo(int restaurantId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new AppResponse.AppResponse<string>(null, "No file uploaded", 400, false));

            try
            {
                var url = await _cloudinaryService.UploadImageAsync(file, "restaurants/logos");
                var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantId);
                if (restaurant == null)
                return NotFound(new AppResponse.AppResponse<string>(null, "Restaurant not found", 404, false));
                
                restaurant.LogoUrl = url;
                await _restaurantRepository.UpdateRestaurantAsync(restaurant);
                await _context.SaveChangesAsync();
                
                return Ok(new AppResponse.AppResponse<string>(url, "Logo image uploaded successfully", 200, true));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppResponse.AppResponse<string>(null, $"Error uploading image: {ex.Message}", 500, false));
            }
        }
        
        [HttpPost("upload-restaurant-cover/{restaurantId}")]
        public async Task<IActionResult> UploadRestaurantCover(int restaurantId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new AppResponse.AppResponse<string>(null, "No file uploaded", 400, false));

            try
            {
                var url = await _cloudinaryService.UploadImageAsync(file, "restaurants/covers");
                var restaurant = await _restaurantRepository.GetRestaurantByIdAsync(restaurantId);
                if (restaurant == null)                return NotFound(new AppResponse.AppResponse<string>(null, "Restaurant not found", 404, false));
                
                restaurant.CoverImageUrl = url;
                await _restaurantRepository.UpdateRestaurantAsync(restaurant);
                await _context.SaveChangesAsync();
                
                return Ok(new AppResponse.AppResponse<string>(url, "Cover image uploaded successfully", 200, true));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppResponse.AppResponse<string>(null, $"Error uploading image: {ex.Message}", 500, false));
            }
        }

        [HttpPost("upload-dish-image/{dishId}")]
        public async Task<IActionResult> UploadDishImage(int dishId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new AppResponse.AppResponse<string>(null, "No file uploaded", 400, false));

            try
            {
                // Upload image to Cloudinary in a dishes folder
                var url = await _cloudinaryService.UploadImageAsync(file, "dishes");
                
                // Update the dish in the database with the new image URL
                var dish = await _dishRepository.GetDishByIdAsync(dishId);
                if (dish == null)
                    return NotFound(new AppResponse.AppResponse<string>(null, "Dish not found", 404, false));
                  dish.ImageUrl = url;
                await _dishRepository.UpdateDishAsync(dish);
                await _context.SaveChangesAsync();
                
                // Return success response with the URL
                return Ok(new AppResponse.AppResponse<string>(url, "Image uploaded successfully", 200, true));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppResponse.AppResponse<string>(null, $"Error uploading image: {ex.Message}", 500, false));
            }
        }
    }
}
