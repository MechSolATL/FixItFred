using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class SlaDriftSnapshot
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int TotalRequests { get; set; }
        public int DriftedRequests { get; set; }
        public double AverageDriftMinutes { get; set; }
        public string? HeatmapJson { get; set; }
    }

    public class RootCauseCorrelationLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Summary { get; set; }
        public string? Details { get; set; }
        public string? Module { get; set; }
    }

    public class StorageGrowthSnapshot
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public long TotalSizeBytes { get; set; }
        public long GrowthBytes { get; set; }
        public double CompressionRatio { get; set; }
    }

    public class AdminAlertLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AlertType { get; set; }
        public string? Message { get; set; }
        public string? Severity { get; set; }
        public bool IsResolved { get; set; }
    }
}
