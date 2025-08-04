using Microsoft.AspNetCore.Mvc;
using Services.PetMatrix;
using Data.Models.PetMatrix;

namespace Controllers.Api.PetMatrix
{
    /// <summary>
    /// API controller for the Chase-powered snack shop.
    /// Handles snack browsing, purchasing, and shop analytics.
    /// </summary>
    [ApiController]
    [Route("api/petmatrix/shop")]
    public class SnackShopController : ControllerBase
    {
        private readonly SnackShopService _snackShopService;
        private readonly ILogger<SnackShopController> _logger;

        public SnackShopController(SnackShopService snackShopService, ILogger<SnackShopController> logger)
        {
            _snackShopService = snackShopService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all available snacks in the shop.
        /// </summary>
        [HttpGet("snacks")]
        public async Task<ActionResult<List<Snack>>> GetAvailableSnacks()
        {
            try
            {
                var snacks = await _snackShopService.GetAvailableSnacksAsync();
                return Ok(snacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting available snacks");
                return StatusCode(500, "Failed to get snacks");
            }
        }

        /// <summary>
        /// Gets snacks compatible with a specific pet family.
        /// </summary>
        [HttpGet("snacks/family/{petFamily}")]
        public async Task<ActionResult<List<Snack>>> GetSnacksForFamily(string petFamily)
        {
            try
            {
                var snacks = await _snackShopService.GetSnacksForPetFamilyAsync(petFamily);
                return Ok(snacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting snacks for family {PetFamily}", petFamily);
                return StatusCode(500, "Failed to get snacks for family");
            }
        }

        /// <summary>
        /// Purchases a snack for a specific pet.
        /// </summary>
        [HttpPost("purchase")]
        public async Task<ActionResult<SnackPurchaseResult>> PurchaseSnack([FromBody] PurchaseSnackRequest request)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var result = await _snackShopService.PurchaseSnackAsync(
                    request.SnackId, 
                    request.PetId, 
                    userId, 
                    request.FlavorType);

                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                _logger.LogInformation("[Sprint135_PetMatrix_API] User {UserId} purchased {SnackName} for {Price:C}", 
                    userId, result.SnackName, result.TotalPrice);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error purchasing snack {SnackId} for user {UserId}", 
                    request.SnackId, userId);
                return StatusCode(500, "Failed to purchase snack");
            }
        }

        /// <summary>
        /// Gets available flavor options for snacks.
        /// </summary>
        [HttpGet("flavors")]
        public ActionResult<List<FlavorOption>> GetAvailableFlavors()
        {
            try
            {
                var flavors = _snackShopService.GetAvailableFlavors();
                return Ok(flavors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting available flavors");
                return StatusCode(500, "Failed to get flavors");
            }
        }

        /// <summary>
        /// Gets purchase history for the current user.
        /// </summary>
        [HttpGet("my-purchases")]
        public async Task<ActionResult<List<SnackPurchase>>> GetMyPurchases([FromQuery] int limit = 20)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var purchases = await _snackShopService.GetUserPurchaseHistoryAsync(userId, limit);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting purchase history for user {UserId}", userId);
                return StatusCode(500, "Failed to get purchase history");
            }
        }

        /// <summary>
        /// Gets shop analytics (admin only).
        /// </summary>
        [HttpGet("analytics")]
        public async Task<ActionResult<SnackShopAnalytics>> GetShopAnalytics()
        {
            // In a real implementation, this would check for admin role
            // For now, anyone can access analytics

            try
            {
                var analytics = await _snackShopService.GetShopAnalyticsAsync();
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting shop analytics");
                return StatusCode(500, "Failed to get shop analytics");
            }
        }

        /// <summary>
        /// Creates a custom snack (admin only).
        /// </summary>
        [HttpPost("admin/create-snack")]
        public async Task<ActionResult<Snack>> CreateCustomSnack([FromBody] CreateSnackRequest request)
        {
            // In a real implementation, this would check for admin role
            
            if (string.IsNullOrEmpty(request.Name) || request.Price <= 0)
            {
                return BadRequest("Snack name and valid price are required");
            }

            try
            {
                var snack = await _snackShopService.CreateCustomSnackAsync(request);
                
                _logger.LogInformation("[Sprint135_PetMatrix_API] Created custom snack: {SnackName} - {Price:C}", 
                    snack.Name, snack.Price);

                return Ok(snack);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error creating custom snack {SnackName}", request.Name);
                return StatusCode(500, "Failed to create custom snack");
            }
        }

        /// <summary>
        /// Seeds the shop with default snacks (admin only).
        /// </summary>
        [HttpPost("admin/seed-defaults")]
        public async Task<ActionResult> SeedDefaultSnacks()
        {
            try
            {
                await _snackShopService.SeedDefaultSnacksAsync();
                
                _logger.LogInformation("[Sprint135_PetMatrix_API] Seeded default snacks");
                
                return Ok(new { Message = "Default snacks seeded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error seeding default snacks");
                return StatusCode(500, "Failed to seed default snacks");
            }
        }

        /// <summary>
        /// Gets detailed snack information including risk warnings.
        /// </summary>
        [HttpGet("snacks/{snackId}")]
        public async Task<ActionResult<SnackDetailsResponse>> GetSnackDetails(Guid snackId)
        {
            try
            {
                var snacks = await _snackShopService.GetAvailableSnacksAsync();
                var snack = snacks.FirstOrDefault(s => s.SnackId == snackId);

                if (snack == null)
                {
                    return NotFound("Snack not found");
                }

                var details = new SnackDetailsResponse
                {
                    Snack = snack,
                    RiskWarning = snack.GetRiskWarning(),
                    CompatibleFamilies = GetCompatibleFamilies(snack),
                    TotalPriceWithFlavor = snack.GetTotalPrice(),
                    AvailableFlavors = _snackShopService.GetAvailableFlavors()
                };

                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting snack details for {SnackId}", snackId);
                return StatusCode(500, "Failed to get snack details");
            }
        }

        /// <summary>
        /// Gets the current user ID from the request context.
        /// </summary>
        private string GetCurrentUserId()
        {
            return Request.Headers["X-User-Id"].FirstOrDefault() ?? 
                   Request.Query["userId"].FirstOrDefault() ?? 
                   "demo-user";
        }

        /// <summary>
        /// Gets compatible pet families for a snack.
        /// </summary>
        private List<string> GetCompatibleFamilies(Snack snack)
        {
            if (snack.TargetFamily == "all")
            {
                return new List<string> { "feline", "canine", "matrix_creature" };
            }
            
            return new List<string> { snack.TargetFamily };
        }
    }

    // Request/Response DTOs

    public class PurchaseSnackRequest
    {
        public Guid SnackId { get; set; }
        public Guid PetId { get; set; }
        public string? FlavorType { get; set; }
    }

    public class SnackDetailsResponse
    {
        public Snack Snack { get; set; } = null!;
        public string RiskWarning { get; set; } = "";
        public List<string> CompatibleFamilies { get; set; } = new();
        public decimal TotalPriceWithFlavor { get; set; }
        public List<FlavorOption> AvailableFlavors { get; set; } = new();
    }
}