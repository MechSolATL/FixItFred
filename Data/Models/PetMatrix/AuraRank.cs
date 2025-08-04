using System.ComponentModel.DataAnnotations;

namespace Data.Models.PetMatrix
{
    /// <summary>
    /// Represents a user's aura points and MOD rank status in the PetMatrix Protocol.
    /// $1 spent = 1 Aura point. 1000 points unlocks Watchtower access.
    /// </summary>
    public class AuraRank
    {
        [Key]
        public Guid AuraRankId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = "";

        public decimal AuraPoints { get; set; } = 0m; // $1 = 1 Aura point

        public decimal TotalSpent { get; set; } = 0m; // Total money spent

        public bool HasWatchtowerAccess { get; set; } = false; // Unlocked at 1000 points

        [StringLength(50)]
        public string ModRank { get; set; } = "None"; // None, Moderator, Manager

        public DateTime? ModPromotionDate { get; set; }

        public bool IsLoyaltySubscriber { get; set; } = false; // 10-year subscribers

        public DateTime? SubscriptionStartDate { get; set; }

        public int SubscriptionYears { get; set; } = 0;

        [StringLength(50)]
        public string AuraColor { get; set; } = "none"; // none, silver, gold, platinum

        [StringLength(50)]
        public string ModSkin { get; set; } = "default"; // rocket, UFO, motorcycle for Managers

        public bool IsNearWatchtower { get; set; } = false; // For proximity effects

        public DateTime LastWatchtowerVisit { get; set; } = DateTime.MinValue;

        public int WatchtowerVisitCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Adds aura points based on money spent ($1 = 1 point).
        /// </summary>
        public void AddPurchase(decimal amountSpent)
        {
            TotalSpent += amountSpent;
            AuraPoints += amountSpent; // $1 = 1 Aura point
            
            CheckForPromotions();
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Checks and applies automatic promotions based on aura points and subscription status.
        /// </summary>
        public void CheckForPromotions()
        {
            // Unlock Watchtower at 1000 points
            if (AuraPoints >= 1000 && !HasWatchtowerAccess)
            {
                HasWatchtowerAccess = true;
                
                // Auto-promote to Moderator if not already promoted
                if (ModRank == "None")
                {
                    ModRank = "Moderator";
                    ModPromotionDate = DateTime.UtcNow;
                    AuraColor = "silver";
                }
            }

            // Auto-promote 10-year subscribers to Manager
            if (IsLoyaltySubscriber && SubscriptionYears >= 10 && ModRank != "Manager")
            {
                ModRank = "Manager";
                ModPromotionDate = DateTime.UtcNow;
                AuraColor = "gold";
                ModSkin = GetRandomManagerSkin();
            }

            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets a random manager skin as specified in requirements.
        /// </summary>
        private string GetRandomManagerSkin()
        {
            var skins = new[] { "rocket", "UFO", "motorcycle" };
            return skins[Random.Shared.Next(skins.Length)];
        }

        /// <summary>
        /// Updates proximity status to Watchtower.
        /// </summary>
        public void UpdateWatchtowerProximity(bool isNear)
        {
            IsNearWatchtower = isNear;
            
            if (isNear && HasWatchtowerAccess)
            {
                LastWatchtowerVisit = DateTime.UtcNow;
                WatchtowerVisitCount++;
            }
            
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the visual aura effect description based on rank and proximity.
        /// </summary>
        public string GetAuraEffect()
        {
            if (!HasWatchtowerAccess) return "none";

            var effect = AuraColor;
            
            if (IsNearWatchtower)
            {
                effect += "_glowing";
            }
            
            if (ModRank == "Manager")
            {
                effect += "_" + ModSkin;
            }

            return effect;
        }

        /// <summary>
        /// Determines if user has Oracle Hover State (secret state near Watchtower).
        /// </summary>
        public bool HasOracleHoverState()
        {
            return IsNearWatchtower && 
                   HasWatchtowerAccess && 
                   ModRank == "Manager" && 
                   WatchtowerVisitCount >= 50; // Secret threshold
        }

        /// <summary>
        /// Gets the current rank display name.
        /// </summary>
        public string GetRankDisplayName()
        {
            return ModRank switch
            {
                "Moderator" => $"🛡️ MOD ({AuraPoints:N0} aura)",
                "Manager" => $"👑 MANAGER ({AuraPoints:N0} aura) [{ModSkin?.ToUpper()}]",
                _ => $"User ({AuraPoints:N0} aura)"
            };
        }

        /// <summary>
        /// Gets performance metrics for managers.
        /// </summary>
        public ManagerMetrics? GetManagerMetrics()
        {
            if (ModRank != "Manager") return null;

            return new ManagerMetrics
            {
                TotalAura = AuraPoints,
                WatchtowerVisits = WatchtowerVisitCount,
                DaysAsManager = ModPromotionDate.HasValue ? 
                    (DateTime.UtcNow - ModPromotionDate.Value).Days : 0,
                HasOracleAccess = HasOracleHoverState(),
                Skin = ModSkin
            };
        }
    }

    /// <summary>
    /// Performance metrics for Manager-ranked MODs.
    /// </summary>
    public class ManagerMetrics
    {
        public decimal TotalAura { get; set; }
        public int WatchtowerVisits { get; set; }
        public int DaysAsManager { get; set; }
        public bool HasOracleAccess { get; set; }
        public string Skin { get; set; } = "default";
    }

    /// <summary>
    /// Represents proximity-based interactions with other MODs near the Watchtower.
    /// </summary>
    public class WatchtowerProximity
    {
        [Key]
        public Guid ProximityId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string NearbyUserId { get; set; } = "";

        public float Distance { get; set; } = 0f; // Simulated distance units

        public bool IsGlowing { get; set; } = false;

        public bool IsFaintInDistance { get; set; } = false;

        public DateTime ProximityStart { get; set; } = DateTime.UtcNow;

        public DateTime? ProximityEnd { get; set; }

        // Navigation properties
        public virtual AuraRank? User { get; set; }
        public virtual AuraRank? NearbyUser { get; set; }

        /// <summary>
        /// Gets the visual effect based on distance and MOD status.
        /// </summary>
        public string GetProximityEffect()
        {
            if (Distance <= 10f)
                return "bright_glow";
            else if (Distance <= 25f)
                return "medium_glow";
            else if (Distance <= 50f)
                return "faint_glow";
            else
                return "distant_shimmer";
        }
    }
}