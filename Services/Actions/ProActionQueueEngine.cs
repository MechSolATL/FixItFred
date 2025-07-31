using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models.ViewModels;
using Data;

namespace Services.Actions
{
    // Sprint 89.1 — Phase 3A: ProActionQueueEngine created for tactical decision generation
    public class ProActionQueueEngine
    {
        private readonly ApplicationDbContext _db;
        public ProActionQueueEngine(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<ProActionCardViewModel> GetProActions()
        {
            var actions = new List<ProActionCardViewModel>
            {
                new ProActionCardViewModel
                {
                    Title = "Coach Technician on Timeliness",
                    Description = "Technician #42 has repeated late arrivals. Recommend scheduling a coaching session.",
                    RiskLevel = "High",
                    ActionLabel = "Schedule Coaching",
                    ActionUrl = "/Admin/CoachingLog?techId=42"
                },
                new ProActionCardViewModel
                {
                    Title = "Acknowledge Customer Feedback",
                    Description = "Recent negative feedback detected for Technician #17. Consider sending a follow-up.",
                    RiskLevel = "Medium",
                    ActionLabel = "Send Follow-up",
                    ActionUrl = "/Admin/TrustTimeline?techId=17"
                }
            };

            // Sprint 89.1: Notify admin of unpaid invoices over 14 days
            var overdueInvoices = _db.BillingInvoiceRecords
                .Where(i => !i.IsPaid && i.CreatedAt < DateTime.UtcNow.AddDays(-14))
                .ToList();
            if (overdueInvoices.Any())
            {
                actions.Add(new ProActionCardViewModel
                {
                    Title = "Unpaid Invoices Alert",
                    Description = $"There are {overdueInvoices.Count} unpaid invoices over 14 days old. Notify admin for review.",
                    RiskLevel = "High",
                    ActionLabel = "Review Invoices",
                    ActionUrl = "/Admin/Billing"
                });
            }

            return actions;
        }
    }
}
