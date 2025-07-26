using Services.Dispatch;
using Services.Storage;
using Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SlaDriftAnalyzerService _slaDriftAnalyzerService;
        private readonly StorageMonitorService _storageMonitorService;
        private readonly SmartAdminAlertsService _smartAdminAlertsService;

        public DashboardModel(ApplicationDbContext context, SlaDriftAnalyzerService slaDriftAnalyzerService, StorageMonitorService storageMonitorService, SmartAdminAlertsService smartAdminAlertsService)
        {
            _context = context;
            _slaDriftAnalyzerService = slaDriftAnalyzerService;
            _storageMonitorService = storageMonitorService;
            _smartAdminAlertsService = smartAdminAlertsService;
        }

        [BindProperty(SupportsGet = true)]
        public string? ServiceType { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }
        public List<ServiceRequest> ServiceRequests { get; set; } = new();
        public string SlaHeatmap { get; set; } = "No data";
        public string StorageChart { get; set; } = "No data";
        public List<string> Alerts { get; set; } = new();
        public List<AdminAlertLog> ActiveAlerts { get; set; } = new();
        public string AdminUserId => User?.Identity?.Name ?? "admin";

        public async Task OnGetAsync()
        {
            IQueryable<ServiceRequest> query = _context.ServiceRequests.AsQueryable();
            if (!string.IsNullOrEmpty(ServiceType))
                query = query.Where(r => r.ServiceType == ServiceType);
            if (!string.IsNullOrEmpty(Status))
                query = query.Where(r => r.Status == Status);
            ServiceRequests = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
            SlaHeatmap = await _slaDriftAnalyzerService.AnalyzeSlaDriftAsync() ?? "No data";
            StorageChart = await _storageMonitorService.MonitorStorageAsync() ?? "No data";
            Alerts = await _smartAdminAlertsService.TriggerAlertsAsync() ?? new List<string>();
            ActiveAlerts = await _smartAdminAlertsService.GetActiveAlertsAsync(AdminUserId);
        }

        public async Task<IActionResult> OnPostAcknowledgeAlertAsync(int alertId, string actionTaken)
        {
            await _smartAdminAlertsService.AcknowledgeAlertAsync(alertId, AdminUserId, actionTaken);
            return RedirectToPage();
        }
    }
}
