using API.AppResponse;
using API.DTOs;
using API.Services.CartServiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Calculate cart totals securely on the backend
        /// Matches the Order Summary format: Subtotal, Delivery Fee, Tax, Total
        /// </summary>
        [HttpPost("calculate")]
        [AllowAnonymous] // Allow anonymous users to calculate cart totals
        public async Task<ActionResult<AppResponse<CartCalculationResponseDto>>> CalculateCartTotals([FromBody] CartCalculationRequestDto request)
        {
            try
            {
                // Log the request for debugging
                _logger.LogInformation($"Cart calculation requested for restaurant {request.RestaurantId} with {request.CartItems.Count} items");

                // Get user ID if authenticated (optional for cart calculations)
                int? userId = null;
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int authenticatedUserId))
                {
                    userId = authenticatedUserId;
                }

                // Validate request
                if (request.CartItems == null || !request.CartItems.Any())
                {
                    return BadRequest(new AppResponse<CartCalculationResponseDto>(null, "Cart cannot be empty", 400, false));
                }

                if (request.RestaurantId <= 0)
                {
                    return BadRequest(new AppResponse<CartCalculationResponseDto>(null, "Valid restaurant ID is required", 400, false));
                }

                // Calculate totals securely on backend
                var result = await _cartService.CalculateCartTotalsAsync(request, userId);

                if (result.Success)
                {
                    _logger.LogInformation($"Cart calculation successful: Subtotal={result.Data?.Subtotal:F2}, Total={result.Data?.GrandTotal:F2} JOD");
                }
                else
                {
                    _logger.LogWarning($"Cart calculation failed: {result.ErrorMessage}");
                }

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CalculateCartTotals");
                return StatusCode(500, new AppResponse<CartCalculationResponseDto>(null, $"Internal server error: {ex.Message}", 500, false));
            }
        }

        /// <summary>
        /// Validate promo code and return discount details
        /// </summary>
        [HttpPost("validate-promo")]
        [AllowAnonymous]
        public async Task<ActionResult<AppResponse<PromoCodeDto>>> ValidatePromoCode([FromBody] ValidatePromoCodeRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.PromoCode))
                {
                    return BadRequest(new AppResponse<PromoCodeDto>(null, "Promo code is required", 400, false));
                }

                if (request.Subtotal <= 0)
                {
                    return BadRequest(new AppResponse<PromoCodeDto>(null, "Valid subtotal is required", 400, false));
                }

                var result = await _cartService.ValidatePromoCodeAsync(request.PromoCode, request.Subtotal);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ValidatePromoCode");
                return StatusCode(500, new AppResponse<PromoCodeDto>(null, $"Internal server error: {ex.Message}", 500, false));
            }
        }

        /// <summary>
        /// Get available promo codes for current cart
        /// </summary>
        [HttpGet("promo-codes")]
        [AllowAnonymous]
        public async Task<ActionResult<AppResponse<List<PromoCodeDto>>>> GetAvailablePromoCodes([FromQuery] decimal subtotal = 0)
        {
            try
            {
                var result = await _cartService.GetAvailablePromoCodesAsync(subtotal);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAvailablePromoCodes");
                return StatusCode(500, new AppResponse<List<PromoCodeDto>>(null, $"Internal server error: {ex.Message}", 500, false));
            }
        }

        /// <summary>
        /// Health check endpoint for cart service
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult<AppResponse<object>> HealthCheck()
        {
            var healthData = new { 
                status = "Cart service is running", 
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            };
            
            return Ok(new AppResponse<object>(healthData, "Cart service is healthy", 200, true));
        }
    }
} 