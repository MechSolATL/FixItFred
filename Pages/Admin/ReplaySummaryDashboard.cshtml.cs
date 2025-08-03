using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Pages.Admin
{
    /// <summary>
    /// [OmegaSweep_Auto] Admin dashboard for replay summary and persona analytics
    /// Provides empathy score analytics and export capabilities for compliance
    /// </summary>
    public class ReplaySummaryDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReplaySummaryDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string DateRange { get; set; } = "30"; // Default to 30 days

        [BindProperty(SupportsGet = true)]
        public string PersonaFilter { get; set; } = "All";

        public List<PersonaAnalytic> PersonaAnalytics { get; set; } = new();
        public List<EmpathyTrendData> TrendData { get; set; } = new();
        public string ExportMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await LoadPersonaAnalytics();
            await LoadTrendData();
        }

        /// <summary>
        /// [OmegaSweep_Auto] Export persona analytics for compliance review
        /// </summary>
        public async Task<IActionResult> OnPostExportAsync()
        {
            await LoadPersonaAnalytics();
            
            var csvContent = GenerateCSVExport();
            var fileName = $"PersonaAnalytics_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            
            ExportMessage = $"[OmegaSweep_Auto] Analytics exported successfully as {fileName}";
            
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), 
                       "text/csv", 
                       fileName);
        }

        private async Task LoadPersonaAnalytics()
        {
            var daysBack = int.TryParse(DateRange, out var days) ? days : 30;
            var cutoffDate = DateTime.UtcNow.AddDays(-daysBack);

            // [OmegaSweep_Auto] Generate sample persona analytics data
            // In real implementation, this would query actual empathy test results
            PersonaAnalytics = new List<PersonaAnalytic>
            {
                new PersonaAnalytic
                {
                    PersonaType = "Customer Service",
                    CurrentScore = 87.1,
                    PreviousScore = 85.2,
                    TestCount = 45,
                    PassRate = 95.6,
                    LastUpdated = DateTime.UtcNow.AddHours(-2)
                },
                new PersonaAnalytic
                {
                    PersonaType = "Technical Support", 
                    CurrentScore = 80.3,
                    PreviousScore = 78.5,
                    TestCount = 38,
                    PassRate = 92.1,
                    LastUpdated = DateTime.UtcNow.AddHours(-1)
                },
                new PersonaAnalytic
                {
                    PersonaType = "Emergency Response",
                    CurrentScore = 93.2,
                    PreviousScore = 92.1,
                    TestCount = 28,
                    PassRate = 100.0,
                    LastUpdated = DateTime.UtcNow.AddMinutes(-30)
                },
                new PersonaAnalytic
                {
                    PersonaType = "Sales Support",
                    CurrentScore = 76.8,
                    PreviousScore = 74.5,
                    TestCount = 22,
                    PassRate = 86.4,
                    LastUpdated = DateTime.UtcNow.AddHours(-3)
                }
            };

            // Apply persona filter if specified
            if (PersonaFilter != "All")
            {
                PersonaAnalytics = PersonaAnalytics.Where(p => p.PersonaType == PersonaFilter).ToList();
            }

            await Task.CompletedTask; // Placeholder for async database operations
        }

        private async Task LoadTrendData()
        {
            // [OmegaSweep_Auto] Generate sample trend data for the last 7 days
            TrendData = new List<EmpathyTrendData>();
            
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                TrendData.Add(new EmpathyTrendData
                {
                    Date = date,
                    CustomerService = 85.0 + (Random.Shared.NextDouble() * 4.0), // 85-89 range
                    TechnicalSupport = 78.0 + (Random.Shared.NextDouble() * 4.0), // 78-82 range
                    EmergencyResponse = 91.0 + (Random.Shared.NextDouble() * 3.0), // 91-94 range
                    SalesSupport = 74.0 + (Random.Shared.NextDouble() * 4.0) // 74-78 range
                });
            }

            await Task.CompletedTask; // Placeholder for async database operations
        }

        private string GenerateCSVExport()
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("PersonaType,CurrentScore,PreviousScore,ScoreDiff,TestCount,PassRate,LastUpdated");
            
            foreach (var analytic in PersonaAnalytics)
            {
                var scoreDiff = analytic.CurrentScore - analytic.PreviousScore;
                csv.AppendLine($"\"{analytic.PersonaType}\",{analytic.CurrentScore:F1},{analytic.PreviousScore:F1},{scoreDiff:+0.0;-0.0;0.0},{analytic.TestCount},{analytic.PassRate:F1},\"{analytic.LastUpdated:yyyy-MM-dd HH:mm:ss}\"");
            }
            
            return csv.ToString();
        }
    }

    /// <summary>
    /// [OmegaSweep_Auto] Data model for persona empathy analytics
    /// </summary>
    public class PersonaAnalytic
    {
        public string PersonaType { get; set; } = string.Empty;
        public double CurrentScore { get; set; }
        public double PreviousScore { get; set; }
        public int TestCount { get; set; }
        public double PassRate { get; set; }
        public DateTime LastUpdated { get; set; }
        
        public double ScoreDifference => CurrentScore - PreviousScore;
        public string TrendIndicator => ScoreDifference > 0 ? "↗️" : ScoreDifference < 0 ? "↘️" : "➡️";
    }

    /// <summary>
    /// [OmegaSweep_Auto] Data model for empathy score trend tracking
    /// </summary>
    public class EmpathyTrendData
    {
        public DateTime Date { get; set; }
        public double CustomerService { get; set; }
        public double TechnicalSupport { get; set; }
        public double EmergencyResponse { get; set; }
        public double SalesSupport { get; set; }
    }
}