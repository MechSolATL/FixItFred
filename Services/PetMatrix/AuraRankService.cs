using Data;
using Data.Models.PetMatrix;
using Microsoft.EntityFrameworkCore;

namespace Services.PetMatrix
{
    /// <summary>
    /// Service for managing aura points, MOD ranks, and Watchtower access.
    /// Implements the $1 = 1 Aura point economy and 1000-point Watchtower unlock system.
    /// </summary>
    public class AuraRankService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuraRankService> _logger;

        public AuraRankService(ApplicationDbContext context, ILogger<AuraRankService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets or creates an AuraRank record for a user.
        /// </summary>
        public async Task<AuraRank> GetOrCreateAuraRankAsync(string userId)
        {
            var auraRank = await _context.AuraRanks.FirstOrDefaultAsync(ar => ar.UserId == userId);
            
            if (auraRank == null)
            {
                auraRank = new AuraRank
                {
                    UserId = userId,
                    AuraPoints = 0m,
                    TotalSpent = 0m,
                    ModRank = "None",
                    AuraColor = "none",
                    ModSkin = "default"
                };

                _context.AuraRanks.Add(auraRank);
                await _context.SaveChangesAsync();

                _logger.LogInformation("[Sprint135_PetMatrix] Created new AuraRank for user {UserId}", userId);
            }

            return auraRank;
        }

        /// <summary>
        /// Adds aura points based on a purchase ($1 = 1 Aura point).
        /// </summary>
        public async Task<AuraUpdateResult> AddAuraPurchaseAsync(string userId, decimal amountSpent)
        {
            var auraRank = await GetOrCreateAuraRankAsync(userId);
            var previousRank = auraRank.ModRank;
            var previousWatchtowerAccess = auraRank.HasWatchtowerAccess;

            auraRank.AddPurchase(amountSpent);
            await _context.SaveChangesAsync();

            var result = new AuraUpdateResult
            {
                Success = true,
                AuraPointsAdded = amountSpent,
                NewAuraPoints = auraRank.AuraPoints,
                NewRank = auraRank.ModRank,
                RankChanged = previousRank != auraRank.ModRank,
                WatchtowerUnlocked = !previousWatchtowerAccess && auraRank.HasWatchtowerAccess,
                AuraColor = auraRank.AuraColor,
                ModSkin = auraRank.ModSkin
            };

            if (result.RankChanged)
            {
                _logger.LogInformation("[Sprint135_PetMatrix] User {UserId} promoted from {OldRank} to {NewRank} with {AuraPoints} aura points", 
                    userId, previousRank, auraRank.ModRank, auraRank.AuraPoints);
            }

            if (result.WatchtowerUnlocked)
            {
                _logger.LogInformation("[Sprint135_PetMatrix] User {UserId} unlocked Watchtower access at {AuraPoints} aura points", 
                    userId, auraRank.AuraPoints);
            }

            return result;
        }

        /// <summary>
        /// Updates a user's subscription status for loyalty benefits.
        /// </summary>
        public async Task<bool> UpdateSubscriptionStatusAsync(string userId, bool isSubscriber, int subscriptionYears)
        {
            var auraRank = await GetOrCreateAuraRankAsync(userId);
            var wasManager = auraRank.ModRank == "Manager";

            auraRank.IsLoyaltySubscriber = isSubscriber;
            auraRank.SubscriptionYears = subscriptionYears;

            if (isSubscriber && auraRank.SubscriptionStartDate == null)
            {
                auraRank.SubscriptionStartDate = DateTime.UtcNow.AddYears(-subscriptionYears);
            }

            auraRank.CheckForPromotions();
            await _context.SaveChangesAsync();

            if (!wasManager && auraRank.ModRank == "Manager")
            {
                _logger.LogInformation("[Sprint135_PetMatrix] Long-term subscriber {UserId} ({Years} years) auto-promoted to Manager with {Skin} skin", 
                    userId, subscriptionYears, auraRank.ModSkin);
                return true; // Promotion occurred
            }

            return false;
        }

        /// <summary>
        /// Updates Watchtower proximity status for a user.
        /// </summary>
        public async Task<WatchtowerProximityResult> UpdateWatchtowerProximityAsync(string userId, bool isNear, float distance = 0f)
        {
            var auraRank = await GetOrCreateAuraRankAsync(userId);
            var wasNear = auraRank.IsNearWatchtower;

            auraRank.UpdateWatchtowerProximity(isNear);
            await _context.SaveChangesAsync();

            var result = new WatchtowerProximityResult
            {
                Success = true,
                IsNear = isNear,
                Distance = distance,
                AuraEffect = auraRank.GetAuraEffect(),
                HasOracleAccess = auraRank.HasOracleHoverState(),
                ProximityChanged = wasNear != isNear
            };

            if (result.ProximityChanged && isNear && auraRank.HasWatchtowerAccess)
            {
                // Check for nearby MODs to create proximity records
                await UpdateNearbyModProximityAsync(userId, distance);
            }

            return result;
        }

        /// <summary>
        /// Gets all MODs currently near the Watchtower for proximity effects.
        /// </summary>
        public async Task<List<NearbyMod>> GetNearbyModsAsync(string userId)
        {
            var userAuraRank = await GetOrCreateAuraRankAsync(userId);
            
            if (!userAuraRank.IsNearWatchtower || !userAuraRank.HasWatchtowerAccess)
            {
                return new List<NearbyMod>();
            }

            var nearbyMods = await _context.AuraRanks
                .Where(ar => ar.UserId != userId && 
                           ar.IsNearWatchtower && 
                           ar.HasWatchtowerAccess && 
                           ar.ModRank != "None")
                .Select(ar => new NearbyMod
                {
                    UserId = ar.UserId,
                    ModRank = ar.ModRank,
                    AuraPoints = ar.AuraPoints,
                    AuraColor = ar.AuraColor,
                    ModSkin = ar.ModSkin,
                    Distance = Random.Shared.Next(5, 100), // Simulated distance
                    IsGlowing = true
                })
                .ToListAsync();

            return nearbyMods;
        }

        /// <summary>
        /// Gets leaderboard of top aura earners.
        /// </summary>
        public async Task<List<AuraLeaderboardEntry>> GetAuraLeaderboardAsync(int limit = 10)
        {
            return await _context.AuraRanks
                .Where(ar => ar.AuraPoints > 0)
                .OrderByDescending(ar => ar.AuraPoints)
                .Take(limit)
                .Select(ar => new AuraLeaderboardEntry
                {
                    UserId = ar.UserId,
                    AuraPoints = ar.AuraPoints,
                    ModRank = ar.ModRank,
                    AuraColor = ar.AuraColor,
                    ModSkin = ar.ModSkin,
                    HasWatchtowerAccess = ar.HasWatchtowerAccess,
                    WatchtowerVisits = ar.WatchtowerVisitCount
                })
                .ToListAsync();
        }

        /// <summary>
        /// Gets manager metrics for high-ranking users.
        /// </summary>
        public async Task<List<ManagerMetrics>> GetManagerMetricsAsync()
        {
            var managers = await _context.AuraRanks
                .Where(ar => ar.ModRank == "Manager")
                .ToListAsync();

            return managers
                .Select(ar => ar.GetManagerMetrics())
                .Where(m => m != null)
                .Cast<ManagerMetrics>()
                .OrderByDescending(m => m.TotalAura)
                .ToList();
        }

        /// <summary>
        /// Force promotes a user to a specific rank (admin function).
        /// </summary>
        public async Task<bool> ForcePromoteUserAsync(string userId, string rank, string? skin = null)
        {
            if (rank != "None" && rank != "Moderator" && rank != "Manager")
            {
                return false;
            }

            var auraRank = await GetOrCreateAuraRankAsync(userId);
            auraRank.ModRank = rank;
            auraRank.ModPromotionDate = DateTime.UtcNow;

            auraRank.AuraColor = rank switch
            {
                "Moderator" => "silver",
                "Manager" => "gold",
                _ => "none"
            };

            if (rank == "Manager" && !string.IsNullOrEmpty(skin))
            {
                auraRank.ModSkin = skin;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("[Sprint135_PetMatrix] Admin force-promoted user {UserId} to {Rank}", userId, rank);
            return true;
        }

        /// <summary>
        /// Creates proximity records for nearby MODs.
        /// </summary>
        private async Task UpdateNearbyModProximityAsync(string userId, float userDistance)
        {
            var nearbyMods = await _context.AuraRanks
                .Where(ar => ar.UserId != userId && 
                           ar.IsNearWatchtower && 
                           ar.HasWatchtowerAccess)
                .ToListAsync();

            foreach (var nearbyMod in nearbyMods)
            {
                var distance = Math.Abs(userDistance - Random.Shared.Next(0, 50)); // Simulate relative distance
                
                var proximity = new WatchtowerProximity
                {
                    UserId = userId,
                    NearbyUserId = nearbyMod.UserId,
                    Distance = distance,
                    IsGlowing = distance <= 25f,
                    IsFaintInDistance = distance > 25f && distance <= 50f,
                    ProximityStart = DateTime.UtcNow
                };

                _context.WatchtowerProximities.Add(proximity);
            }

            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Result of an aura point update operation.
    /// </summary>
    public class AuraUpdateResult
    {
        public bool Success { get; set; }
        public decimal AuraPointsAdded { get; set; }
        public decimal NewAuraPoints { get; set; }
        public string NewRank { get; set; } = "";
        public bool RankChanged { get; set; }
        public bool WatchtowerUnlocked { get; set; }
        public string AuraColor { get; set; } = "";
        public string ModSkin { get; set; } = "";
    }

    /// <summary>
    /// Result of a Watchtower proximity update.
    /// </summary>
    public class WatchtowerProximityResult
    {
        public bool Success { get; set; }
        public bool IsNear { get; set; }
        public float Distance { get; set; }
        public string AuraEffect { get; set; } = "";
        public bool HasOracleAccess { get; set; }
        public bool ProximityChanged { get; set; }
    }

    /// <summary>
    /// Information about a nearby MOD at the Watchtower.
    /// </summary>
    public class NearbyMod
    {
        public string UserId { get; set; } = "";
        public string ModRank { get; set; } = "";
        public decimal AuraPoints { get; set; }
        public string AuraColor { get; set; } = "";
        public string ModSkin { get; set; } = "";
        public float Distance { get; set; }
        public bool IsGlowing { get; set; }
        public string ProximityEffect => GetProximityEffect();

        private string GetProximityEffect()
        {
            if (Distance <= 10f) return "bright_glow";
            if (Distance <= 25f) return "medium_glow";
            if (Distance <= 50f) return "faint_glow";
            return "distant_shimmer";
        }
    }

    /// <summary>
    /// Aura leaderboard entry.
    /// </summary>
    public class AuraLeaderboardEntry
    {
        public string UserId { get; set; } = "";
        public decimal AuraPoints { get; set; }
        public string ModRank { get; set; } = "";
        public string AuraColor { get; set; } = "";
        public string ModSkin { get; set; } = "";
        public bool HasWatchtowerAccess { get; set; }
        public int WatchtowerVisits { get; set; }
    }
}