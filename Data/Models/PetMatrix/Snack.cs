using System.ComponentModel.DataAnnotations;

namespace Data.Models.PetMatrix
{
    /// <summary>
    /// Represents a snack item in the Chase-powered snack shop.
    /// Snacks have different effects on pets and can trigger various behaviors.
    /// </summary>
    public class Snack
    {
        [Key]
        public Guid SnackId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [StringLength(255)]
        public string Description { get; set; } = "";

        [Required]
        public decimal Price { get; set; } = 0.01m; // In dollars

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = "GOOSENECK Brands Inc."; // GOOSENECK or ACME Factory Error

        [StringLength(50)]
        public string FlavorType { get; set; } = ""; // Optional flavor add-on

        public decimal FlavorPrice { get; set; } = 0.25m; // Additional cost for flavors

        public bool IsRisky { get; set; } = false; // "Use at your own risk" items

        public bool IsMislabeled { get; set; } = false; // Can cause Matrix-mode

        public float TrustBonus { get; set; } = 10.0f; // Trust level increase when fed

        public float HappinessBonus { get; set; } = 5.0f; // Mood improvement

        [StringLength(50)]
        public string TargetFamily { get; set; } = "all"; // feline, canine, matrix_creature, or all

        [StringLength(500)]
        public string EffectDescription { get; set; } = "";

        [StringLength(100)]
        public string SpecialEffect { get; set; } = ""; // hiccups, dancing, burping, howling, matrix_mode

        public int EffectDurationMinutes { get; set; } = 30; // How long effects last

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the total price including flavor add-on.
        /// </summary>
        public decimal GetTotalPrice()
        {
            return Price + (string.IsNullOrEmpty(FlavorType) ? 0 : FlavorPrice);
        }

        /// <summary>
        /// Determines if this snack is compatible with the given pet family.
        /// </summary>
        public bool IsCompatibleWith(string petFamily)
        {
            return TargetFamily == "all" || TargetFamily.Equals(petFamily, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets a risk warning message for risky items.
        /// </summary>
        public string GetRiskWarning()
        {
            if (!IsRisky) return "";
            
            return "⚠️ Use at your own risk - unpredictable effects may occur!";
        }

        /// <summary>
        /// Calculates the effective trust bonus based on pet compatibility and special conditions.
        /// </summary>
        public float CalculateEffectiveTrustBonus(Pet pet)
        {
            var bonus = TrustBonus;

            // Compatibility bonus
            if (IsCompatibleWith(pet.Family))
                bonus *= 1.2f;

            // Hungry pet bonus
            if (pet.IsHungry())
                bonus *= 1.5f;

            // Mislabeled items may have unpredictable effects
            if (IsMislabeled && Random.Shared.NextDouble() < 0.3) // 30% chance
                bonus = 0; // No trust gain, but might trigger Matrix mode

            return bonus;
        }
    }

    /// <summary>
    /// Represents a snack purchase transaction.
    /// </summary>
    public class SnackPurchase
    {
        [Key]
        public Guid PurchaseId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SnackId { get; set; }

        [Required]
        public Guid PetId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = "";

        public int Quantity { get; set; } = 1;

        public decimal TotalPrice { get; set; }

        [StringLength(50)]
        public string FlavorUsed { get; set; } = "";

        public DateTime PurchaseTime { get; set; } = DateTime.UtcNow;

        public bool EffectTriggered { get; set; } = false;

        [StringLength(100)]
        public string EffectTriggeredType { get; set; } = "";

        // Navigation properties
        public virtual Snack? Snack { get; set; }
        public virtual Pet? Pet { get; set; }
    }

    /// <summary>
    /// Predefined snack types as specified in the requirements.
    /// </summary>
    public static class SnackCatalog
    {
        public static readonly List<Snack> DefaultSnacks = new()
        {
            new Snack
            {
                Name = "Rotten Fish",
                Description = "Budget-friendly option for training new pets. Sold in bulk.",
                Price = 0.01m,
                Brand = "GOOSENECK Brands Inc.",
                TrustBonus = 2.0f,
                HappinessBonus = 1.0f,
                TargetFamily = "feline",
                EffectDescription = "Basic trust building for cats",
                SpecialEffect = "none"
            },
            new Snack
            {
                Name = "Rotten Fish (10,000 bag)",
                Description = "Bulk purchase saves money! Perfect for dedicated pet owners.",
                Price = 1.00m,
                Brand = "GOOSENECK Brands Inc.",
                TrustBonus = 25.0f,
                HappinessBonus = 10.0f,
                TargetFamily = "feline",
                EffectDescription = "Massive trust boost for cats",
                SpecialEffect = "none"
            },
            new Snack
            {
                Name = "Juicy Steak",
                Description = "Premium quality meat for discerning pets.",
                Price = 7.99m,
                Brand = "GOOSENECK Brands Inc.",
                TrustBonus = 30.0f,
                HappinessBonus = 25.0f,
                TargetFamily = "canine",
                EffectDescription = "High-quality protein for dogs",
                SpecialEffect = "happiness_boost"
            },
            new Snack
            {
                Name = "Mega Feast",
                Description = "The ultimate pet dining experience. Luxury at its finest.",
                Price = 49.00m,
                Brand = "GOOSENECK Brands Inc.",
                TrustBonus = 50.0f,
                HappinessBonus = 40.0f,
                TargetFamily = "all",
                EffectDescription = "Ultimate trust and happiness boost",
                SpecialEffect = "mega_happiness",
                EffectDurationMinutes = 120
            },
            new Snack
            {
                Name = "Mystery Meat Batch #X1",
                Description = "ACME Factory Error - Contents unknown",
                Price = 2.99m,
                Brand = "ACME Factory Error",
                TrustBonus = 15.0f,
                HappinessBonus = 10.0f,
                TargetFamily = "all",
                IsRisky = true,
                IsMislabeled = true,
                EffectDescription = "Unpredictable effects - may cause Matrix glitches",
                SpecialEffect = "matrix_mode",
                EffectDurationMinutes = 180 // 1-3 hours as specified
            }
        };

        /// <summary>
        /// Available flavor types that can be added to snacks.
        /// </summary>
        public static readonly List<string> AvailableFlavors = new()
        {
            "Spicy (causes hiccups)",
            "Fizzy (causes dancing)",
            "Fermented (causes burping)",
            "Aromatic (causes howling)",
            "Experimental (random effect)"
        };
    }
}