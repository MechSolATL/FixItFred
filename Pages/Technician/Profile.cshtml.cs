using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Admin;
using MVP_Core.Services.Admin;
using MVP_Core.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Technician
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly RewardTriggerService _rewardTriggerService;

        public TierStatusDto? TierStatus { get; set; }

        public ProfileModel(ApplicationDbContext db, RewardTriggerService rewardTriggerService)
        {
            _db = db;
            _rewardTriggerService = rewardTriggerService;
        }

        public async Task OnGetAsync()
        {
            var techId = User.Identity?.Name != null ? int.Parse(User.Identity.Name) : 0;
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == techId);
            if (tech != null)
            {
                var trustLog = _db.TechnicianTrustLogs
                    .Where(x => x.TechnicianId == techId)
                    .OrderByDescending(x => x.RecordedAt)
                    .FirstOrDefault();
                var currentTier = ""; // TODO: Get from loyalty/tier logic
                var totalPoints = _db.LoyaltyPointTransactions
                    .Where(l => l.TechnicianId == techId)
                    .Sum(l => l.Points);
                var lastReward = trustLog?.LastRewardSentAt;
                var eligible = trustLog == null || !lastReward.HasValue || (System.DateTime.UtcNow - lastReward.Value).TotalDays >= 1;
                TierStatus = new TierStatusDto
                {
                    TechnicianId = tech.Id,
                    TechnicianName = tech.FullName,
                    CurrentTier = currentTier,
                    TotalPoints = totalPoints,
                    LastRewardSentAt = lastReward,
                    IsEligibleForReward = eligible
                };
            }
        }
    }
}
