// Created by FixItFred for Sprint 70.5 — Insight tracking for dispute reviews
namespace Data.Models;
public class DisputeInsightLog
{
    public int Id { get; set; }
    public int DisputeId { get; set; }
    public string InsightType { get; set; } = string.Empty;  // e.g., "Customer Input Delay", "Agent Retry"
    public string Description { get; set; } = string.Empty;
    public string LoggedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class TechnicianReputationEdge
{
    public int Id { get; set; }
    public int SourceTechnicianId { get; set; } // Technician initiating influence
    public int TargetTechnicianId { get; set; } // Technician receiving influence
    public double TrustWeight { get; set; } // Positive/negative trust value
    public string? InfluenceType { get; set; } // e.g., "Peer Review", "Conflict", "Collaboration"
    public DateTime CreatedAt { get; set; }
}

public class DisputeFusionLog
{
    public int Id { get; set; }
    public int DisputeId { get; set; }
    public int? TriggerTechnicianId { get; set; } // Technician linked to trigger
    public string? TriggerType { get; set; } // e.g., "Conflict", "Favoritism", "SLA Breach"
    public string? SourceTag { get; set; } // e.g., "Dispatcher Bias", "Peer Friction"
    public string? TracebackJson { get; set; } // Serialized traceback info
    public bool AutoResolved { get; set; }
    public DateTime LoggedAt { get; set; }
}
