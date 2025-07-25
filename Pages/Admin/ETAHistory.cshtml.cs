using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    // Sprint 32 - Admin Reschedule Logic
    [Authorize(Roles = "Admin,Dispatcher,Supervisor")]
    public class ETAHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<ETAHistoryEntryView> Entries { get; set; } = new();
        public ETAHistoryModel(ApplicationDbContext db) { _db = db; }
        public void OnGet()
        {
            Entries = _db.ETAHistoryEntries
                .OrderByDescending(e => e.Timestamp)
                .Select(e => new ETAHistoryEntryView
                {
                    Timestamp = e.Timestamp,
                    TechnicianName = e.TechnicianName,
                    JobAddress = _db.ServiceRequests.FirstOrDefault(r => r.Id == e.ServiceRequestId)!.Address ?? "",
                    PreviousETA = e.PredictedETA,
                    NewETA = e.ActualArrival,
                    UpdateReason = e.Message
                })
                .ToList();
        }
        public class ETAHistoryEntryView
        {
            public DateTime Timestamp { get; set; }
            public string TechnicianName { get; set; } = string.Empty;
            public string JobAddress { get; set; } = string.Empty;
            public DateTime? PreviousETA { get; set; }
            public DateTime? NewETA { get; set; }
            public string UpdateReason { get; set; } = string.Empty;
        }
    }
}
