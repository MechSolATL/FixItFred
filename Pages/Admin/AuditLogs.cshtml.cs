// ===============================
// File: Pages/Admin/AuditLogs.cshtml.cs
// ===============================
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public AuditLogsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AuditLog> Logs { get; set; } = new();

        public async Task OnGetAsync()
        {
            Logs = await _db.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(200)
                .ToListAsync();
        }
    }
}
