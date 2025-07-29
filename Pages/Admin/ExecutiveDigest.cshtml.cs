using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Pages.Admin;
using MVP_Core.Services;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,CEO")]
    public class ExecutiveDigestModel : PageModel
    {
        private readonly ExecutiveSnapshotService _snapshotService;
        private readonly IAuditLogger _auditLogger;
        private readonly INotificationService _notificationService;
        public ExecutiveSnapshotViewModel Snapshot { get; set; } = new();

        public ExecutiveDigestModel(ExecutiveSnapshotService snapshotService, IAuditLogger auditLogger, INotificationService notificationService)
        {
            _snapshotService = snapshotService;
            _auditLogger = auditLogger;
            _notificationService = notificationService;
        }

        public async Task OnGetAsync()
        {
            Snapshot = await _snapshotService.GetSnapshotAsync();
        }
    }
}
