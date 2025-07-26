using MVP_Core.Data.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.FollowUp
{
    public interface INotificationHelperService
    {
        Task SendFollowUpNotificationAsync(ApplicationDbContext db, int serviceRequestId, string reason, string escalationLevel);
    }

    public class NotificationHelperService : INotificationHelperService
    {
        public async Task SendFollowUpNotificationAsync(ApplicationDbContext db, int serviceRequestId, string reason, string escalationLevel)
        {
            var request = db.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);
            if (request == null) return;
            string message = escalationLevel == "Gentle"
                ? $"Hi {request.CustomerName}, we noticed you have a pending action: {reason}. Please take a moment to complete it."
                : $"Action Required: {reason} for your recent service. Please respond promptly.";
            db.FollowUpActionLogs.Add(new FollowUpActionLog
            {
                UserId = 0, // TODO: Lookup user/customer ID
                ActionType = "Email/SMS",
                TriggerType = reason,
                TriggeredAt = DateTime.UtcNow,
                Status = "Sent",
                RelatedServiceRequestId = serviceRequestId,
                EscalationLevel = escalationLevel,
                Notes = message
            });
            await db.SaveChangesAsync();
        }
    }
}
