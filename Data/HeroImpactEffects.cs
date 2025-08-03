using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    /// <summary>
    /// Data model for HeroFX Studio impact effects configuration
    /// Sprint127_HeroFX_StudioDivision
    /// </summary>
    public class HeroImpactEffect
    {
        /// <summary>
        /// Unique identifier for the effect
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the effect (e.g., "slam", "pop", "yeet", "glitch")
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Display name for admin interface
        /// </summary>
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// CSS class name for the effect
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CssClass { get; set; } = string.Empty;

        /// <summary>
        /// CSS animation properties
        /// </summary>
        public string? AnimationCss { get; set; }

        /// <summary>
        /// JavaScript function name for the effect
        /// </summary>
        [StringLength(100)]
        public string? JsFunction { get; set; }

        /// <summary>
        /// Effect duration in milliseconds
        /// </summary>
        [Range(100, 10000)]
        public int DurationMs { get; set; } = 1000;

        /// <summary>
        /// Effect trigger events (dispatch, login, praise, etc.)
        /// </summary>
        public string? TriggerEvents { get; set; }

        /// <summary>
        /// Persona assignments (Tech, Admin, CSR)
        /// </summary>
        public string? PersonaAssignments { get; set; }

        /// <summary>
        /// Role assignments (clean fades for CSR, explosions for Tech)
        /// </summary>
        public string? RoleAssignments { get; set; }

        /// <summary>
        /// Behavior mood assignments (calm, frustration, celebration)
        /// </summary>
        public string? BehaviorMoods { get; set; }

        /// <summary>
        /// VoiceFX hook configuration
        /// </summary>
        public string? VoiceFxConfig { get; set; }

        /// <summary>
        /// Voice type for narrator (calm vs chaos)
        /// </summary>
        [StringLength(50)]
        public string? VoiceType { get; set; }

        /// <summary>
        /// Sound effect file path
        /// </summary>
        [StringLength(255)]
        public string? SoundEffectPath { get; set; }

        /// <summary>
        /// Whether effect is available for mobile
        /// </summary>
        public bool IsMobileCompatible { get; set; } = true;

        /// <summary>
        /// Whether effect is available for desktop
        /// </summary>
        public bool IsDesktopCompatible { get; set; } = true;

        /// <summary>
        /// Whether effect is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether effect is premium/paid
        /// </summary>
        public bool IsPremium { get; set; } = false;

        /// <summary>
        /// FX Pack category (hype packs, narrator packs, motion pro)
        /// </summary>
        [StringLength(100)]
        public string? FxPackCategory { get; set; }

        /// <summary>
        /// Usage count for analytics
        /// </summary>
        public int UsageCount { get; set; } = 0;

        /// <summary>
        /// Reaction count for analytics
        /// </summary>
        public int ReactionCount { get; set; } = 0;

        /// <summary>
        /// KAPOW-to-CLICKS ratio for analytics
        /// </summary>
        public decimal KapowToClickRatio { get; set; } = 0.0m;

        /// <summary>
        /// When the effect was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the effect was last updated
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who created the effect
        /// </summary>
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// User who last updated the effect
        /// </summary>
        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// Analytics log for HeroFX usage tracking
    /// </summary>
    public class HeroFxAnalyticsLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EffectId { get; set; }

        [Required]
        [StringLength(100)]
        public string EffectName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TriggerEvent { get; set; } = string.Empty;

        [StringLength(100)]
        public string? UserId { get; set; }

        [StringLength(50)]
        public string? UserRole { get; set; }

        [StringLength(50)]
        public string? DeviceType { get; set; }

        public bool WasSuccessful { get; set; } = true;

        public bool GotReaction { get; set; } = false;

        [StringLength(255)]
        public string? ErrorMessage { get; set; }

        [Required]
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual HeroImpactEffect? Effect { get; set; }
    }
}