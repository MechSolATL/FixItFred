using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;

namespace Controllers.Api
{
    [ApiController]
    [Route("api/mentor-mode")]
    public class MentorModeApiController : ControllerBase
    {
        private readonly UserPerformanceLevelEngine _performanceEngine;

        public MentorModeApiController(UserPerformanceLevelEngine performanceEngine)
        {
            _performanceEngine = performanceEngine;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleMentorMode([FromBody] MentorModeToggleRequest request)
        {
            try
            {
                // Check if user has mentor mode unlocked
                var isUnlocked = await _performanceEngine.IsMentorModeUnlockedAsync(request.UserId);
                if (!isUnlocked)
                {
                    return BadRequest(new { message = "Mentor Mode is not unlocked for this user" });
                }

                // Toggle mentor mode
                await _performanceEngine.SetMentorModeAsync(request.UserId, request.Enabled);

                return Ok(new { 
                    success = true,
                    enabled = request.Enabled,
                    message = $"Mentor Mode {(request.Enabled ? "enabled" : "disabled")} successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating Mentor Mode", details = ex.Message });
            }
        }

        [HttpGet("status/{userId}")]
        public async Task<IActionResult> GetMentorModeStatus(int userId)
        {
            try
            {
                var profile = await _performanceEngine.GetUserPerformanceProfileAsync(userId);
                var performanceLevel = await _performanceEngine.GetUserPerformanceLevelAsync(userId);

                return Ok(new
                {
                    userId = userId,
                    performanceLevel = performanceLevel,
                    mentorModeUnlocked = profile.MentorModeUnlocked,
                    mentorModeEnabled = profile.MentorModeEnabled,
                    lastToggled = profile.MentorModeLastToggled
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving Mentor Mode status", details = ex.Message });
            }
        }
    }

    public class MentorModeToggleRequest
    {
        public int UserId { get; set; }
        public bool Enabled { get; set; }
    }
}