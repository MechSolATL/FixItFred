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
            var activeTenants = await _db.Tenants.CountAsync(t => t.IsActive);
            var openRequests = await _db.ServiceRequests.CountAsync(r => r.Status != "Closed");
            var totalRevenue = await _db.BillingInvoiceRecords.SumAsync(i => (decimal?)i.AmountTotal) ?? 0;
            var techViolations = await _db.TechnicianAuditLogs.CountAsync();
            var systemAlerts = await _db.AdminAlertLogs.CountAsync(a => !a.IsResolved);

            // New metrics for Sprint 89.1
            var today = DateTime.UtcNow.Date;
            var weekAgo = today.AddDays(-7);
            var paidThisWeek = await _db.BillingInvoiceRecords.CountAsync(i => i.PaidDate >= weekAgo && i.PaidDate <= today && i.IsPaid);
            var totalThisWeek = await _db.BillingInvoiceRecords.CountAsync(i => i.CreatedAt >= weekAgo && i.CreatedAt <= today);
            var percentPaidThisWeek = totalThisWeek > 0 ? (int)(100.0 * paidThisWeek / totalThisWeek) : 0;
            var invoicedToday = await _db.BillingInvoiceRecords.Where(i => i.CreatedAt >= today).SumAsync(i => (decimal?)i.AmountTotal) ?? 0;
            var zelleCount = await _db.BillingInvoiceRecords.CountAsync(i => i.PaymentMethod == MVP_Core.Data.Enums.PaymentMethodEnum.Zelle);
            var qbCount = await _db.BillingInvoiceRecords.CountAsync(i => i.PaymentMethod == MVP_Core.Data.Enums.PaymentMethodEnum.QuickBooks);

            return new List<MetricsCardViewModel>
            {
                new MetricsCardViewModel { Title = "Active Tenants", Value = activeTenants.ToString(), Icon = "fa-users", LinkTo = "/Admin/TenantDashboard" },
                new MetricsCardViewModel { Title = "Open Requests", Value = openRequests.ToString(), Icon = "fa-tasks", LinkTo = "/Admin/ServiceRequests" },
                new MetricsCardViewModel { Title = "Total Revenue", Value = "$" + totalRevenue.ToString("N0"), Icon = "fa-dollar-sign", LinkTo = "/Admin/Billing" },
                new MetricsCardViewModel { Title = "% Paid This Week", Value = percentPaidThisWeek + "%", Icon = "fa-percentage", LinkTo = "/Admin/Billing" },
                new MetricsCardViewModel { Title = "$ Invoiced Today", Value = "$" + invoicedToday.ToString("N0"), Icon = "fa-calendar-day", LinkTo = "/Admin/Billing" },
                new MetricsCardViewModel { Title = "# Zelle vs QuickBooks used", Value = $"Zelle: {zelleCount} / QB: {qbCount}", Icon = "fa-exchange-alt", LinkTo = "/Admin/Billing" },
                new MetricsCardViewModel { Title = "Tech Violations", Value = techViolations.ToString(), Icon = "fa-exclamation-triangle", RiskLevel = techViolations > 0 ? "High" : "Low", LinkTo = "/Admin/TechAudit" },
                new MetricsCardViewModel { Title = "System Alerts", Value = systemAlerts.ToString(), Icon = "fa-bell", RiskLevel = systemAlerts > 0 ? "Medium" : "Low", LinkTo = "/Admin/SystemStatus" }
            };
        }
    }
}
