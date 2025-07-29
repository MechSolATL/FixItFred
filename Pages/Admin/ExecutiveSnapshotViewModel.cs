using System;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class ExecutiveSnapshotViewModel
    {
        public string SnapshotDate { get; set; }
        public int TotalRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int InvoicedRequests { get; set; }
        public double AverageCompletionTimeHours { get; set; }
        public double MissedRescheduleRatio { get; set; }
        public List<TopTechPerformance> TopTechnicians { get; set; }
        public List<string> CriticalAlerts { get; set; }
        public HistoricalComparison Trend { get; set; }
    }
    public class TopTechPerformance
    {
        public string TechnicianName { get; set; }
        public int JobsCompleted { get; set; }
        public decimal RevenueGenerated { get; set; }
    }
    public class HistoricalComparison
    {
        public int YesterdayCompleted { get; set; }
        public double PercentChange { get; set; }
    }
}
