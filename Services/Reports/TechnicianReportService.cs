using Data;
using Data.Models;
using Data.Models.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Sprint 91.23 - Technician Report Library
namespace Services.Reports
{
    public class TechnicianReportService
    {
        private readonly ApplicationDbContext _context;

        public TechnicianReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(TechnicianReport report)
        {
            _context.TechnicianReports.Add(report);
            await _context.SaveChangesAsync();
            return report.Id;
        }

        public async Task<List<TechnicianReport>> GetByTechIdAsync(int techId)
        {
            return await _context.TechnicianReports
                .Where(r => r.TechnicianId == techId)
                .OrderByDescending(r => r.SubmittedOn)
                .ToListAsync();
        }

        public async Task<List<TechnicianReport>> FilterAsync(DateTime? startDate, DateTime? endDate, int? technicianId)
        {
            var query = _context.TechnicianReports.AsQueryable();

            if (technicianId.HasValue)
                query = query.Where(r => r.TechnicianId == technicianId);

            if (startDate.HasValue)
                query = query.Where(r => r.SubmittedOn >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.SubmittedOn <= endDate.Value);

            return await query.OrderByDescending(r => r.SubmittedOn).ToListAsync();
        }

        public async Task AddFeedbackAsync(TechnicianReportFeedback feedback)
        {
            _context.TechnicianReportFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TechnicianReportFeedback>> GetFeedbackForReportAsync(int reportId)
        {
            return await _context.TechnicianReportFeedbacks
                .Where(f => f.ReportId == reportId)
                .ToListAsync();
        }

        // ✅ Added by FixItFred — Sprint92_RecoveryFixes
        public async Task<ReportResult> GenerateSingleReport(Guid technicianId, DateTime fromDate, DateTime toDate)
        {
            // TODO: Replace with actual logic
            return await Task.FromResult(new ReportResult
            {
                TechnicianId = technicianId,
                FromDate = fromDate,
                ToDate = toDate,
                Summary = "Report generation pending implementation."
            });
        }

        // ✅ Added by FixItFred — Sprint92_RecoveryFixes
        public async Task<ReportComparison> GenerateComparisonReport(Guid[] technicianIds, DateTime fromDate, DateTime toDate)
        {
            // TODO: Replace with real aggregation logic
            return await Task.FromResult(new ReportComparison
            {
                TechnicianIds = technicianIds.ToList(),
                FromDate = fromDate,
                ToDate = toDate,
                ComparisonData = new Dictionary<Guid, string>() // placeholder
            });
        }

        // ✅ Overloaded method for controller compatibility
        public async Task<byte[]> GenerateSingleReport(TechnicianProfileDto technician, byte[] chartImg, string? notes)
        {
            // TODO: Replace with actual PDF generation logic
            var reportText = $"Technician Report for {technician.Name}\nNotes: {notes ?? "None"}\nChart included: {chartImg.Length > 0}";
            return global::System.Text.Encoding.UTF8.GetBytes(reportText); // FixItFred: Use global:: to avoid namespace conflict with Services.System
        }

        // ✅ Overloaded method for controller compatibility
        public async Task<byte[]> GenerateComparisonReport(List<TechnicianProfileDto> technicians, List<byte[]> chartImgs, string? notes)
        {
            // TODO: Replace with actual PDF generation logic
            var reportText = $"Comparison Report for {technicians.Count} technicians\nNotes: {notes ?? "None"}\nCharts included: {chartImgs.Count}";
            return global::System.Text.Encoding.UTF8.GetBytes(reportText); // FixItFred: Use global:: to avoid namespace conflict with Services.System
        }
    }
}
