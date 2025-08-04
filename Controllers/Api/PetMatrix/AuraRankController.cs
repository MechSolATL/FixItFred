using Microsoft.AspNetCore.Mvc;
using Services.PetMatrix;
using Data.Models.PetMatrix;

namespace Controllers.Api.PetMatrix
{
    /// <summary>
    /// API controller for aura points, MOD ranks, and Watchtower features.
    /// Handles the $1 = 1 Aura point economy and MOD progression system.
    /// </summary>
    [ApiController]
    [Route("api/petmatrix/aura")]
    public class AuraRankController : ControllerBase
    {
        private readonly AuraRankService _auraRankService;
        private readonly WatchtowerPetMatrixService _watchtowerService;
        private readonly ILogger<AuraRankController> _logger;

        public AuraRankController(
            AuraRankService auraRankService, 
            WatchtowerPetMatrixService watchtowerService,
            ILogger<AuraRankController> logger)
        {
            _auraRankService = auraRankService;
            _watchtowerService = watchtowerService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current user's aura rank and status.
        /// </summary>
        [HttpGet("my-rank")]
        public async Task<ActionResult<AuraRank>> GetMyAuraRank()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var auraRank = await _auraRankService.GetOrCreateAuraRankAsync(userId);
                return Ok(auraRank);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting aura rank for user {UserId}", userId);
                return StatusCode(500, "Failed to get aura rank");
            }
        }

        /// <summary>
        /// Updates subscription status for loyalty benefits.
        /// </summary>
        [HttpPost("subscription")]
        public async Task<ActionResult<SubscriptionUpdateResult>> UpdateSubscription([FromBody] UpdateSubscriptionRequest request)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var promotionOccurred = await _auraRankService.UpdateSubscriptionStatusAsync(
                    userId, request.IsSubscriber, request.SubscriptionYears);

                var result = new SubscriptionUpdateResult
                {
                    Success = true,
                    Message = promotionOccurred ? "Congratulations! You've been promoted to Manager!" : "Subscription updated",
                    PromotionOccurred = promotionOccurred
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error updating subscription for user {UserId}", userId);
                return StatusCode(500, "Failed to update subscription");
            }
        }

        /// <summary>
        /// Enters or exits Watchtower proximity.
        /// </summary>
        [HttpPost("watchtower/proximity")]
        public async Task<ActionResult<WatchtowerProximityResult>> UpdateWatchtowerProximity([FromBody] WatchtowerProximityRequest request)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                WatchtowerProximityResult result;
                
                if (request.IsNear)
                {
                    result = await _watchtowerService.EnterWatchtowerProximityAsync(userId, request.Distance);
                }
                else
                {
                    result = await _watchtowerService.ExitWatchtowerProximityAsync(userId);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error updating Watchtower proximity for user {UserId}", userId);
                return StatusCode(500, "Failed to update Watchtower proximity");
            }
        }

        /// <summary>
        /// Gets nearby MODs for proximity effects.
        /// </summary>
        [HttpGet("watchtower/nearby-mods")]
        public async Task<ActionResult<List<NearbyMod>>> GetNearbyMods()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var nearbyMods = await _auraRankService.GetNearbyModsAsync(userId);
                return Ok(nearbyMods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting nearby MODs for user {UserId}", userId);
                return StatusCode(500, "Failed to get nearby MODs");
            }
        }

        /// <summary>
        /// Gets the HERO bar view for Matrix window effect.
        /// </summary>
        [HttpGet("watchtower/hero-bar")]
        public async Task<ActionResult<List<HeroBarMod>>> GetHeroBarView()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var heroBarMods = await _watchtowerService.GetHeroBarViewAsync(userId);
                return Ok(heroBarMods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting HERO bar view for user {UserId}", userId);
                return StatusCode(500, "Failed to get HERO bar view");
            }
        }

        /// <summary>
        /// Activates Oracle Hover State for qualified Manager MODs.
        /// </summary>
        [HttpPost("oracle/activate")]
        public async Task<ActionResult<OracleHoverResult>> ActivateOracleHover()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var result = await _watchtowerService.ToggleOracleHoverStateAsync(userId);
                
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error activating Oracle Hover for user {UserId}", userId);
                return StatusCode(500, "Failed to activate Oracle Hover State");
            }
        }

        /// <summary>
        /// Gets the aura leaderboard.
        /// </summary>
        [HttpGet("leaderboard")]
        public async Task<ActionResult<List<AuraLeaderboardEntry>>> GetAuraLeaderboard([FromQuery] int limit = 10)
        {
            try
            {
                var leaderboard = await _auraRankService.GetAuraLeaderboardAsync(limit);
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting aura leaderboard");
                return StatusCode(500, "Failed to get leaderboard");
            }
        }

        /// <summary>
        /// Gets manager metrics for high-ranking users.
        /// </summary>
        [HttpGet("managers")]
        public async Task<ActionResult<List<ManagerMetrics>>> GetManagerMetrics()
        {
            try
            {
                var metrics = await _auraRankService.GetManagerMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting manager metrics");
                return StatusCode(500, "Failed to get manager metrics");
            }
        }

        /// <summary>
        /// Forces a user promotion (admin only).
        /// </summary>
        [HttpPost("admin/promote")]
        public async Task<ActionResult> ForcePromoteUser([FromBody] ForcePromoteRequest request)
        {
            // In a real implementation, this would check for admin role
            
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Rank))
            {
                return BadRequest("User ID and rank are required");
            }

            try
            {
                var success = await _auraRankService.ForcePromoteUserAsync(request.UserId, request.Rank, request.Skin);
                
                if (!success)
                {
                    return BadRequest("Invalid rank or promotion failed");
                }

                _logger.LogInformation("[Sprint135_PetMatrix_API] Admin promoted user {UserId} to {Rank}", 
                    request.UserId, request.Rank);

                return Ok(new { Message = $"User promoted to {request.Rank}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error promoting user {UserId} to {Rank}", 
                    request.UserId, request.Rank);
                return StatusCode(500, "Failed to promote user");
            }
        }

        /// <summary>
        /// Gets comprehensive Watchtower dashboard data.
        /// </summary>
        [HttpGet("watchtower/dashboard")]
        public async Task<ActionResult<WatchtowerDashboardData>> GetWatchtowerDashboard()
        {
            var userId = GetCurrentUserId();
            
            try
            {
                var dashboardData = await _watchtowerService.GetEnhancedWatchtowerDataAsync(userId);
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting Watchtower dashboard data");
                return StatusCode(500, "Failed to get dashboard data");
            }
        }

        /// <summary>
        /// Gets system-wide analytics.
        /// </summary>
        [HttpGet("analytics")]
        public async Task<ActionResult<PetMatrixSystemAnalytics>> GetSystemAnalytics()
        {
            try
            {
                var analytics = await _watchtowerService.GetSystemAnalyticsAsync();
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting system analytics");
                return StatusCode(500, "Failed to get system analytics");
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
    }

    // Request/Response DTOs

    public class UpdateSubscriptionRequest
    {
        public bool IsSubscriber { get; set; }
        public int SubscriptionYears { get; set; }
    }

    public class SubscriptionUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public bool PromotionOccurred { get; set; }
    }

    public class WatchtowerProximityRequest
    {
        public bool IsNear { get; set; }
        public float Distance { get; set; } = 15f;
    }

    public class ForcePromoteRequest
    {
        public string UserId { get; set; } = "";
        public string Rank { get; set; } = "";
        public string? Skin { get; set; }
    }
}