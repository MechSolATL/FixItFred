using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Data;

namespace Pages.Admin
{
    public class SecondChanceTechsModel : PageModel
    {
        private readonly ApplicationDbContext _db; // Sprint 79.2
        public SecondChanceTechsModel(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Sprint 79.2
        }
        public void OnGet() { }
        public IActionResult OnPost()
        {
            int techId = int.TryParse(Request?.Form?["TechId"].ToString(), out var tid) ? tid : 0; // Sprint 79.2
            int logId = int.TryParse(Request?.Form?["LogId"].ToString(), out var lid) ? lid : 0; // Sprint 79.2
            bool approve = (Request?.Form?["Approve"].ToString() ?? "") == "true"; // Sprint 79.2
            string notes = Request?.Form?["OverrideNotes"].ToString() ?? string.Empty; // Sprint 79.2
            var tech = _db?.Technicians?.FirstOrDefault(t => t.Id == techId); // Sprint 79.2
            var log = _db?.SecondChanceFlagLogs?.FirstOrDefault(l => l.Id == logId); // Sprint 79.2
            if (tech != null && log != null)
            {
                log.IsOverrideApproved = approve;
                log.OverrideNotes = notes;
                log.ReviewedBy = User?.Identity?.Name ?? "admin"; // Sprint 79.2
                tech.OnboardingStatus = approve ? "Approved" : "Blocked";
                _db?.SaveChanges(); // Sprint 79.2
            }
            return RedirectToPage();
        }
    }
}
