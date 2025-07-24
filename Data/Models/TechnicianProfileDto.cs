using System;
using System.Collections.Generic;

namespace MVP_Core.Data.Models
{
    public class TechnicianProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
        public bool IsActive { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Badges { get; set; } // JSON array

        // Stats
        public int CompletedJobs { get; set; }
        public int Callbacks { get; set; }
        public double CloseRate { get; set; } // 0-1
        public double AvgReviewScore { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Skills { get; set; } = new();
    }

    public class TechnicianAnalyticsDto
    {
        public List<MonthlyKpiDto> CloseRateTrends { get; set; } = new();
        public List<CallbackTrendDto> CallbackTrends { get; set; } = new();
        public double EtaSuccessRate { get; set; }
        public double AverageDelayMinutes { get; set; }

        // Predictive metrics
        public double CloseRateForecast7d { get; set; }
        public double CloseRateForecast30d { get; set; }
        public double CallbackRateForecast7d { get; set; }
        public double CallbackRateForecast30d { get; set; }

        // Risk flagging
        public bool IsAtRisk { get; set; }
        public List<string> RiskFlags { get; set; } = new();
    }

    public class MonthlyKpiDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double CloseRate { get; set; }
    }

    public class CallbackTrendDto
    {
        public DateTime Date { get; set; }
        public int CallbackCount { get; set; }
    }

    public class DateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
