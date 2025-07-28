using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Models;

namespace MVP_Core.Services.Loyalty
{
    public class LoyaltyRewardEngine
    {
        private readonly ApplicationDbContext _context;

        public LoyaltyRewardEngine(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task EvaluateProgressAsync(int technicianId)
        {
            var progress = await _context.TechProgresses
                .FirstOrDefaultAsync(p => p.TechnicianId == technicianId);

            if (progress == null)
                return;

            var milestones = await _context.TechMilestones
                .Where(m => m.IsActive)
                .OrderBy(m => m.Points ?? 0)
                .ToListAsync();

            foreach (var milestone in milestones)
            {
                if ((progress.ProgressCount >= (milestone.Points ?? 0)) &&
                    !_context.MilestoneAuditLogs.Any(x =>
                        x.TechnicianId == technicianId &&
                        x.MilestoneId == milestone.Id &&
                        x.Action == "Unlocked"))
                {
                    // Unlock milestone and log
                    _context.MilestoneAuditLogs.Add(new MilestoneAuditLog
                    {
                        TechnicianId = technicianId,
                        MilestoneId = milestone.Id,
                        Action = "Unlocked",
                        Timestamp = DateTime.UtcNow
                    });

                    // TODO: Update TrustIndex or other rewards here
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
