using Microsoft.AspNetCore.Mvc;
using Services.PetMatrix;
using Data.Models.PetMatrix;

namespace Controllers.Api.PetMatrix
{
    /// <summary>
    /// API controller for pet interactions including feeding, playing, and horn triggers.
    /// Implements the core PetMatrix Protocol interaction endpoints.
    /// </summary>
    [ApiController]
    [Route("api/petmatrix/pets")]
    public class PetInteractionController : ControllerBase
    {
        private readonly PetInteractionService _petInteractionService;
        private readonly ILogger<PetInteractionController> _logger;

        public PetInteractionController(PetInteractionService petInteractionService, ILogger<PetInteractionController> logger)
        {
            _petInteractionService = petInteractionService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new pet for the current user.
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<Pet>> CreatePet([FromBody] CreatePetRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Species))
            {
                return BadRequest("Pet name and species are required");
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var pet = await _petInteractionService.CreatePetAsync(userId, request.Name, request.Species);
                
                _logger.LogInformation("[Sprint135_PetMatrix_API] Created pet {PetName} for user {UserId}", 
                    request.Name, userId);

                return Ok(pet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error creating pet {PetName} for user {UserId}", 
                    request.Name, userId);
                return StatusCode(500, "Failed to create pet");
            }
        }

        /// <summary>
        /// Gets all pets for the current user.
        /// </summary>
        [HttpGet("my-pets")]
        public async Task<ActionResult<List<Pet>>> GetMyPets()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var pets = await _petInteractionService.GetUserPetsAsync(userId);
                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting pets for user {UserId}", userId);
                return StatusCode(500, "Failed to get pets");
            }
        }

        /// <summary>
        /// Feeds a pet with the specified snack.
        /// </summary>
        [HttpPost("{petId}/feed")]
        public async Task<ActionResult<PetInteractionResult>> FeedPet(Guid petId, [FromBody] FeedPetRequest request)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var result = await _petInteractionService.FeedPetAsync(petId, request.SnackId, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                _logger.LogInformation("[Sprint135_PetMatrix_API] User {UserId} fed pet {PetId}, trust gained: {TrustGained}", 
                    userId, petId, result.TrustGained);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error feeding pet {PetId} for user {UserId}", 
                    petId, userId);
                return StatusCode(500, "Failed to feed pet");
            }
        }

        /// <summary>
        /// Plays with a pet using the specified toy.
        /// </summary>
        [HttpPost("{petId}/play")]
        public async Task<ActionResult<PetInteractionResult>> PlayWithPet(Guid petId, [FromBody] PlayWithPetRequest request)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var result = await _petInteractionService.PlayWithPetAsync(petId, request.ToyName, userId);
                
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error playing with pet {PetId} for user {UserId}", 
                    petId, userId);
                return StatusCode(500, "Failed to play with pet");
            }
        }

        /// <summary>
        /// Triggers a horn to call pets of the target family.
        /// </summary>
        [HttpPost("horn-trigger")]
        public async Task<ActionResult<HornTriggerResult>> TriggerHorn([FromBody] HornTriggerRequest request)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            if (string.IsNullOrEmpty(request.HornType) || 
                !new[] { "A", "B", "C" }.Contains(request.HornType.ToUpper()))
            {
                return BadRequest("Invalid horn type. Must be A, B, or C");
            }

            try
            {
                var result = await _petInteractionService.TriggerHornAsync(request.HornType.ToUpper(), userId);
                
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error triggering horn {HornType} for user {UserId}", 
                    request.HornType, userId);
                return StatusCode(500, "Failed to trigger horn");
            }
        }

        /// <summary>
        /// Gets interaction history for a specific pet.
        /// </summary>
        [HttpGet("{petId}/history")]
        public async Task<ActionResult<List<PetInteraction>>> GetPetHistory(Guid petId, [FromQuery] int limit = 10)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID required");
            }

            try
            {
                var history = await _petInteractionService.GetPetInteractionHistoryAsync(petId, limit);
                
                // Filter to only show interactions from the current user's pets
                var userHistory = history.Where(h => h.UserId == userId).ToList();
                
                return Ok(userHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint135_PetMatrix_API] Error getting history for pet {PetId}", petId);
                return StatusCode(500, "Failed to get pet history");
            }
        }

        /// <summary>
        /// Gets available horn types and their descriptions.
        /// </summary>
        [HttpGet("horn-types")]
        public ActionResult<List<HornTypeInfo>> GetHornTypes()
        {
            var hornTypes = new List<HornTypeInfo>
            {
                new HornTypeInfo 
                { 
                    Type = "A", 
                    Description = HornTrigger.GetHornDescription("A"),
                    TargetFamily = HornTrigger.GetTargetFamily("A")
                },
                new HornTypeInfo 
                { 
                    Type = "B", 
                    Description = HornTrigger.GetHornDescription("B"),
                    TargetFamily = HornTrigger.GetTargetFamily("B")
                },
                new HornTypeInfo 
                { 
                    Type = "C", 
                    Description = HornTrigger.GetHornDescription("C"),
                    TargetFamily = HornTrigger.GetTargetFamily("C")
                }
            };

            return Ok(hornTypes);
        }

        /// <summary>
        /// Gets the current user ID from the request context.
        /// In a real implementation, this would extract from JWT, session, etc.
        /// </summary>
        private string GetCurrentUserId()
        {
            // For now, use a header or query parameter for testing
            // In production, this would come from authentication
            return Request.Headers["X-User-Id"].FirstOrDefault() ?? 
                   Request.Query["userId"].FirstOrDefault() ?? 
                   "demo-user";
        }
    }

    // Request/Response DTOs

    public class CreatePetRequest
    {
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";
    }

    public class FeedPetRequest
    {
        public Guid SnackId { get; set; }
    }

    public class PlayWithPetRequest
    {
        public string ToyName { get; set; } = "";
    }

    public class HornTriggerRequest
    {
        public string HornType { get; set; } = "";
    }

    public class HornTypeInfo
    {
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public string TargetFamily { get; set; } = "";
    }
}