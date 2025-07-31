using Data.Models;

namespace Interfaces
{
    public interface IRootCauseCorrelationEngine
    {
        void AnalyzeRootCause(string data);
        Task<bool> QueueRecoveryScenarioAsync(string sourceId, string scenarioKey, UserContext context);
        Task<string> AnalyzeRecoveryPatternsAsync(string sourceId, DateTime since, UserContext context);
    }
}
