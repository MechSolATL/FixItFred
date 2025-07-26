// Created by FixItFred for Sprint 70.5 — Insight tracking for dispute reviews
namespace MVP_Core.Data.Models;
public class DisputeInsightLog
{
    public int Id { get; set; }
    public int DisputeId { get; set; }
    public string InsightType { get; set; }  // e.g., "Customer Input Delay", "Agent Retry"
    public string Description { get; set; }
    public string LoggedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
