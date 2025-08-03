using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    /// <summary>
    /// HeroFX Engine service for managing visual effects and animations across MVP-Core
    /// Sprint127_HeroFX_StudioDivision - Empowers admins to control "KAPOW" moments
    /// </summary>
    public class HeroFXEngine
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HeroFXEngine> _logger;

        public HeroFXEngine(ApplicationDbContext context, ILogger<HeroFXEngine> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Effect Management

        /// <summary>
        /// Get all available effects with optional filtering
        /// </summary>
        public async Task<List<HeroImpactEffect>> GetEffectsAsync(bool activeOnly = true, string? fxPackCategory = null)
        {
            var query = _context.HeroImpactEffects.AsQueryable();
            
            if (activeOnly)
                query = query.Where(e => e.IsActive);
                
            if (!string.IsNullOrEmpty(fxPackCategory))
                query = query.Where(e => e.FxPackCategory == fxPackCategory);
                
            return await query.OrderBy(e => e.DisplayName).ToListAsync();
        }

        /// <summary>
        /// Get effect by ID
        /// </summary>
        public async Task<HeroImpactEffect?> GetEffectAsync(int effectId)
        {
            return await _context.HeroImpactEffects.FindAsync(effectId);
        }

        /// <summary>
        /// Get effects by trigger event (dispatch, login, praise, etc.)
        /// </summary>
        public async Task<List<HeroImpactEffect>> GetEffectsByTriggerAsync(string triggerEvent, string? userRole = null, string? behaviorMood = null)
        {
            var query = _context.HeroImpactEffects
                .Where(e => e.IsActive)
                .Where(e => e.TriggerEvents != null && e.TriggerEvents.Contains(triggerEvent));

            // Filter by role assignments if specified
            if (!string.IsNullOrEmpty(userRole))
            {
                query = query.Where(e => e.RoleAssignments == null || e.RoleAssignments.Contains(userRole));
            }

            // Filter by behavior mood if specified
            if (!string.IsNullOrEmpty(behaviorMood))
            {
                query = query.Where(e => e.BehaviorMoods == null || e.BehaviorMoods.Contains(behaviorMood));
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Create or update an effect
        /// </summary>
        public async Task<HeroImpactEffect> SaveEffectAsync(HeroImpactEffect effect, string userId)
        {
            if (effect.Id == 0)
            {
                // Creating new effect
                effect.CreatedAt = DateTime.UtcNow;
                effect.CreatedBy = userId;
                effect.UpdatedAt = DateTime.UtcNow;
                effect.UpdatedBy = userId;
                _context.HeroImpactEffects.Add(effect);
            }
            else
            {
                // Updating existing effect
                var existing = await _context.HeroImpactEffects.FindAsync(effect.Id);
                if (existing == null)
                    throw new ArgumentException($"Effect with ID {effect.Id} not found");

                // Update properties
                existing.DisplayName = effect.DisplayName;
                existing.CssClass = effect.CssClass;
                existing.AnimationCss = effect.AnimationCss;
                existing.JsFunction = effect.JsFunction;
                existing.DurationMs = effect.DurationMs;
                existing.TriggerEvents = effect.TriggerEvents;
                existing.PersonaAssignments = effect.PersonaAssignments;
                existing.RoleAssignments = effect.RoleAssignments;
                existing.BehaviorMoods = effect.BehaviorMoods;
                existing.VoiceFxConfig = effect.VoiceFxConfig;
                existing.VoiceType = effect.VoiceType;
                existing.SoundEffectPath = effect.SoundEffectPath;
                existing.IsMobileCompatible = effect.IsMobileCompatible;
                existing.IsDesktopCompatible = effect.IsDesktopCompatible;
                existing.IsActive = effect.IsActive;
                existing.IsPremium = effect.IsPremium;
                existing.FxPackCategory = effect.FxPackCategory;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = userId;

                effect = existing;
            }

            await _context.SaveChangesAsync();
            return effect;
        }

        /// <summary>
        /// Delete an effect
        /// </summary>
        public async Task<bool> DeleteEffectAsync(int effectId, string userId)
        {
            var effect = await _context.HeroImpactEffects.FindAsync(effectId);
            if (effect == null)
                return false;

            _context.HeroImpactEffects.Remove(effect);
            await _context.SaveChangesAsync();

            _logger.LogInformation("HeroFX effect '{EffectName}' deleted by user {UserId}", effect.Name, userId);
            return true;
        }

        #endregion

        #region Effect Triggering

        /// <summary>
        /// Trigger an effect and log analytics
        /// </summary>
        public async Task<bool> TriggerEffectAsync(string effectName, string triggerEvent, string? userId = null, string? userRole = null, string? deviceType = null)
        {
            try
            {
                var effect = await _context.HeroImpactEffects
                    .FirstOrDefaultAsync(e => e.Name == effectName && e.IsActive);

                if (effect == null)
                {
                    _logger.LogWarning("HeroFX effect '{EffectName}' not found or inactive", effectName);
                    return false;
                }

                // Check device compatibility
                if (deviceType == "mobile" && !effect.IsMobileCompatible)
                    return false;
                if (deviceType == "desktop" && !effect.IsDesktopCompatible)
                    return false;

                // Increment usage count
                effect.UsageCount++;
                effect.UpdatedAt = DateTime.UtcNow;

                // Log analytics
                var analyticsLog = new HeroFxAnalyticsLog
                {
                    EffectId = effect.Id,
                    EffectName = effect.Name,
                    TriggerEvent = triggerEvent,
                    UserId = userId,
                    UserRole = userRole,
                    DeviceType = deviceType,
                    WasSuccessful = true,
                    LoggedAt = DateTime.UtcNow
                };

                _context.HeroFxAnalyticsLogs.Add(analyticsLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation("HeroFX effect '{EffectName}' triggered by {TriggerEvent} for user {UserId}", 
                    effectName, triggerEvent, userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering HeroFX effect '{EffectName}'", effectName);
                
                // Log failed attempt
                var failedLog = new HeroFxAnalyticsLog
                {
                    EffectName = effectName,
                    TriggerEvent = triggerEvent,
                    UserId = userId,
                    UserRole = userRole,
                    DeviceType = deviceType,
                    WasSuccessful = false,
                    ErrorMessage = ex.Message,
                    LoggedAt = DateTime.UtcNow
                };

                _context.HeroFxAnalyticsLogs.Add(failedLog);
                await _context.SaveChangesAsync();

                return false;
            }
        }

        /// <summary>
        /// Get random effect for persona/role/mood combination
        /// </summary>
        public async Task<HeroImpactEffect?> GetRandomEffectAsync(string? persona = null, string? role = null, string? mood = null)
        {
            var query = _context.HeroImpactEffects.Where(e => e.IsActive);

            if (!string.IsNullOrEmpty(persona))
                query = query.Where(e => e.PersonaAssignments == null || e.PersonaAssignments.Contains(persona));

            if (!string.IsNullOrEmpty(role))
                query = query.Where(e => e.RoleAssignments == null || e.RoleAssignments.Contains(role));

            if (!string.IsNullOrEmpty(mood))
                query = query.Where(e => e.BehaviorMoods == null || e.BehaviorMoods.Contains(mood));

            var effects = await query.ToListAsync();
            if (!effects.Any())
                return null;

            var random = new Random();
            return effects[random.Next(effects.Count)];
        }

        #endregion

        #region Analytics

        /// <summary>
        /// Get analytics for effects within date range
        /// </summary>
        public async Task<object> GetAnalyticsAsync(DateTime fromDate, DateTime toDate)
        {
            var logs = await _context.HeroFxAnalyticsLogs
                .Where(l => l.LoggedAt >= fromDate && l.LoggedAt <= toDate)
                .ToListAsync();

            var totalTriggers = logs.Count;
            var successfulTriggers = logs.Count(l => l.WasSuccessful);
            var reactionsCount = logs.Count(l => l.GotReaction);

            var effectStats = logs.GroupBy(l => l.EffectName)
                .Select(g => new {
                    EffectName = g.Key,
                    TriggerCount = g.Count(),
                    SuccessRate = g.Count(l => l.WasSuccessful) * 100.0 / g.Count(),
                    ReactionRate = g.Count(l => l.GotReaction) * 100.0 / g.Count()
                })
                .OrderByDescending(e => e.TriggerCount)
                .ToList();

            var deviceStats = logs.GroupBy(l => l.DeviceType)
                .Select(g => new {
                    DeviceType = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .ToList();

            return new
            {
                Summary = new
                {
                    TotalTriggers = totalTriggers,
                    SuccessfulTriggers = successfulTriggers,
                    SuccessRate = totalTriggers > 0 ? successfulTriggers * 100.0 / totalTriggers : 0,
                    ReactionsCount = reactionsCount,
                    KapowToClickRatio = totalTriggers > 0 ? reactionsCount * 100.0 / totalTriggers : 0
                },
                EffectStats = effectStats,
                DeviceStats = deviceStats
            };
        }

        /// <summary>
        /// Update reaction count for an effect
        /// </summary>
        public async Task LogReactionAsync(string effectName, string? userId = null)
        {
            var effect = await _context.HeroImpactEffects
                .FirstOrDefaultAsync(e => e.Name == effectName);

            if (effect != null)
            {
                effect.ReactionCount++;
                effect.KapowToClickRatio = effect.UsageCount > 0 ? 
                    (decimal)effect.ReactionCount / effect.UsageCount * 100 : 0;
                await _context.SaveChangesAsync();
            }

            // Update the latest analytics log
            var latestLog = await _context.HeroFxAnalyticsLogs
                .Where(l => l.EffectName == effectName && l.UserId == userId)
                .OrderByDescending(l => l.LoggedAt)
                .FirstOrDefaultAsync();

            if (latestLog != null)
            {
                latestLog.GotReaction = true;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Seeding and Initialization

        /// <summary>
        /// Seed default effects for HeroFX Studio
        /// </summary>
        public async Task SeedDefaultEffectsAsync(string userId = "system")
        {
            var existingEffects = await _context.HeroImpactEffects.AnyAsync();
            if (existingEffects)
                return; // Already seeded

            var defaultEffects = new List<HeroImpactEffect>
            {
                new()
                {
                    Name = "slam",
                    DisplayName = "SLAM!",
                    CssClass = "hero-fx-slam",
                    DurationMs = 800,
                    TriggerEvents = "dispatch,praise,success",
                    RoleAssignments = "Tech,Admin",
                    BehaviorMoods = "celebration",
                    VoiceType = "chaos",
                    SoundEffectPath = "/sounds/slam.mp3",
                    CreatedBy = userId,
                    UpdatedBy = userId
                },
                new()
                {
                    Name = "pop",
                    DisplayName = "POP!",
                    CssClass = "hero-fx-pop",
                    DurationMs = 600,
                    TriggerEvents = "login,notification",
                    RoleAssignments = "CSR,Admin",
                    BehaviorMoods = "calm,celebration",
                    VoiceType = "calm",
                    SoundEffectPath = "/sounds/pop.mp3",
                    CreatedBy = userId,
                    UpdatedBy = userId
                },
                new()
                {
                    Name = "yeet",
                    DisplayName = "YEET!",
                    CssClass = "hero-fx-yeet",
                    DurationMs = 1200,
                    TriggerEvents = "dispatch,close",
                    RoleAssignments = "Tech",
                    BehaviorMoods = "frustration,celebration",
                    VoiceType = "chaos",
                    SoundEffectPath = "/sounds/yeet.mp3",
                    IsPremium = true,
                    FxPackCategory = "hype",
                    CreatedBy = userId,
                    UpdatedBy = userId
                },
                new()
                {
                    Name = "glitch",
                    DisplayName = "GLITCH",
                    CssClass = "hero-fx-glitch",
                    DurationMs = 1000,
                    TriggerEvents = "error,warning",
                    RoleAssignments = "Tech,Admin",
                    BehaviorMoods = "frustration",
                    VoiceType = "chaos",
                    SoundEffectPath = "/sounds/glitch.mp3",
                    IsPremium = true,
                    FxPackCategory = "motion pro",
                    CreatedBy = userId,
                    UpdatedBy = userId
                },
                new()
                {
                    Name = "stretch",
                    DisplayName = "Stretch",
                    CssClass = "hero-fx-stretch",
                    DurationMs = 800,
                    TriggerEvents = "update,edit",
                    RoleAssignments = "CSR,Admin",
                    BehaviorMoods = "calm",
                    VoiceType = "calm",
                    SoundEffectPath = "/sounds/stretch.mp3",
                    CreatedBy = userId,
                    UpdatedBy = userId
                }
            };

            _context.HeroImpactEffects.AddRange(defaultEffects);
            await _context.SaveChangesAsync();

            _logger.LogInformation("HeroFX default effects seeded successfully");
        }

        #endregion
    }
}