namespace MVP_Core.Models.FixItFred;

public enum SweepState
{
    Queued,
    Running,
    Completed,
    Failed,
    RolledBack
}

public enum RunPhase
{
    Clean,
    Build,
    EmpathyTests,
    IntegrationTests,
    RevitalizeValidation
}

public class OmegaSweepRun
{
    public int RunNumber { get; set; }
    public SweepState State { get; set; }
    public RunPhase? CurrentPhase { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? EstimatedTimeRemaining { get; set; }
    public List<string> PhaseResults { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class OmegaSweepStatus
{
    public string SweepId { get; set; } = string.Empty;
    public string TriggerSource { get; set; } = string.Empty;
    public string SweepTag { get; set; } = string.Empty;
    public string? RelatedPr { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public SweepState OverallState { get; set; }
    public List<OmegaSweepRun> Runs { get; set; } = new();
    public bool IsReleaseGateActive { get; set; }
    public bool IsTagLocked { get; set; }
    public string? TagLockReason { get; set; }
    public List<string> AuditLogPaths { get; set; } = new();
}

public class OmegaSweepProgressEventArgs : EventArgs
{
    public string SweepId { get; set; } = string.Empty;
    public OmegaSweepStatus Status { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}