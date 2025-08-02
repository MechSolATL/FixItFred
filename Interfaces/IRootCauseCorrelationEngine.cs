using Data.Models;

namespace Interfaces
{
    public interface IRootCauseCorrelationEngine
    {
        void AnalyzeRootCause(string data);
        Task<bool> QueueRecoveryScenarioAsync(string sourceId, string scenarioKey, Models.UserContext context);
        Task<string> AnalyzeRecoveryPatternsAsync(string sourceId, DateTime since, Models.UserContext context);
    }
}
