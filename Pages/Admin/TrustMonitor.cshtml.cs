using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    public class TrustMonitorModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly TechnicianTrustEngineService _trustEngine;

        public TrustMonitorModel(ApplicationDbContext db, TechnicianTrustEngineService trustEngine)
        {
            _db = db;
            _trustEngine = trustEngine;
        }

        public void OnGet()
        {
            // Data loaded in Razor
        }

        public async Task<IActionResult> OnPostFlag(int id)
        {
            // Example: Add a warning log for technician
            _db.TechnicianWarningLogs.Add(new TechnicianWarningLog { TechnicianId = id, Reason = "Flagged for review", Timestamp = DateTime.UtcNow });
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearFlags(int id)
        {
            var warnings = _db.TechnicianWarningLogs.Where(w => w.TechnicianId == id).ToList();
            _db.TechnicianWarningLogs.RemoveRange(warnings);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
