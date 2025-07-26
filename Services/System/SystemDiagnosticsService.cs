using MVP_Core.Data.Models;
using MVP_Core.Helpers;
using MVP_Core.DTOs.Reports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MVP_Core.Services.System
{
    public class SystemDiagnosticsService
    {
        private readonly ApplicationDbContext _db;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SystemDiagnosticsService> _logger;

        public SystemDiagnosticsService(ApplicationDbContext db, IServiceProvider serviceProvider, ILogger<SystemDiagnosticsService> logger)
        {
            _db = db;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<DiagnosticReportDTO> RunDiagnosticsAsync()
        {
            var warnings = new List<string>();
            int errorCount = 0;
            string status = "OK";

            // 1. Razor page model binding mismatches (stub: add real checks as needed)
            // TODO: Implement actual model binding mismatch detection
            // For now, add a placeholder warning
            warnings.Add("Model binding mismatch check not implemented.");

            // 2. Missing EF DbSet<> for existing tables
            var dbSets = typeof(ApplicationDbContext).GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).Select(p => p.Name).ToList();
            var tables = await _db.Database.GetDbConnection().GetSchemaAsync("Tables");
            // TODO: Compare tables to dbSets and add warnings
            warnings.Add("Missing DbSet<> check not fully implemented.");

            // 3. Unresponsive services in DI container
            // TODO: Implement actual DI health check
            warnings.Add("DI container health check not implemented.");

            // 4. Last 10 log entries with errors/exceptions
            var lastLogs = _db.SystemDiagnosticLogs.OrderByDescending(l => l.Timestamp).Take(10).ToList();
            errorCount = lastLogs.Count(l => l.StatusLevel == DiagnosticStatusLevel.Error);
            if (errorCount > 0) status = "Error";
            else if (warnings.Count > 0) status = "Warning";

            // 5. Output DiagnosticReportDTO
            return new DiagnosticReportDTO
            {
                Status = status,
                ErrorCount = errorCount,
                Warnings = warnings
            };
        }
    }
}
