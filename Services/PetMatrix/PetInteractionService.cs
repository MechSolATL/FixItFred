using Data;
using Data.Models.PetMatrix;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Services.PetMatrix
{
    /// <summary>
    /// Service for managing pet interactions including feeding, playing, and trust building.
    /// Implements the core PetMatrix Protocol interaction mechanics.
    /// </summary>
    public class PetInteractionService
    {
        private readonly ApplicationDbContext _context;
        private readonly HeroFXEngine _heroFxEngine;
        private readonly ILogger<PetInteractionService> _logger;

        public PetInteractionService(ApplicationDbContext context, HeroFXEngine heroFxEngine, ILogger<PetInteractionService> logger)
        {
            _context = context;
            _heroFxEngine = heroFxEngine;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new pet for the specified user.
        /// </summary>
        public async Task<Pet> CreatePetAsync(string userId, string name, string species)
        {
            var family = GetPetFamily(species);
            
            var pet = new Pet
            {
                Name = name,
                Species = species,
                Family = family,
                UserId = userId,
                TrustLevel = 5.0f, // Start with minimal trust
                CurrentMood = "curious"
            };

            pet.UpdateMood();

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();

            _logger.LogInformation("[Sprint135_PetMatrix] Created new pet {PetName} ({Species}) for user {UserId}", 
                name, species, userId);

            return pet;
        }

        /// <summary>
        /// Feeds a pet with the specified snack, updating trust and triggering effects.
        /// </summary>
        public async Task<PetInteractionResult> FeedPetAsync(Guid petId, Guid snackId, string userId)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId && p.UserId == userId);
            var snack = await _context.Snacks.FirstOrDefaultAsync(s => s.SnackId == snackId);

            if (pet == null || snack == null)
            {
                return new PetInteractionResult
                {
                    Success = false,
                    Message = "Pet or snack not found"
                };
            }

            // Calculate trust and happiness gains
            var trustGain = snack.CalculateEffectiveTrustBonus(pet);
            var happinessGain = snack.HappinessBonus;

            // Apply compatibility multipliers
            if (snack.IsCompatibleWith(pet.Family))
            {
                happinessGain *= 1.3f;
            }

            // Check for special effects
            var effectTriggered = false;
            var effectType = "";

            // Matrix mode from mislabeled treats
            if (snack.IsMislabeled && Random.Shared.NextDouble() < 0.4) // 40% chance
            {
                pet.IsInMatrixMode = true;
                pet.MatrixModeEndTime = DateTime.UtcNow.AddMinutes(Random.Shared.Next(60, 180)); // 1-3 hours
                effectTriggered = true;
                effectType = "matrix_mode";
                
                // Trigger Matrix FX via HeroFX engine
                await _heroFxEngine.TriggerEffectAsync("matrix_glitch", userId);
                
                _logger.LogWarning("[Sprint135_PetMatrix] Pet {PetName} entered Matrix mode after consuming {SnackName}", 
                    pet.Name, snack.Name);
            }

            // Flavor effects
            if (!string.IsNullOrEmpty(snack.FlavorType) && !effectTriggered)
            {
                effectType = GetFlavorEffect(snack.FlavorType);
                if (!string.IsNullOrEmpty(effectType))
                {
                    effectTriggered = true;
                    await _heroFxEngine.TriggerEffectAsync(effectType, userId);
                }
            }

            // Update pet stats
            pet.TrustLevel = Math.Min(100f, pet.TrustLevel + trustGain);
            pet.LastFed = DateTime.UtcNow;
            pet.LastInteraction = DateTime.UtcNow;

            // Check for full trust achievement
            if (pet.TrustLevel >= 95f && !pet.IsFullyTrusted)
            {
                pet.IsFullyTrusted = true;
                await _heroFxEngine.TriggerEffectAsync("trust_achievement", userId);
                _logger.LogInformation("[Sprint135_PetMatrix] Pet {PetName} achieved full trust!", pet.Name);
            }

            pet.UpdateMood();

            // Record the interaction
            var interaction = new PetInteraction
            {
                PetId = petId,
                UserId = userId,
                InteractionType = "feed",
                ItemUsed = snack.Name,
                TrustGained = trustGain,
                HappinessGained = happinessGain,
                SpecialEffectTriggered = effectTriggered,
                SpecialEffect = effectType,
                Duration = snack.EffectDurationMinutes * 60 // Convert to seconds
            };

            _context.PetInteractions.Add(interaction);
            await _context.SaveChangesAsync();

            return new PetInteractionResult
            {
                Success = true,
                Message = "Pet fed successfully",
                TrustGained = trustGain,
                HappinessGained = happinessGain,
                EffectTriggered = effectTriggered,
                EffectType = effectType,
                NewTrustLevel = pet.TrustLevel,
                NewMood = pet.CurrentMood,
                IsFullyTrusted = pet.IsFullyTrusted
            };
        }

        /// <summary>
        /// Triggers a horn call to summon pets of the target family.
        /// </summary>
        public async Task<HornTriggerResult> TriggerHornAsync(string hornType, string userId)
        {
            var targetFamily = HornTrigger.GetTargetFamily(hornType);
            if (targetFamily == "none")
            {
                return new HornTriggerResult
                {
                    Success = false,
                    Message = "Invalid horn type"
                };
            }

            // Get all user's pets of the target family
            var pets = await _context.Pets
                .Where(p => p.UserId == userId && p.Family == targetFamily)
                .ToListAsync();

            var petsResponded = 0;
            var interactions = new List<PetInteraction>();

            foreach (var pet in pets)
            {
                // Check if pet responds based on trust level
                var responseChance = Math.Min(0.9, pet.TrustLevel / 100.0 * 0.8 + 0.2); // 20% base + trust bonus
                
                if (Random.Shared.NextDouble() < responseChance)
                {
                    petsResponded++;
                    
                    // Update pet interaction time
                    pet.LastInteraction = DateTime.UtcNow;
                    pet.UpdateMood();

                    // Create interaction record
                    var interaction = new PetInteraction
                    {
                        PetId = pet.PetId,
                        UserId = userId,
                        InteractionType = "horn_call",
                        ItemUsed = $"Horn {hornType}",
                        TrustGained = 2.0f, // Small trust bonus for responding
                        HappinessGained = 5.0f,
                        Duration = 15 // Horn calls are brief
                    };

                    interactions.Add(interaction);
                    pet.TrustLevel = Math.Min(100f, pet.TrustLevel + 2.0f);
                }
            }

            // Record the horn trigger
            var hornTrigger = new HornTrigger
            {
                HornType = hornType,
                TargetFamily = targetFamily,
                UserId = userId,
                PetsResponded = petsResponded,
                Effect = "call_effect"
            };

            _context.HornTriggers.Add(hornTrigger);
            _context.PetInteractions.AddRange(interactions);
            await _context.SaveChangesAsync();

            // Trigger visual FX
            await _heroFxEngine.TriggerEffectAsync($"horn_{hornType.ToLower()}_call", userId);

            _logger.LogInformation("[Sprint135_PetMatrix] Horn {HornType} triggered by user {UserId}, {PetsResponded}/{TotalPets} pets responded", 
                hornType, userId, petsResponded, pets.Count);

            return new HornTriggerResult
            {
                Success = true,
                Message = $"Horn {hornType} activated",
                HornType = hornType,
                TargetFamily = targetFamily,
                PetsResponded = petsResponded,
                TotalPets = pets.Count,
                Description = HornTrigger.GetHornDescription(hornType)
            };
        }

        /// <summary>
        /// Plays with a pet using toys, increasing trick count when happy.
        /// </summary>
        public async Task<PetInteractionResult> PlayWithPetAsync(Guid petId, string toyName, string userId)
        {
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId && p.UserId == userId);
            
            if (pet == null)
            {
                return new PetInteractionResult
                {
                    Success = false,
                    Message = "Pet not found"
                };
            }

            var trustGain = 5.0f;
            var happinessGain = 15.0f;
            var trickPerformed = false;

            // Happy pets are more likely to perform tricks
            if (pet.CurrentMood == "happy" || pet.CurrentMood == "devoted")
            {
                if (Random.Shared.NextDouble() < 0.7) // 70% chance
                {
                    trickPerformed = true;
                    pet.TrickCount++;
                    trustGain += 10.0f;
                    happinessGain += 10.0f;
                }
            }

            // Check for unique tricks for fully trusted pets
            var uniqueTrickPerformed = false;
            if (pet.IsFullyTrusted && pet.CurrentMood == "devoted" && Random.Shared.NextDouble() < 0.3) // 30% chance
            {
                uniqueTrickPerformed = true;
                await _heroFxEngine.TriggerEffectAsync($"unique_trick_{pet.Family}", userId);
                
                _logger.LogInformation("[Sprint135_PetMatrix] Pet {PetName} performed unique trick!", pet.Name);
            }

            // Update pet stats
            pet.TrustLevel = Math.Min(100f, pet.TrustLevel + trustGain);
            pet.LastInteraction = DateTime.UtcNow;
            pet.UpdateMood();

            // Record interaction
            var interaction = new PetInteraction
            {
                PetId = petId,
                UserId = userId,
                InteractionType = "play",
                ItemUsed = toyName,
                TrustGained = trustGain,
                HappinessGained = happinessGain,
                TrickPerformed = trickPerformed || uniqueTrickPerformed,
                TrickType = uniqueTrickPerformed ? "unique_trick" : (trickPerformed ? "standard_trick" : ""),
                Duration = 60 // Play sessions last about a minute
            };

            _context.PetInteractions.Add(interaction);
            await _context.SaveChangesAsync();

            return new PetInteractionResult
            {
                Success = true,
                Message = trickPerformed ? "Pet performed a trick!" : "Pet enjoyed playing",
                TrustGained = trustGain,
                HappinessGained = happinessGain,
                TrickPerformed = trickPerformed || uniqueTrickPerformed,
                TrickType = interaction.TrickType,
                NewTrustLevel = pet.TrustLevel,
                NewMood = pet.CurrentMood,
                IsFullyTrusted = pet.IsFullyTrusted
            };
        }

        /// <summary>
        /// Gets all pets for a user with their current status.
        /// </summary>
        public async Task<List<Pet>> GetUserPetsAsync(string userId)
        {
            var pets = await _context.Pets
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            // Update moods and check for matrix mode expiration
            foreach (var pet in pets)
            {
                if (pet.IsInMatrixMode && pet.MatrixModeEndTime.HasValue && DateTime.UtcNow > pet.MatrixModeEndTime)
                {
                    pet.IsInMatrixMode = false;
                    pet.MatrixModeEndTime = null;
                    _logger.LogInformation("[Sprint135_PetMatrix] Pet {PetName} recovered from Matrix mode", pet.Name);
                }
                
                pet.UpdateMood();
            }

            await _context.SaveChangesAsync();
            return pets;
        }

        /// <summary>
        /// Gets recent interaction history for a pet.
        /// </summary>
        public async Task<List<PetInteraction>> GetPetInteractionHistoryAsync(Guid petId, int limit = 10)
        {
            return await _context.PetInteractions
                .Where(pi => pi.PetId == petId)
                .Include(pi => pi.Pet)
                .OrderByDescending(pi => pi.InteractionTime)
                .Take(limit)
                .ToListAsync();
        }

        /// <summary>
        /// Determines the family classification for a pet species.
        /// </summary>
        private string GetPetFamily(string species)
        {
            return species.ToLower() switch
            {
                "cat" or "kitten" or "lynx" => "feline",
                "dog" or "puppy" or "wolf" => "canine",
                "raccoon" or "rabbit" or "fox" or "squirrel" => "matrix_creature",
                _ => "matrix_creature" // Default to matrix creatures for unusual species
            };
        }

        /// <summary>
        /// Gets the effect type for flavor additions.
        /// </summary>
        private string GetFlavorEffect(string flavorType)
        {
            if (flavorType.Contains("Spicy")) return "hiccups";
            if (flavorType.Contains("Fizzy")) return "dancing";
            if (flavorType.Contains("Fermented")) return "burping";
            if (flavorType.Contains("Aromatic")) return "howling";
            if (flavorType.Contains("Experimental")) return GetRandomEffect();
            
            return "";
        }

        /// <summary>
        /// Gets a random effect for experimental flavors.
        /// </summary>
        private string GetRandomEffect()
        {
            var effects = new[] { "hiccups", "dancing", "burping", "howling", "sparkles", "dizzy" };
            return effects[Random.Shared.Next(effects.Length)];
        }
    }

    /// <summary>
    /// Result of a pet interaction operation.
    /// </summary>
    public class PetInteractionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public float TrustGained { get; set; }
        public float HappinessGained { get; set; }
        public bool EffectTriggered { get; set; }
        public string EffectType { get; set; } = "";
        public bool TrickPerformed { get; set; }
        public string TrickType { get; set; } = "";
        public float NewTrustLevel { get; set; }
        public string NewMood { get; set; } = "";
        public bool IsFullyTrusted { get; set; }
    }

    /// <summary>
    /// Result of a horn trigger operation.
    /// </summary>
    public class HornTriggerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string HornType { get; set; } = "";
        public string TargetFamily { get; set; } = "";
        public int PetsResponded { get; set; }
        public int TotalPets { get; set; }
        public string Description { get; set; } = "";
    }
}