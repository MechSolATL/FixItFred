// ===============================
// File: Pages/Admin/AuditLogs.cshtml.cs
// ===============================
using Microsoft.AspNetCore.Authorization;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AuditLogsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditLogger _auditLogger;
        private readonly AuditLogEncryptionService _encryptionService;

        public AuditLogsModel(ApplicationDbContext db, AuditLogger auditLogger, AuditLogEncryptionService encryptionService)
        {
            _db = db;
            _auditLogger = auditLogger;
            _encryptionService = encryptionService;
        }

        public List<AuditLog> Logs { get; set; } = [];

        public async Task OnGetAsync()
        {
            Logs = await _db.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(200)
                .ToListAsync();

            // Only decrypt for SuperAdmin
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                foreach (var log in Logs)
                {
                    _auditLogger.DecryptLog(log);
                }
            }
        }
    }
}
