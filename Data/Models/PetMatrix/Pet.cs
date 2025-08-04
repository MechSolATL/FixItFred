using System.ComponentModel.DataAnnotations;

namespace Data.Models.PetMatrix
{
    /// <summary>
    /// Represents a virtual pet in the PetMatrix Protocol system.
    /// Pets have species-specific behaviors, trust levels, and reaction patterns.
    /// </summary>
    public class Pet
    {
        [Key]
        public Guid PetId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string Species { get; set; } = ""; // cat, dog, raccoon, rabbit, etc.

        [Required]
        [StringLength(50)]
        public string Family { get; set; } = ""; // feline, canine, matrix_creature

        public float TrustLevel { get; set; } = 0.0f; // 0.0 to 100.0

        public DateTime LastFed { get; set; } = DateTime.UtcNow.AddDays(-1);

        public DateTime LastInteraction { get; set; } = DateTime.UtcNow.AddDays(-1);

        public int TrickCount { get; set; } = 0;

        public bool IsFullyTrusted { get; set; } = false;

        public bool IsInMatrixMode { get; set; } = false;

        public DateTime? MatrixModeEndTime { get; set; }

        [StringLength(50)]
        public string CurrentMood { get; set; } = "neutral"; // happy, hungry, playful, suspicious, etc.

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the time-based hangout multiplier based on trust level and last interaction.
        /// </summary>
        public float GetHangoutMultiplier()
        {
            var hoursSinceLastInteraction = (DateTime.UtcNow - LastInteraction).TotalHours;
            var baseMultiplier = TrustLevel / 100.0f;
            
            // Reduce multiplier if pet hasn't been interacted with recently
            if (hoursSinceLastInteraction > 24)
                baseMultiplier *= 0.5f;
            else if (hoursSinceLastInteraction > 12)
                baseMultiplier *= 0.75f;

            return Math.Max(0.1f, baseMultiplier);
        }

        /// <summary>
        /// Determines if the pet is hungry based on last feeding time.
        /// </summary>
        public bool IsHungry()
        {
            var hoursSinceLastFed = (DateTime.UtcNow - LastFed).TotalHours;
            return hoursSinceLastFed > 8; // Pets get hungry every 8 hours
        }

        /// <summary>
        /// Updates pet mood based on current conditions.
        /// </summary>
        public void UpdateMood()
        {
            if (IsInMatrixMode)
            {
                CurrentMood = "matrix_glitched";
                return;
            }

            if (IsHungry())
            {
                CurrentMood = "hungry";
            }
            else if (TrustLevel >= 90)
            {
                CurrentMood = "devoted";
            }
            else if (TrustLevel >= 70)
            {
                CurrentMood = "happy";
            }
            else if (TrustLevel >= 40)
            {
                CurrentMood = "content";
            }
            else if (TrustLevel >= 20)
            {
                CurrentMood = "wary";
            }
            else
            {
                CurrentMood = "suspicious";
            }

            UpdatedAt = DateTime.UtcNow;
        }
    }
}