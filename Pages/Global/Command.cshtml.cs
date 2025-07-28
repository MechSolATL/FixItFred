using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.ViewModels;
using MVP_Core.Models.ViewModels;
using MVP_Core.Services.Metrics;
using MVP_Core.Services.Actions;
using MVP_Core.Services.Diagnostics;
using MVP_Core.Services.Compliance;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MVP_Core.Pages.Global
{
    public class CommandModel : PageModel
    {
        private readonly MetricsEngineService _metricsEngine;
        private readonly ForecastingEngine _forecastingEngine;
        private readonly ProActionQueueEngine _proActionQueueEngine;
        private readonly ProcessMonitorService _processMonitorService;
        private readonly ComplianceEnforcementService _complianceService;
        private readonly DiagnosticsAIService _diagnosticsAIService;
        private readonly ComplianceReminderService _reminderService;
        private readonly AIWarningQueueEngine _warningEngine;
        private readonly ApplicationDbContext _db;
        public CommandModel(MetricsEngineService metricsEngine, ForecastingEngine forecastingEngine, ProActionQueueEngine proActionQueueEngine, ProcessMonitorService processMonitorService, ComplianceEnforcementService complianceService, DiagnosticsAIService diagnosticsAIService, ComplianceReminderService reminderService, AIWarningQueueEngine warningEngine, ApplicationDbContext db)
        {
            _metricsEngine = metricsEngine;
            _forecastingEngine = forecastingEngine;
            _proActionQueueEngine = proActionQueueEngine;
            _processMonitorService = processMonitorService;
            _complianceService = complianceService;
            _diagnosticsAIService = diagnosticsAIService;
            _reminderService = reminderService;
            _warningEngine = warningEngine;
            _db = db;
        }
        public List<MetricsCardViewModel> Metrics { get; set; } = new();
        public ForecastMetricsViewModel? Forecast { get; set; }
        public List<ProActionCardViewModel> ProActions { get; set; } = new();
        public List<ProcessStatusViewModel> ProcessStatuses { get; set; } = new();
        public List<Tenant> LockedOutTenants { get; set; } = new();
        public List<MVP_Core.Data.Models.ComplianceAlertLog> AlertSummary { get; set; } = new();
        public List<DiagnosticsAlertResult> DiagnosticsAlerts { get; set; } = new();
        public async Task OnGetAsync()
        {
            Metrics = await _metricsEngine.GetGlobalMetricsAsync();
            Forecast = await _forecastingEngine.GetForecastAsync();
            ProActions = _proActionQueueEngine.GetProActions();
            ProcessStatuses = await _processMonitorService.GetAllProcessStatusesAsync();
            DiagnosticsAlerts = await _diagnosticsAIService.GetLatestAlertsAsync();
            AlertSummary = _reminderService.GetActiveReminders();
            // Example: Find tenants with expired docs (replace with real logic)
            LockedOutTenants = _db.Tenants.Where(t => t.CompanyName.Contains("Lockout")).ToList();
        }
        public async Task<IActionResult> OnPostOverrideLockoutAsync(string TenantId, string AdminNote)
        {
            if (Guid.TryParse(TenantId, out var tenantGuid))
            {
                await _complianceService.LogUploadAsync(tenantGuid, "ALL", "OverrideLockout", AdminNote);
                // TODO: Remove lockout flag
            }
            return RedirectToPage();
        }
    }
}
