using Services.Dispatch;
using Services.Storage;
using Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Services.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using MVP_Core.Models;
using MVP_Core.Data.Models.PatchAnalytics;

namespace MVP_Core.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SlaDriftAnalyzerService _slaDriftAnalyzerService;
        private readonly StorageMonitorService _storageMonitorService;
        private readonly SmartAdminAlertsService _smartAdminAlertsService;
        // Sprint 85.0 — Admin Drop Alert UI + Toast Integration
        private readonly TrustScoreDropAlertService _trustScoreDropAlertService;
        // Sprint 85.2 — Admin Metrics Dashboard Integration
        private readonly AdminAnalyticsService _adminAnalyticsService;

        public DashboardModel(ApplicationDbContext context, SlaDriftAnalyzerService slaDriftAnalyzerService, StorageMonitorService storageMonitorService, SmartAdminAlertsService smartAdminAlertsService, TrustScoreDropAlertService trustScoreDropAlertService, AdminAnalyticsService adminAnalyticsService)
        {
            _context = context;
            _slaDriftAnalyzerService = slaDriftAnalyzerService;
            _storageMonitorService = storageMonitorService;
            _smartAdminAlertsService = smartAdminAlertsService;
            _trustScoreDropAlertService = trustScoreDropAlertService;
            _adminAnalyticsService = adminAnalyticsService;
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
        public int PendingEscalationCount { get; set; }

        // Sprint 85.0 — Admin Drop Alert UI + Toast Integration
        public List<TechnicianAlertLog> LatestDropAlerts { get; set; } = new();
        public int DropAlertCount { get; set; }

        // Sprint 85.2 — Admin Metrics Dashboard Integration
        public AdminMetricsViewModel Metrics { get; set; } = new();
        public List<(string TechName, TechnicianBadge Badge)> CurrentBadgeHolders { get; set; } = new();

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
            // Inject escalation count
            var escalationMatrix = new MVP_Core.Services.Admin.DispatcherEscalationMatrix(_context);
            PendingEscalationCount = escalationMatrix.GetPendingEscalationCount();

            // Sprint 85.0 — Admin Drop Alert UI + Toast Integration
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            LatestDropAlerts = await _context.TechnicianAlertLogs
                .Include(a => a.TechnicianId)
                .Where(a => a.TriggeredAt >= sevenDaysAgo)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();
            DropAlertCount = LatestDropAlerts.Count;

            // Sprint 85.2 — Admin Metrics Dashboard Integration
            Metrics = new AdminMetricsViewModel
            {
                TechniciansFlaggedThisWeek = _adminAnalyticsService.GetTechniciansFlaggedThisWeek(),
                TrustScoreDrops7Days = _adminAnalyticsService.GetTrustScoreDrops7Days(),
                CoachingLogs14Days = _adminAnalyticsService.GetCoachingLogs14Days(),
                AvgRebuildSuggestionsPerTech = _adminAnalyticsService.GetAvgRebuildSuggestionsPerTech()
            };
            // Sprint 91.22.5 - Patch Badge Visuals: Get current week's badge holders
            var weekAgo = DateTime.UtcNow.AddDays(-7);
            CurrentBadgeHolders = _context.TechnicianBadges
                .Where(b => b.EarnedOn >= weekAgo)
                .Join(_context.Technicians, b => b.TechnicianId, t => t.Id, (b, t) => new { t.FullName, Badge = b })
                .OrderByDescending(x => x.Badge.EarnedOn)
                .Select(x => (x.FullName, x.Badge))
                .ToList();
        }

        public async Task<IActionResult> OnPostAcknowledgeAlertAsync(int alertId, string actionTaken)
        {
            await _smartAdminAlertsService.AcknowledgeAlertAsync(alertId, AdminUserId, actionTaken);
            return RedirectToPage();
        }
    }
}
