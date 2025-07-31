// ===============================
// File: Pages/Admin/AuditLogs.cshtml.cs
// ===============================
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data;
using Services;

namespace Pages.Admin
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
