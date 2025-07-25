using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class TechnicianPerformanceModel : PageModel
    {
        public List<TechnicianPerformanceDto> TechnicianPerformances { get; set; } = new();
        public List<string> ServiceZones { get; set; } = new() { "North", "South", "East", "West" };
        [BindProperty(SupportsGet = true)] public string? FilterTechnician { get; set; }
        [BindProperty(SupportsGet = true)] public string? FilterZone { get; set; }
        [BindProperty(SupportsGet = true)] public string? SortBy { get; set; }

        public void OnGet()
        {
            // FixItFred: Sprint 35.2.B - Demo/mock data for dashboard
            var allTechs = new List<TechnicianPerformanceDto>
            {
                new TechnicianPerformanceDto { Name = "Alice Smith", Score = 92, OnTimePercent = 0.97, SLAPercent = 0.95, FeedbackAvg = 4.8, LastUpdate = DateTime.UtcNow.AddMinutes(-5) },
                new TechnicianPerformanceDto { Name = "Bob Jones", Score = 78, OnTimePercent = 0.85, SLAPercent = 0.88, FeedbackAvg = 4.2, LastUpdate = DateTime.UtcNow.AddMinutes(-12) },
                new TechnicianPerformanceDto { Name = "Carlos Lee", Score = 65, OnTimePercent = 0.72, SLAPercent = 0.80, FeedbackAvg = 3.9, LastUpdate = DateTime.UtcNow.AddMinutes(-22) },
                new TechnicianPerformanceDto { Name = "Dana Patel", Score = 55, OnTimePercent = 0.60, SLAPercent = 0.70, FeedbackAvg = 3.5, LastUpdate = DateTime.UtcNow.AddMinutes(-8) },
                new TechnicianPerformanceDto { Name = "Evan Kim", Score = 88, OnTimePercent = 0.93, SLAPercent = 0.91, FeedbackAvg = 4.6, LastUpdate = DateTime.UtcNow.AddMinutes(-18) }
            };
            var filtered = allTechs.AsQueryable();
            if (!string.IsNullOrEmpty(FilterTechnician))
                filtered = filtered.Where(t => t.Name.Contains(FilterTechnician, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(FilterZone))
                filtered = filtered.Where(t => t.Name.Contains(FilterZone, StringComparison.OrdinalIgnoreCase)); // Replace with actual zone logic
            if (!string.IsNullOrEmpty(SortBy))
            {
                filtered = SortBy switch
                {
                    "Score" => filtered.OrderByDescending(t => t.Score),
                    "OnTime" => filtered.OrderByDescending(t => t.OnTimePercent),
                    "SLA" => filtered.OrderByDescending(t => t.SLAPercent),
                    "Feedback" => filtered.OrderByDescending(t => t.FeedbackAvg),
                    "LastUpdate" => filtered.OrderByDescending(t => t.LastUpdate),
                    _ => filtered
                };
            }
            TechnicianPerformances = filtered.ToList();
            // CSV export
            if (Request.Query["export"] == "csv")
            {
                var csv = new StringBuilder();
                csv.AppendLine("Technician,Score,OnTime%,SLA%,FeedbackAvg,LastUpdate");
                foreach (var t in TechnicianPerformances)
                {
                    csv.AppendLine($"{t.Name},{t.Score},{t.OnTimePercent:P0},{t.SLAPercent:P0},{t.FeedbackAvg:0.0},{t.LastUpdate.ToLocalTime():g}");
                }
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                Response.Headers["Content-Disposition"] = "attachment; filename=technician-performance.csv";
                Response.ContentType = "text/csv";
                Response.Body.WriteAsync(bytes, 0, bytes.Length).Wait();
                Response.Body.Close();
            }
        }
    }
    // FixItFred: Sprint 35.2.B - DTO for dashboard
    public class TechnicianPerformanceDto
    {
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
        public double OnTimePercent { get; set; }
        public double SLAPercent { get; set; }
        public double FeedbackAvg { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
