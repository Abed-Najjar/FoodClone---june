using API.AppResponse;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Provider,Admin")]
    [Route("api/[controller]")]
    public class CategoryManagementController : ControllerBase
    {
        private readonly ICategoryManagementService _categoryManagementService;

        public CategoryManagementController(ICategoryManagementService categoryManagementService)
        {
            _categoryManagementService = categoryManagementService;
        }

        // Category Endpoints
        [HttpGet("categories/{restaurantId}")]
        public async Task<AppResponse<PagedResultDto<CategoryDto>>> GetCategories(int restaurantId, [FromQuery] PaginationDto? paginationDto = null)
        {
            return await _categoryManagementService.GetCategories(restaurantId, paginationDto);
        }

        [HttpPost("categories")]
        public async Task<AppResponse<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            return await _categoryManagementService.CreateCategory(categoryDto);
        }

        [HttpPut("categories/{id}")]
        public async Task<AppResponse<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto categoryDto)
        {
            return await _categoryManagementService.UpdateCategory(id, categoryDto);
        }

        [HttpDelete("categories/{id}")]
        public async Task<AppResponse<bool>> DeleteCategory(int id)
        {
            return await _categoryManagementService.DeleteCategory(id);
        }
    }
}