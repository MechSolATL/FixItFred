using System.ComponentModel.DataAnnotations;

namespace Data.Models.PetMatrix
{
    /// <summary>
    /// Represents horn trigger system for different pet families.
    /// 3 horn types trigger different pet groups as specified in requirements.
    /// </summary>
    public class HornTrigger
    {
        [Key]
        public Guid HornTriggerId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(10)]
        public string HornType { get; set; } = ""; // A, B, C

        [Required]
        [StringLength(50)]
        public string TargetFamily { get; set; } = ""; // feline, canine, matrix_creature

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = "";

        public DateTime TriggerTime { get; set; } = DateTime.UtcNow;

        public int PetsResponded { get; set; } = 0;

        [StringLength(100)]
        public string Effect { get; set; } = ""; // call_effect, attention_effect, etc.

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets the family that responds to this horn type.
        /// </summary>
        public static string GetTargetFamily(string hornType)
        {
            return hornType.ToUpper() switch
            {
                "A" => "feline",      // Horn A → Trigger cat family
                "B" => "canine",      // Horn B → Trigger dog family  
                "C" => "matrix_creature", // Horn C → Trigger matrix creatures (raccoon, rabbit, etc.)
                _ => "none"
            };
        }

        /// <summary>
        /// Gets the description of what this horn does.
        /// </summary>
        public static string GetHornDescription(string hornType)
        {
            return hornType.ToUpper() switch
            {
                "A" => "🔔 Feline Horn - Calls all cats within range",
                "B" => "🎺 Canine Horn - Summons loyal dogs",
                "C" => "📯 Matrix Horn - Awakens mysterious creatures",
                _ => "Unknown horn type"
            };
        }
    }

    /// <summary>
    /// Represents a pet's interaction session including tricks, feeding, and play.
    /// </summary>
    public class PetInteraction
    {
        [Key]
        public Guid InteractionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PetId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string InteractionType { get; set; } = ""; // feed, play, trick, horn_call

        [StringLength(100)]
        public string ItemUsed { get; set; } = ""; // Snack name, toy name, horn type

        public float TrustGained { get; set; } = 0f;

        public float HappinessGained { get; set; } = 0f;

        public bool TrickPerformed { get; set; } = false;

        [StringLength(100)]
        public string TrickType { get; set; } = ""; // sit, fetch, unique_trick

        public bool SpecialEffectTriggered { get; set; } = false;

        [StringLength(100)]
        public string SpecialEffect { get; set; } = ""; // matrix_mode, hiccups, dancing, etc.

        public DateTime InteractionTime { get; set; } = DateTime.UtcNow;

        public int Duration { get; set; } = 30; // Duration in seconds

        // Navigation property
        public virtual Pet? Pet { get; set; }

        /// <summary>
        /// Gets a description of what happened during this interaction.
        /// </summary>
        public string GetInteractionDescription()
        {
            var description = InteractionType switch
            {
                "feed" => $"Fed {Pet?.Name} with {ItemUsed}",
                "play" => $"Played with {Pet?.Name} using {ItemUsed}",
                "trick" => $"{Pet?.Name} performed {TrickType}",
                "horn_call" => $"Called {Pet?.Name} with {ItemUsed}",
                _ => $"Interacted with {Pet?.Name}"
            };

            if (SpecialEffectTriggered)
            {
                description += $" - Special effect: {SpecialEffect}";
            }

            return description;
        }
    }

    /// <summary>
    /// Tracks pet behavioral patterns and unique tricks for fully trusted pets.
    /// </summary>
    public class PetBehaviorPattern
    {
        [Key]
        public Guid PatternId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PetId { get; set; }

        [Required]
        [StringLength(100)]
        public string BehaviorType { get; set; } = ""; // reaction, trick, mood_change

        [StringLength(255)]
        public string TriggerCondition { get; set; } = ""; // What causes this behavior

        [StringLength(255)]
        public string BehaviorDescription { get; set; } = ""; // What the pet does

        public bool IsUniqueTrick { get; set; } = false; // Only for fully trusted pets

        public bool RequiresFullTrust { get; set; } = false;

        public float TrustThreshold { get; set; } = 0f; // Minimum trust required

        [StringLength(50)]
        public string MoodRequired { get; set; } = "any"; // happy, content, etc.

        public int TimesPerformed { get; set; } = 0;

        public DateTime FirstPerformed { get; set; } = DateTime.UtcNow;

        public DateTime LastPerformed { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual Pet? Pet { get; set; }

        /// <summary>
        /// Checks if this behavior can be performed by the given pet.
        /// </summary>
        public bool CanBePerformed(Pet pet)
        {
            if (!IsActive) return false;
            if (pet.TrustLevel < TrustThreshold) return false;
            if (RequiresFullTrust && !pet.IsFullyTrusted) return false;
            if (MoodRequired != "any" && pet.CurrentMood != MoodRequired) return false;
            if (pet.IsInMatrixMode && BehaviorType != "matrix_glitch") return false;

            return true;
        }

        /// <summary>
        /// Records that this behavior was performed.
        /// </summary>
        public void RecordPerformance()
        {
            TimesPerformed++;
            LastPerformed = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Predefined behavior patterns for different pet species and trust levels.
    /// </summary>
    public static class PetBehaviorCatalog
    {
        public static readonly List<PetBehaviorPattern> DefaultBehaviors = new()
        {
            // Cat behaviors
            new PetBehaviorPattern
            {
                BehaviorType = "reaction",
                TriggerCondition = "Horn A activated",
                BehaviorDescription = "Perks ears and approaches cautiously",
                TrustThreshold = 10f,
                MoodRequired = "any"
            },
            new PetBehaviorPattern
            {
                BehaviorType = "trick",
                TriggerCondition = "Given favorite snack when happy",
                BehaviorDescription = "Purrs and rubs against screen",
                TrustThreshold = 30f,
                MoodRequired = "happy"
            },
            new PetBehaviorPattern
            {
                BehaviorType = "unique_trick",
                TriggerCondition = "Fully trusted cat shown affection",
                BehaviorDescription = "Performs mystical cat dance with sparkling effects",
                RequiresFullTrust = true,
                IsUniqueTrick = true,
                MoodRequired = "devoted"
            },

            // Dog behaviors
            new PetBehaviorPattern
            {
                BehaviorType = "reaction",
                TriggerCondition = "Horn B activated",
                BehaviorDescription = "Barks excitedly and runs to screen",
                TrustThreshold = 5f,
                MoodRequired = "any"
            },
            new PetBehaviorPattern
            {
                BehaviorType = "trick",
                TriggerCondition = "Fed quality treat",
                BehaviorDescription = "Sits, stays, and wags tail vigorously",
                TrustThreshold = 20f,
                MoodRequired = "happy"
            },
            new PetBehaviorPattern
            {
                BehaviorType = "unique_trick",
                TriggerCondition = "Fully trusted dog given command",
                BehaviorDescription = "Performs impossible backflip with rainbow trail",
                RequiresFullTrust = true,
                IsUniqueTrick = true,
                MoodRequired = "devoted"
            },

            // Matrix creature behaviors
            new PetBehaviorPattern
            {
                BehaviorType = "reaction",
                TriggerCondition = "Horn C activated",
                BehaviorDescription = "Materializes from digital static",
                TrustThreshold = 15f,
                MoodRequired = "any"
            },
            new PetBehaviorPattern
            {
                BehaviorType = "matrix_glitch",
                TriggerCondition = "Consumed mislabeled treat",
                BehaviorDescription = "Phases in and out of reality, tries to hack interface",
                TrustThreshold = 0f,
                MoodRequired = "matrix_glitched"
            },
            new PetBehaviorPattern
            {
                BehaviorType = "unique_trick",
                TriggerCondition = "Fully trusted matrix creature given freedom",
                BehaviorDescription = "Opens portal to digital realm and invites user inside",
                RequiresFullTrust = true,
                IsUniqueTrick = true,
                MoodRequired = "devoted"
            }
        };
    }
}