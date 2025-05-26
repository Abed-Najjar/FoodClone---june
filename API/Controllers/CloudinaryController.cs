using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using API.Services.CmsServiceFolder;
using API.Repositories.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICmsRepository _cmsRepository;

        public CloudinaryController(ICloudinaryService cloudinaryService, ICmsRepository cmsRepository)
        {
            _cloudinaryService = cloudinaryService;
            _cmsRepository = cmsRepository;
        }        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _cloudinaryService.UploadImageAsync(file);
            return Ok(new { url });
        }        [HttpPost("upload-restaurant-logo/{restaurantId}")]
        public async Task<IActionResult> UploadRestaurantLogo(int restaurantId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _cloudinaryService.UploadImageAsync(file, "restaurants/logos");
            var restaurant = await _cmsRepository.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
                return NotFound("Restaurant not found.");
            restaurant.LogoUrl = url;
            await _cmsRepository.UpdateRestaurantAsync(restaurant);
            return Ok(new { url });
        }        [HttpPost("upload-restaurant-cover/{restaurantId}")]
        public async Task<IActionResult> UploadRestaurantCover(int restaurantId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _cloudinaryService.UploadImageAsync(file, "restaurants/covers");
            var restaurant = await _cmsRepository.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
                return NotFound("Restaurant not found.");
            restaurant.CoverImageUrl = url;
            await _cmsRepository.UpdateRestaurantAsync(restaurant);
            return Ok(new { url });
        }
    }
}
