using Data;
using Data.Models.PetMatrix;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Services.PetMatrix
{
    /// <summary>
    /// Service for integrating PetMatrix Protocol with the existing Watchtower system.
    /// Manages MOD proximity effects, aura glow, and Oracle Hover State.
    /// </summary>
    public class WatchtowerPetMatrixService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuraRankService _auraRankService;
        private readonly HeroFXEngine _heroFxEngine;
        private readonly ILogger<WatchtowerPetMatrixService> _logger;

        public WatchtowerPetMatrixService(
            ApplicationDbContext context, 
            AuraRankService auraRankService,
            HeroFXEngine heroFxEngine,
            ILogger<WatchtowerPetMatrixService> logger)
        {
            _context = context;
            _auraRankService = auraRankService;
            _heroFxEngine = heroFxEngine;
            _logger = logger;
        }

        /// <summary>
        /// Updates the Watchtower dashboard with PetMatrix Protocol enhancements.
        /// </summary>
        public async Task<WatchtowerDashboardData> GetEnhancedWatchtowerDataAsync(string? currentUserId = null)
        {
            var auraStats = await GetAuraSystemStatsAsync();
            var nearbyMods = new List<NearbyMod>();
            var userAuraRank = (AuraRank?)null;

            if (!string.IsNullOrEmpty(currentUserId))
            {
                userAuraRank = await _auraRankService.GetOrCreateAuraRankAsync(currentUserId);
                if (userAuraRank.IsNearWatchtower && userAuraRank.HasWatchtowerAccess)
                {
                    nearbyMods = await _auraRankService.GetNearbyModsAsync(currentUserId);
                }
            }

            var recentInteractions = await GetRecentPetMatrixActivityAsync();
            var matrixGlitches = await GetActiveMatrixGlitchesAsync();

            return new WatchtowerDashboardData
            {
                AuraSystemStats = auraStats,
                UserAuraRank = userAuraRank,
                NearbyMods = nearbyMods,
                RecentActivity = recentInteractions,
                ActiveMatrixGlitches = matrixGlitches,
                OracleHoverAvailable = userAuraRank?.HasOracleHoverState() ?? false
            };
        }

        /// <summary>
        /// Enters or exits the Oracle Hover State for qualified Manager MODs.
        /// </summary>
        public async Task<OracleHoverResult> ToggleOracleHoverStateAsync(string userId)
        {
            var auraRank = await _auraRankService.GetOrCreateAuraRankAsync(userId);

            if (!auraRank.HasOracleHoverState())
            {
                return new OracleHoverResult
                {
                    Success = false,
                    Message = "Oracle Hover State requires Manager rank with 50+ Watchtower visits while near the tower"
                };
            }

            // Oracle Hover State provides access to system-wide insights
            var oracleData = await GetOracleInsightsAsync();

            // Trigger special Oracle FX
            await _heroFxEngine.TriggerEffectAsync("oracle_hover_activation", userId);

            _logger.LogInformation("[Sprint135_PetMatrix] User {UserId} activated Oracle Hover State", userId);

            return new OracleHoverResult
            {
                Success = true,
                Message = "Oracle Hover State activated - System insights unlocked",
                SystemInsights = oracleData,
                AuraEffect = "oracle_glow_" + auraRank.ModSkin
            };
        }

        /// <summary>
        /// Simulates user entering Watchtower proximity.
        /// </summary>
        public async Task<WatchtowerProximityResult> EnterWatchtowerProximityAsync(string userId, float distance = 15f)
        {
            var result = await _auraRankService.UpdateWatchtowerProximityAsync(userId, true, distance);

            if (result.Success && result.ProximityChanged)
            {
                // Trigger proximity-based visual effects
                await _heroFxEngine.TriggerEffectAsync("watchtower_proximity_enter", userId);
                
                // Add to live feed
                await LogWatchtowerActivity(userId, "Approached Watchtower", "proximity");

                _logger.LogInformation("[Sprint135_PetMatrix] User {UserId} entered Watchtower proximity at distance {Distance}", 
                    userId, distance);
            }

            return result;
        }

        /// <summary>
        /// Simulates user leaving Watchtower proximity.
        /// </summary>
        public async Task<WatchtowerProximityResult> ExitWatchtowerProximityAsync(string userId)
        {
            var result = await _auraRankService.UpdateWatchtowerProximityAsync(userId, false);

            if (result.Success && result.ProximityChanged)
            {
                // End any active proximity records
                var activeProximities = await _context.WatchtowerProximities
                    .Where(wp => wp.UserId == userId && wp.ProximityEnd == null)
                    .ToListAsync();

                foreach (var proximity in activeProximities)
                {
                    proximity.ProximityEnd = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                await _heroFxEngine.TriggerEffectAsync("watchtower_proximity_exit", userId);
                await LogWatchtowerActivity(userId, "Left Watchtower vicinity", "proximity");

                _logger.LogInformation("[Sprint135_PetMatrix] User {UserId} exited Watchtower proximity", userId);
            }

            return result;
        }

        /// <summary>
        /// Gets real-time HERO bar view of nearby MODs for Matrix window effect.
        /// </summary>
        public async Task<List<HeroBarMod>> GetHeroBarViewAsync(string userId)
        {
            var userAuraRank = await _auraRankService.GetOrCreateAuraRankAsync(userId);

            if (!userAuraRank.HasWatchtowerAccess)
            {
                return new List<HeroBarMod>();
            }

            var nearbyMods = await _auraRankService.GetNearbyModsAsync(userId);

            return nearbyMods.Select(mod => new HeroBarMod
            {
                ModRank = mod.ModRank,
                AuraEffect = mod.AuraColor + (mod.Distance <= 25f ? "_glowing" : "_faint"),
                ModSkin = mod.ModSkin,
                Distance = mod.Distance,
                IsVisible = mod.Distance <= 100f, // Only show MODs within 100 units
                GlowIntensity = CalculateGlowIntensity(mod.Distance)
            }).ToList();
        }

        /// <summary>
        /// Generates system-wide analytics for Watchtower overview.
        /// </summary>
        public async Task<PetMatrixSystemAnalytics> GetSystemAnalyticsAsync()
        {
            var totalPets = await _context.Pets.CountAsync();
            var activePets = await _context.Pets.CountAsync(p => !p.IsInMatrixMode);
            var matrixPets = await _context.Pets.CountAsync(p => p.IsInMatrixMode);
            var fullyTrustedPets = await _context.Pets.CountAsync(p => p.IsFullyTrusted);

            var totalAuraEarned = await _context.AuraRanks.SumAsync(ar => ar.AuraPoints);
            var totalMods = await _context.AuraRanks.CountAsync(ar => ar.ModRank != "None");
            var watchtowerUsers = await _context.AuraRanks.CountAsync(ar => ar.HasWatchtowerAccess);

            var recentPurchases = await _context.SnackPurchases
                .Where(sp => sp.PurchaseTime >= DateTime.UtcNow.AddHours(-24))
                .SumAsync(sp => sp.TotalPrice);

            var uniqueTricksToday = await _context.PetInteractions
                .Where(pi => pi.InteractionTime >= DateTime.UtcNow.AddDays(-1) && 
                           pi.TrickPerformed && pi.TrickType == "unique_trick")
                .CountAsync();

            return new PetMatrixSystemAnalytics
            {
                TotalPets = totalPets,
                ActivePets = activePets,
                MatrixGlitchedPets = matrixPets,
                FullyTrustedPets = fullyTrustedPets,
                TotalAuraGenerated = totalAuraEarned,
                TotalMods = totalMods,
                WatchtowerUsers = watchtowerUsers,
                Revenue24Hours = recentPurchases,
                UniqueTricksToday = uniqueTricksToday,
                SystemHealth = CalculateSystemHealth(totalPets, activePets, matrixPets)
            };
        }

        /// <summary>
        /// Gets statistics for the aura point economy system.
        /// </summary>
        private async Task<AuraSystemStats> GetAuraSystemStatsAsync()
        {
            var totalUsers = await _context.AuraRanks.CountAsync();
            var totalAura = await _context.AuraRanks.SumAsync(ar => ar.AuraPoints);
            var moderators = await _context.AuraRanks.CountAsync(ar => ar.ModRank == "Moderator");
            var managers = await _context.AuraRanks.CountAsync(ar => ar.ModRank == "Manager");
            var watchtowerUsers = await _context.AuraRanks.CountAsync(ar => ar.HasWatchtowerAccess);
            var nearbyUsers = await _context.AuraRanks.CountAsync(ar => ar.IsNearWatchtower);

            return new AuraSystemStats
            {
                TotalUsers = totalUsers,
                TotalAuraPoints = totalAura,
                Moderators = moderators,
                Managers = managers,
                WatchtowerUsers = watchtowerUsers,
                NearbyUsers = nearbyUsers
            };
        }

        /// <summary>
        /// Gets recent PetMatrix activity for the live feed.
        /// </summary>
        private async Task<List<PetMatrixActivity>> GetRecentPetMatrixActivityAsync(int limit = 10)
        {
            var interactions = await _context.PetInteractions
                .Include(pi => pi.Pet)
                .Where(pi => pi.InteractionTime >= DateTime.UtcNow.AddHours(-2))
                .OrderByDescending(pi => pi.InteractionTime)
                .Take(limit)
                .ToListAsync();

            return interactions.Select(i => new PetMatrixActivity
            {
                Time = i.InteractionTime,
                Event = i.GetInteractionDescription(),
                Type = i.InteractionType,
                UserId = i.UserId,
                Severity = i.SpecialEffectTriggered ? "Warning" : "Info"
            }).ToList();
        }

        /// <summary>
        /// Gets pets currently in Matrix mode for monitoring.
        /// </summary>
        private async Task<List<MatrixGlitchStatus>> GetActiveMatrixGlitchesAsync()
        {
            var matrixPets = await _context.Pets
                .Where(p => p.IsInMatrixMode && (p.MatrixModeEndTime == null || p.MatrixModeEndTime > DateTime.UtcNow))
                .ToListAsync();

            return matrixPets.Select(p => new MatrixGlitchStatus
            {
                PetName = p.Name,
                Species = p.Species,
                UserId = p.UserId,
                GlitchStartTime = p.LastFed, // Approximate glitch start
                EstimatedEndTime = p.MatrixModeEndTime,
                GlitchDurationMinutes = p.MatrixModeEndTime.HasValue ? 
                    (int)(p.MatrixModeEndTime.Value - DateTime.UtcNow).TotalMinutes : 0
            }).ToList();
        }

        /// <summary>
        /// Gets Oracle-level system insights for qualified users.
        /// </summary>
        private async Task<OracleInsights> GetOracleInsightsAsync()
        {
            var systemAnalytics = await GetSystemAnalyticsAsync();
            var topSpenders = await _context.AuraRanks
                .OrderByDescending(ar => ar.TotalSpent)
                .Take(5)
                .Select(ar => new { ar.UserId, ar.TotalSpent, ar.ModRank })
                .ToListAsync();

            var riskItemSales = await _context.SnackPurchases
                .Include(sp => sp.Snack)
                .Where(sp => sp.Snack != null && sp.Snack.IsRisky)
                .CountAsync();

            return new OracleInsights
            {
                SystemAnalytics = systemAnalytics,
                TopSpenders = topSpenders.Select(ts => $"{ts.ModRank} {ts.UserId}: ${ts.TotalSpent:N2}").ToList(),
                RiskItemsPurchased = riskItemSales,
                SystemThreatLevel = CalculateSystemThreatLevel(systemAnalytics),
                PredictedGrowth = "📈 Aura economy expanding steadily"
            };
        }

        /// <summary>
        /// Logs Watchtower activity to the live feed.
        /// </summary>
        private async Task LogWatchtowerActivity(string userId, string description, string activityType)
        {
            // This would integrate with the existing live feed system
            // For now, we'll log it for future integration
            _logger.LogInformation("[Sprint135_PetMatrix_Watchtower] {UserId}: {Description} ({Type})", 
                userId, description, activityType);
        }

        /// <summary>
        /// Calculates glow intensity based on distance.
        /// </summary>
        private float CalculateGlowIntensity(float distance)
        {
            if (distance <= 10f) return 1.0f;
            if (distance <= 25f) return 0.7f;
            if (distance <= 50f) return 0.4f;
            return 0.1f;
        }

        /// <summary>
        /// Calculates overall system health percentage.
        /// </summary>
        private string CalculateSystemHealth(int totalPets, int activePets, int matrixPets)
        {
            if (totalPets == 0) return "Stable";
            
            var healthPercentage = (float)activePets / totalPets * 100;
            var matrixPercentage = (float)matrixPets / totalPets * 100;

            if (matrixPercentage > 20) return "Critical";
            if (matrixPercentage > 10) return "Warning";
            if (healthPercentage > 80) return "Optimal";
            return "Stable";
        }

        /// <summary>
        /// Calculates system threat level for Oracle insights.
        /// </summary>
        private string CalculateSystemThreatLevel(PetMatrixSystemAnalytics analytics)
        {
            var matrixRatio = analytics.TotalPets > 0 ? (float)analytics.MatrixGlitchedPets / analytics.TotalPets : 0f;
            
            if (matrixRatio > 0.3f) return "🔴 HIGH - Matrix contamination spreading";
            if (matrixRatio > 0.15f) return "🟡 MEDIUM - Monitor Matrix activity";
            return "🟢 LOW - System stable";
        }
    }

    // Data transfer objects for Watchtower integration

    public class WatchtowerDashboardData
    {
        public AuraSystemStats AuraSystemStats { get; set; } = new();
        public AuraRank? UserAuraRank { get; set; }
        public List<NearbyMod> NearbyMods { get; set; } = new();
        public List<PetMatrixActivity> RecentActivity { get; set; } = new();
        public List<MatrixGlitchStatus> ActiveMatrixGlitches { get; set; } = new();
        public bool OracleHoverAvailable { get; set; }
    }

    public class AuraSystemStats
    {
        public int TotalUsers { get; set; }
        public decimal TotalAuraPoints { get; set; }
        public int Moderators { get; set; }
        public int Managers { get; set; }
        public int WatchtowerUsers { get; set; }
        public int NearbyUsers { get; set; }
    }

    public class PetMatrixActivity
    {
        public DateTime Time { get; set; }
        public string Event { get; set; } = "";
        public string Type { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Severity { get; set; } = "";
    }

    public class MatrixGlitchStatus
    {
        public string PetName { get; set; } = "";
        public string Species { get; set; } = "";
        public string UserId { get; set; } = "";
        public DateTime GlitchStartTime { get; set; }
        public DateTime? EstimatedEndTime { get; set; }
        public int GlitchDurationMinutes { get; set; }
    }

    public class HeroBarMod
    {
        public string ModRank { get; set; } = "";
        public string AuraEffect { get; set; } = "";
        public string ModSkin { get; set; } = "";
        public float Distance { get; set; }
        public bool IsVisible { get; set; }
        public float GlowIntensity { get; set; }
    }

    public class PetMatrixSystemAnalytics
    {
        public int TotalPets { get; set; }
        public int ActivePets { get; set; }
        public int MatrixGlitchedPets { get; set; }
        public int FullyTrustedPets { get; set; }
        public decimal TotalAuraGenerated { get; set; }
        public int TotalMods { get; set; }
        public int WatchtowerUsers { get; set; }
        public decimal Revenue24Hours { get; set; }
        public int UniqueTricksToday { get; set; }
        public string SystemHealth { get; set; } = "";
    }

    public class OracleHoverResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public OracleInsights? SystemInsights { get; set; }
        public string AuraEffect { get; set; } = "";
    }

    public class OracleInsights
    {
        public PetMatrixSystemAnalytics SystemAnalytics { get; set; } = new();
        public List<string> TopSpenders { get; set; } = new();
        public int RiskItemsPurchased { get; set; }
        public string SystemThreatLevel { get; set; } = "";
        public string PredictedGrowth { get; set; } = "";
    }
}