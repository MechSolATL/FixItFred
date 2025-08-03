using Microsoft.AspNetCore.Mvc;
using Services;
using Data.Models;

namespace Controllers.Api
{
    /// <summary>
    /// REST API Controller for HeroFX Studio effects management
    /// Sprint127_HeroFX_StudioDivision - API endpoints for effect triggering and management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HeroFXController : ControllerBase
    {
        private readonly HeroFXEngine _heroFxEngine;
        private readonly ILogger<HeroFXController> _logger;

        public HeroFXController(HeroFXEngine heroFxEngine, ILogger<HeroFXController> logger)
        {
            _heroFxEngine = heroFxEngine;
            _logger = logger;
        }

        /// <summary>
        /// Get all available effects
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<HeroImpactEffect>>> GetEffects(
            [FromQuery] bool activeOnly = true,
            [FromQuery] string? category = null)
        {
            try
            {
                var effects = await _heroFxEngine.GetEffectsAsync(activeOnly, category);
                return Ok(effects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving effects");
                return StatusCode(500, new { error = "Error retrieving effects" });
            }
        }

        /// <summary>
        /// Get a specific effect by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<HeroImpactEffect>> GetEffect(int id)
        {
            try
            {
                var effect = await _heroFxEngine.GetEffectAsync(id);
                if (effect == null)
                    return NotFound(new { error = "Effect not found" });

                return Ok(effect);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving effect {EffectId}", id);
                return StatusCode(500, new { error = "Error retrieving effect" });
            }
        }

        /// <summary>
        /// Trigger an effect
        /// </summary>
        [HttpPost("trigger")]
        public async Task<ActionResult> TriggerEffect([FromBody] TriggerEffectRequest request)
        {
            try
            {
                var userId = User?.Identity?.Name ?? "anonymous";
                var success = await _heroFxEngine.TriggerEffectAsync(
                    request.EffectName,
                    request.TriggerEvent,
                    userId,
                    request.UserRole,
                    request.DeviceType
                );

                if (!success)
                    return BadRequest(new { error = "Failed to trigger effect" });

                return Ok(new { success = true, message = "Effect triggered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering effect {EffectName}", request.EffectName);
                return StatusCode(500, new { error = "Error triggering effect" });
            }
        }

        /// <summary>
        /// Get effects by trigger event
        /// </summary>
        [HttpGet("trigger/{triggerEvent}")]
        public async Task<ActionResult<List<HeroImpactEffect>>> GetEffectsByTrigger(
            string triggerEvent,
            [FromQuery] string? userRole = null,
            [FromQuery] string? behaviorMood = null)
        {
            try
            {
                var effects = await _heroFxEngine.GetEffectsByTriggerAsync(triggerEvent, userRole, behaviorMood);
                return Ok(effects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving effects for trigger {TriggerEvent}", triggerEvent);
                return StatusCode(500, new { error = "Error retrieving effects" });
            }
        }

        /// <summary>
        /// Get a random effect based on criteria
        /// </summary>
        [HttpGet("random")]
        public async Task<ActionResult<HeroImpactEffect>> GetRandomEffect(
            [FromQuery] string? persona = null,
            [FromQuery] string? role = null,
            [FromQuery] string? mood = null)
        {
            try
            {
                var effect = await _heroFxEngine.GetRandomEffectAsync(persona, role, mood);
                if (effect == null)
                    return NotFound(new { error = "No effects found matching criteria" });

                return Ok(effect);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving random effect");
                return StatusCode(500, new { error = "Error retrieving random effect" });
            }
        }

        /// <summary>
        /// Log a reaction to an effect (for analytics)
        /// </summary>
        [HttpPost("reaction")]
        public async Task<ActionResult> LogReaction([FromBody] LogReactionRequest request)
        {
            try
            {
                var userId = User?.Identity?.Name ?? "anonymous";
                await _heroFxEngine.LogReactionAsync(request.EffectName, userId);
                return Ok(new { success = true, message = "Reaction logged successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging reaction for effect {EffectName}", request.EffectName);
                return StatusCode(500, new { error = "Error logging reaction" });
            }
        }

        /// <summary>
        /// Get analytics for effects
        /// </summary>
        [HttpGet("analytics")]
        public async Task<ActionResult<object>> GetAnalytics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
                var to = toDate ?? DateTime.UtcNow;

                var analytics = await _heroFxEngine.GetAnalyticsAsync(from, to);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analytics");
                return StatusCode(500, new { error = "Error retrieving analytics" });
            }
        }

        /// <summary>
        /// Create or update an effect (Admin only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<HeroImpactEffect>> CreateEffect([FromBody] HeroImpactEffect effect)
        {
            try
            {
                var userId = User?.Identity?.Name ?? "admin";
                var savedEffect = await _heroFxEngine.SaveEffectAsync(effect, userId);
                return Ok(savedEffect);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/updating effect {EffectName}", effect.Name);
                return StatusCode(500, new { error = "Error saving effect" });
            }
        }

        /// <summary>
        /// Delete an effect (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEffect(int id)
        {
            try
            {
                var userId = User?.Identity?.Name ?? "admin";
                var success = await _heroFxEngine.DeleteEffectAsync(id, userId);
                
                if (!success)
                    return NotFound(new { error = "Effect not found" });

                return Ok(new { success = true, message = "Effect deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting effect {EffectId}", id);
                return StatusCode(500, new { error = "Error deleting effect" });
            }
        }

        /// <summary>
        /// Seed default effects (Admin only)
        /// </summary>
        [HttpPost("seed")]
        public async Task<ActionResult> SeedDefaultEffects()
        {
            try
            {
                var userId = User?.Identity?.Name ?? "admin";
                await _heroFxEngine.SeedDefaultEffectsAsync(userId);
                return Ok(new { success = true, message = "Default effects seeded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default effects");
                return StatusCode(500, new { error = "Error seeding effects" });
            }
        }
    }

    /// <summary>
    /// Request model for triggering effects
    /// </summary>
    public class TriggerEffectRequest
    {
        public string EffectName { get; set; } = string.Empty;
        public string TriggerEvent { get; set; } = string.Empty;
        public string? UserRole { get; set; }
        public string? DeviceType { get; set; }
    }

    /// <summary>
    /// Request model for logging reactions
    /// </summary>
    public class LogReactionRequest
    {
        public string EffectName { get; set; } = string.Empty;
    }
}