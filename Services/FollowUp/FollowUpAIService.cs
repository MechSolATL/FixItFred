using MVP_Core.Data.Models;
using System.Threading.Tasks;
using System;

namespace MVP_Core.Services.FollowUp
{
    public class FollowUpAIService
    {
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
                _ => "GeneralFollowUp"
            };
            // Log outcome (stub)
            Console.WriteLine($"[FollowUpAI] Triggered {messageType} for SR#{serviceRequestId} due to {reason}");
            // TODO: Integrate with email/SMS/SignalR
            await Task.CompletedTask;
        }
    }
}
