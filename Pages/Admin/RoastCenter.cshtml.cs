using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Pages.Admin
{
    public class RoastCenterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly RoastEngineService _roastEngine;
        public RoastCenterModel(ApplicationDbContext db)
        {
            _db = db;
            _roastEngine = new RoastEngineService(db);
        }

        public List<NewHireRoastLog> PendingRoasts { get; set; } = new();
        public List<NewHireRoastLog> PastRoasts { get; set; } = new();

        public async Task OnGetAsync()
        {
            PendingRoasts = await _roastEngine.GetPendingRoastsAsync();
            PastRoasts = await _db.NewHireRoastLogs.Where(x => x.IsDelivered).OrderByDescending(x => x.DeliveredAt).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeliverAsync(int id)
        {
            await _roastEngine.DeliverRoastAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostScheduleAsync(string EmployeeId, int RoastLevel)
        {
            await _roastEngine.ScheduleRoastAsync(EmployeeId, RoastLevel);
            return RedirectToPage();
        }
    }
}
