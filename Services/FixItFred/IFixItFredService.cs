using MVP_Core.Models.FixItFred;

namespace MVP_Core.Services.FixItFred;

public interface IFixItFredService
{
    Task<OmegaSweepStatus> StartOmegaSweepAsync(string triggerSource, string sweepTag, string? relatedPr = null);
    Task<OmegaSweepStatus> GetSweepStatusAsync(string sweepId);
    Task<bool> ApproveLockTagAsync(string sweepId, string tagName);
    Task<string> GenerateAuditReportAsync(string sweepId);
    Task<bool> TriggerRollbackAsync(string sweepId, string reason);
    event EventHandler<OmegaSweepProgressEventArgs>? ProgressUpdated;
}