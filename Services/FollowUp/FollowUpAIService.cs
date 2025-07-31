using MVP_Core.Data.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Services.FollowUp
{
    public class FollowUpAIService
    {
        private readonly ApplicationDbContext _db;
        private readonly INotificationHelperService _notificationHelper;
        public FollowUpAIService(ApplicationDbContext db, INotificationHelperService notificationHelper)
        {
            _db = db;
            _notificationHelper = notificationHelper;
        }

        // Trigger follow-up for a specific scenario
        public async Task TriggerFollowUp(int serviceRequestId, string reason)
        {
            // Sprint 50.1: AI-powered follow-up logic
            // Determine message type
            string messageType = reason switch
            {
                "MissedConfirmation" => "Reminder",
                "UnassignedJob" => "Escalation",
                "RepeatedSLABreach" => "Urgency",
                "FailedContact" => "CourtesyFollowUp",
                "NoReview" => "ReviewReminder",
                "UnclaimedReward" => "RewardReminder",
                "BonusNoReschedule" => "RescheduleReminder",
                _ => "GeneralFollowUp"
            };
            // Escalation logic
            string escalationLevel = "Gentle";
            var logCount = await _db.FollowUpActionLogs.CountAsync(l => l.RelatedServiceRequestId == serviceRequestId && l.TriggerType == reason);
            if (logCount > 1) escalationLevel = "Assertive";

            // Log follow-up action
            _db.FollowUpActionLogs.Add(new FollowUpActionLog
            {
                UserId = 0, // TODO: Lookup user/customer ID
                ActionType = "Email/SMS",
                TriggerType = reason,
                TriggeredAt = DateTime.UtcNow,
                Status = "Pending",
                RelatedServiceRequestId = serviceRequestId,
                EscalationLevel = escalationLevel
            });
            await _db.SaveChangesAsync();

            // Auto-schedule notification
            await _notificationHelper.SendFollowUpNotificationAsync(_db, serviceRequestId, reason, escalationLevel);
        }

        // Automated triggers for inactivity, unclaimed rewards, pending reviews, etc.
        public async Task RunAutomatedFollowUpChecksAsync()
        {
            var now = DateTime.UtcNow;
            // No review left after X days (e.g., 3 days)
            var reviewThreshold = now.AddDays(-3);
            var noReviewRequests = _db.ServiceRequests
                .Where(r => r.ClosedAt != null && r.ClosedAt < reviewThreshold && !_db.CustomerReviews.Any(cr => cr.ServiceRequestId == r.Id))
                .ToList();
            foreach (var req in noReviewRequests)
                await TriggerFollowUp(req.Id, "NoReview");

            // Reward unclaimed for Y hours (e.g., 24 hours)
            var rewardThreshold = now.AddHours(-24);
            var unclaimedRewards = _db.LoyaltyPointTransactions
                .Where(l => l.Type == "Bonus" && l.Timestamp < rewardThreshold && (l.Description == null || !l.Description.Contains("Claimed")))
                .ToList();
            foreach (var reward in unclaimedRewards)
            {
                var reqId = reward.RelatedReviewId ?? 0;
                await TriggerFollowUp(reqId, "UnclaimedReward");
            }

            // Bonus issued, no reschedule
            var bonusIssued = _db.LoyaltyPointTransactions
                .Where(l => l.Type == "Bonus" && l.Timestamp < now.AddDays(-2))
                .ToList();
            foreach (var bonus in bonusIssued)
            {
                var customer = _db.Customers.FirstOrDefault(c => c.Id == bonus.CustomerId);
                if (customer != null)
                {
                    var hasRescheduled = _db.ServiceRequests.Any(r => r.Email == customer.Email && r.CreatedAt > bonus.Timestamp);
                    if (!hasRescheduled)
                        await TriggerFollowUp(bonus.RelatedReviewId ?? 0, "BonusNoReschedule");
                }
            }
        }

        // Sprint 52.0: No Review After X Days follow-up logic
        public async Task RunReviewFollowUpAsync()
        {
            var now = DateTime.UtcNow;
            var threeDaysAgo = now.AddDays(-3);
            var sevenDaysAgo = now.AddDays(-7);
            // ServiceRequest marked as Completed, no CustomerReview after 3 days
            var completedRequests = _db.ServiceRequests
                .Where(r => r.Status == "Completed" && r.ClosedAt != null)
                .ToList();
            foreach (var req in completedRequests)
            {
                bool hasReview = _db.CustomerReviews.Any(cr => cr.ServiceRequestId == req.Id);
                if (!hasReview && req.ClosedAt < threeDaysAgo)
                {
                    // Check if already sent
                    var alreadySent = _db.FollowUpActionLogs.Any(l => l.RelatedServiceRequestId == req.Id && l.TriggerType == "NoReview" && l.Status == "Sent");
                    if (!alreadySent)
                    {
                        string reviewMsg = "We noticed your recent service is complete, but no review has been submitted yet.\nYour feedback helps us improve transparency, measure technician performance, and continuously elevate your experience.\nEven a quick rating helps keep our service honest, accountable, and responsive.";
                        // Log and send follow-up
                        _db.FollowUpActionLogs.Add(new FollowUpActionLog
                        {
                            UserId = 0, // TODO: Lookup user/customer ID
                            ActionType = "Email/SMS",
                            TriggerType = "NoReview",
                            TriggeredAt = DateTime.UtcNow,
                            Status = "Sent",
                            RelatedServiceRequestId = req.Id,
                            EscalationLevel = "Gentle",
                            Notes = reviewMsg
                        });
                        await _db.SaveChangesAsync();
                        // TODO: Integrate with EmailService to send review link
                    }
                }
                // If no action after 7 days, show gentle reminder banner
                if (!hasReview && req.ClosedAt < sevenDaysAgo)
                {
                    var bannerExists = _db.FollowUpActionLogs.Any(l => l.RelatedServiceRequestId == req.Id && l.TriggerType == "NoReviewBanner" && l.Status == "Active");
                    if (!bannerExists)
                    {
                        _db.FollowUpActionLogs.Add(new FollowUpActionLog
                        {
                            UserId = 0, // TODO: Lookup user/customer ID
                            ActionType = "Banner",
                            TriggerType = "NoReviewBanner",
                            TriggeredAt = DateTime.UtcNow,
                            Status = "Active",
                            RelatedServiceRequestId = req.Id,
                            EscalationLevel = "Gentle",
                            Notes = "Gentle review reminder banner active."
                        });
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
