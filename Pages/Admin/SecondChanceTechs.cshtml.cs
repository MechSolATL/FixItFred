using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MVP_Core.Pages.Admin
{
    public class SecondChanceTechsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public SecondChanceTechsModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet() { }
        public IActionResult OnPost()
        {
            int techId = int.Parse(Request.Form["TechId"]);
            int logId = int.Parse(Request.Form["LogId"]);
            bool approve = Request.Form["Approve"] == "true";
            string notes = Request.Form["OverrideNotes"];
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == techId);
            var log = _db.SecondChanceFlagLogs.FirstOrDefault(l => l.Id == logId);
            if (tech != null && log != null)
            {
                log.IsOverrideApproved = approve;
                log.OverrideNotes = notes;
                log.ReviewedBy = User?.Identity?.Name ?? "admin";
                tech.OnboardingStatus = approve ? "Approved" : "Blocked";
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
