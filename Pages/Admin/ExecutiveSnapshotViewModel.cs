using System;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class ExecutiveSnapshotViewModel
    {
        public RequestFlowMetrics RequestFlow { get; set; } = new();
        public TimeEfficiencyMetrics TimeEfficiency { get; set; } = new();
        public List<TechnicianDigestMetrics> Technicians { get; set; } = new();
        public List<ManagerContributionMetrics> Managers { get; set; } = new();
        public AlertFlagMetrics AlertFlags { get; set; } = new();
        public HistoricComparisonMetrics HistoricComparison { get; set; } = new();
        public List<CreditAttribution> CreditAttributions { get; set; } = new();
    }
    public class RequestFlowMetrics
    {
        public int TotalReceived { get; set; }
        public int Completed { get; set; }
        public int Rejected { get; set; }
        public int Cancelled { get; set; }
    }
    public class TimeEfficiencyMetrics
    {
        public double AvgCompletionTimeHours { get; set; }
        public int RescheduleCount { get; set; }
        public double SameDayCloseRate { get; set; }
    }
    public class TechnicianDigestMetrics
    {
        public string Name { get; set; } = string.Empty;
        public int JobsCompleted { get; set; }
        public int IssuesFlagged { get; set; }
        public int Callbacks { get; set; }
        public string Zone { get; set; } = string.Empty;
    }
    public class ManagerContributionMetrics
    {
        public string ManagerName { get; set; } = string.Empty;
        public int TasksOwned { get; set; }
        public int TasksReviewed { get; set; }
    }
    public class AlertFlagMetrics
    {
        public int MissedDeadlines { get; set; }
        public int LowPerformers { get; set; }
        public int IncompleteToolReturns { get; set; }
    }
    public class HistoricComparisonMetrics
    {
        public int DeltaDay { get; set; }
        public int DeltaWeek { get; set; }
        public int DeltaMonth { get; set; }
    }
    public class CreditAttribution
    {
        public string Origin { get; set; } = string.Empty;
        public string Executor { get; set; } = string.Empty;
        public string Reviewer { get; set; } = string.Empty;
    }
}
