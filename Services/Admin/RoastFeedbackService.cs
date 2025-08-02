using Data.Models;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Services.Admin
{
    // Sprint 73.8: Karma Feedback Loop + Roast Responder Metrics
    public class RoastFeedbackService
    {
        private readonly ApplicationDbContext _db;
        public RoastFeedbackService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Log a reaction to a roast
        public async Task LogReactionAsync(int roastId, string userId, string reactionType)
        {
            var reaction = new RoastReactionLog
            {
                RoastId = roastId,
                UserId = userId,
                ReactionType = reactionType,
                SubmittedAt = DateTime.UtcNow
            };
            _db.RoastReactionLogs.Add(reaction);
            await _db.SaveChangesAsync();
        }

        // Calculate average reaction score for a roast
        public async Task<double> CalculateAverageReactions(int roastId)
        {
            // For emoji/stars, you may want to map ReactionType to a score
            var reactions = await _db.RoastReactionLogs.Where(r => r.RoastId == roastId).ToListAsync();
            if (!reactions.Any()) return 0;
            // Example: map stars/emoji to int
            var scores = reactions.Select(r => r.ReactionType switch
            {
                "star1" => 1,
                "star2" => 2,
                "star3" => 3,
                "star4" => 4,
                "star5" => 5,
                _ => 0
            });
            return scores.Average();
        }

        // Promote roast to Legendary tier if score > 4.8 and reactions >= 10
        public async Task<bool> PromoteRoastToLegendaryTier(int roastId)
        {
            var avgScore = await CalculateAverageReactions(roastId);
            var count = await _db.RoastReactionLogs.CountAsync(r => r.RoastId == roastId);
            if (avgScore > 4.8 && count >= 10)
            {
                var roast = await _db.NewHireRoastLogs.FindAsync(roastId);
                if (roast != null)
                {
                    roast.RoastLevel = 99; // Legendary tier
                    await _db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<int> GetReactionCountAsync(int roastId)
        {
            return await _db.RoastReactionLogs.CountAsync(r => r.RoastId == roastId);
        }
    }
}
