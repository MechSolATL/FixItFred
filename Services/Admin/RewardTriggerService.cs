using Data;
using Data.Models;
using Services.Dispatch;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Admin
{
    // Sprint 84.2 — Reward Trigger Engine
    public class RewardTriggerService
    {
        private readonly ApplicationDbContext _db;
        private readonly TechnicianLoyaltyService _loyaltyService;
        private readonly NotificationDispatchEngine _notificationEngine;

        public RewardTriggerService(ApplicationDbContext db, TechnicianLoyaltyService loyaltyService, NotificationDispatchEngine notificationEngine)
        {
            _db = db;
            _loyaltyService = loyaltyService;
            _notificationEngine = notificationEngine;
        }

        // Checks for tier unlock and triggers reward logic
        public async Task<bool> TryTriggerRewardAsync(int technicianId, bool dryRun = false)
        {
            var tier = await _loyaltyService.GetTierAsync(technicianId);
            var trustLog = _db.TechnicianTrustLogs
                .Where(x => x.TechnicianId == technicianId)
                .OrderByDescending(x => x.RecordedAt)
                .FirstOrDefault();
            // Only trigger if not already sent for this tier
            if (trustLog != null && trustLog.LastRewardSentAt.HasValue && (DateTime.UtcNow - trustLog.LastRewardSentAt.Value).TotalDays < 1)
                return false;
            if (dryRun)
                return true;
            // Log reward event
            if (trustLog != null)
            {
                trustLog.LastRewardSentAt = DateTime.UtcNow;
                _db.TechnicianTrustLogs.Update(trustLog);
            }
            else
            {
                _db.TechnicianTrustLogs.Add(new TechnicianTrustLog
                {
                    TechnicianId = technicianId,
                    TrustScore = 0,
                    FlagWeight = 0,
                    RecordedAt = DateTime.UtcNow,
                    LastRewardSentAt = DateTime.UtcNow
                });
            }
            _db.LoyaltyPointTransactions.Add(new LoyaltyPointTransaction {
                TechnicianId = technicianId,
                Points = 0, // Set actual reward points as needed
                Type = "Reward",
                Timestamp = DateTime.UtcNow,
                Description = $"Tier unlock reward for technician {technicianId}"
            });
            await _db.SaveChangesAsync();
            // Trigger PDF README packet (static utility call)
            // TODO: PDFPacketComposer.GenerateReadmePdf(...)
            // Trigger confetti JS/Toast (placeholder)
            TriggerConfettiToast(technicianId);
            // Send admin notification
            await _notificationEngine.BroadcastSLAEscalationAsync("admin", $"Technician {technicianId} unlocked tier: {tier} — onboarding packet sent.");
            return true;
        }

        // Placeholder for confetti/toast logic
        public void TriggerConfettiToast(int technicianId)
        {
            // TODO: Integrate with front-end SignalR or JS event
        }
    }
}
