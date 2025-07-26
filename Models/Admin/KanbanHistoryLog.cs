// FixItFred — Sprint 44 Final Model Recovery
using System;
using System.ComponentModel.DataAnnotations;

public class KanbanHistoryLog {
    [Key]
    public int Id { get; set; }
    public int ServiceRequestId { get; set; }
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public int ToIndex { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public string PerformedByRole { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime Timestamp { get => ChangedAt; set => ChangedAt = value; }
}