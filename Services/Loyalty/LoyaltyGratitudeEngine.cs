using Data.Models;
using System.Threading.Tasks;
using System;
using Data;

namespace Services.Loyalty
{
    public class LoyaltyGratitudeEngine
    {
        private readonly ApplicationDbContext _context;
        public LoyaltyGratitudeEngine(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task TriggerGratitudeAsync(int technicianId, int milestoneId)
        {
            // Example: Add a thank-you message or perk flag
            _context.TechnicianLoyaltyLogs.Add(new TechnicianLoyaltyLog
            {
                TechnicianId = technicianId,
                Points = 0,
                Tier = "Perk",
                Milestone = $"Milestone {milestoneId} unlocked",
                AwardedAt = DateTime.UtcNow,
                Notes = "Thank you for your achievement!"
            });
            await _context.SaveChangesAsync();
        }
    }
}
