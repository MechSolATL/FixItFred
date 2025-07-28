using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.ViewModels;
using MVP_Core.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Metrics
{
    public class MetricsEngineService
    {
        private readonly ApplicationDbContext _db;
        public MetricsEngineService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<MetricsCardViewModel>> GetGlobalMetricsAsync()
        {
            // Example metrics, replace with real queries as needed
            var activeTenants = await _db.Tenants.CountAsync(t => t.IsActive);
            var openRequests = await _db.ServiceRequests.CountAsync(r => r.Status != "Closed");
            var totalRevenue = await _db.BillingInvoiceRecords.SumAsync(i => (decimal?)i.Amount) ?? 0;
            var techViolations = await _db.TechnicianAuditLogs.CountAsync();
            var systemAlerts = await _db.AdminAlertLogs.CountAsync(a => a.IsActive);

            return new List<MetricsCardViewModel>
            {
                new MetricsCardViewModel { Title = "Active Tenants", Value = activeTenants.ToString(), Icon = "fa-users", LinkTo = "/Admin/TenantDashboard" },
                new MetricsCardViewModel { Title = "Open Requests", Value = openRequests.ToString(), Icon = "fa-tasks", LinkTo = "/Admin/ServiceRequests" },
                new MetricsCardViewModel { Title = "Total Revenue", Value = "$" + totalRevenue.ToString("N0"), Icon = "fa-dollar-sign", LinkTo = "/Admin/Billing" },
                new MetricsCardViewModel { Title = "Tech Violations", Value = techViolations.ToString(), Icon = "fa-exclamation-triangle", RiskLevel = techViolations > 0 ? "High" : "Low", LinkTo = "/Admin/TechAudit" },
                new MetricsCardViewModel { Title = "System Alerts", Value = systemAlerts.ToString(), Icon = "fa-bell", RiskLevel = systemAlerts > 0 ? "Medium" : "Low", LinkTo = "/Admin/SystemStatus" }
            };
        }
    }
}
