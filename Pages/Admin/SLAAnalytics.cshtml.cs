// Sprint 49.0 Patch Log: SLA Analytics PageModel + Leaderboard + CSV Export (Fixed for build)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.DTOs.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;

namespace MVP_Core.Pages.Admin
{
    public class SLAAnalyticsModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        public SLAAnalyticsModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        // Expose SLA analytics for Chart.js
        public List<SlaTrendDto> SlaTrends { get; set; } = new();
        public List<SlaTrendDto> SlaDistribution { get; set; } = new();
        public List<LeaderboardEntry> Leaderboard { get; set; } = new();
        public async Task OnGetAsync()
        {
            SlaTrends = await _dispatcherService.GetSlaTrendsAsync(groupBy: "ServiceType");
            SlaDistribution = await _dispatcherService.GetSlaTrendsAsync(groupBy: "Technician");
            Leaderboard = await GetLeaderboardAsync();
        }
        // Leaderboard DTO
        public class LeaderboardEntry
        {
            public int Rank { get; set; }
            public string Name { get; set; } = "";
            public int JobsCompleted { get; set; }
            public double SLACompliance { get; set; }
            public int UpsellCount { get; set; }
        }
        // Get leaderboard (stubbed for demo)
        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
        {
            // Replace with real DB logic
            var techs = await _dispatcherService.GetTechnicianLoadSummaryAsync();
            var leaderboard = new List<LeaderboardEntry>();
            int rank = 1;
            foreach (var t in techs.OrderByDescending(x => x.ActiveJobs).Take(5))
            {
                leaderboard.Add(new LeaderboardEntry
                {
                    Rank = rank++,
                    Name = t.Name,
                    JobsCompleted = t.ActiveJobs,
                    SLACompliance = t.DispatchScore / 100.0,
                    UpsellCount = t.TechnicianId % 3 // Stub: random upsell
                });
            }
            return leaderboard;
        }
        // CSV Export (mock data for required fields)
        public async Task<IActionResult> OnPostExportCsvAsync()
        {
            var logs = await _dispatcherService.GetSlaTrendsAsync(groupBy: "Technician");
            var sb = new StringBuilder();
            sb.AppendLine("ServiceRequestId,Technician,ETA,ActualArrival,SLAFlag,Timestamp");
            int id = 1000;
            foreach (var log in logs)
            {
                // Mock/fake values for missing fields
                string technician = log.GroupKey;
                string eta = (30 + log.Count).ToString();
                string actualArrival = DateTime.UtcNow.AddMinutes(-log.Count).ToString("O");
                string slaFlag = log.AverageSlaMinutes > 60 ? "Late" : "OnTime";
                string timestamp = DateTime.UtcNow.ToString("O");
                sb.AppendLine($"{id++},{technician},{eta},{actualArrival},{slaFlag},{timestamp}");
            }
            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "SLA_Logs.csv");
        }
    }
}
// End Sprint 49.0 Patch Log
