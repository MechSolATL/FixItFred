namespace Interfaces
{
    /// <summary>
    /// Service interface for system replay and recovery operations
    /// Handles snapshot replays and recovery scenario queuing
    /// </summary>
    public interface IReplayEngineService
    {
        /// <summary>
        /// Replays a system snapshot by hash identifier
        /// </summary>
        /// <param name="snapshotHash">Hash identifier of the snapshot to replay</param>
        /// <param name="triggeredBy">User or system identifier that triggered the replay</param>
        /// <returns>True if replay was successful</returns>
        Task<bool> ReplaySnapshotAsync(string snapshotHash, string triggeredBy);
        
        /// <summary>
        /// Replays a system snapshot with additional parameters
        /// </summary>
        /// <param name="snapshotHash">Hash identifier of the snapshot to replay</param>
        /// <param name="triggeredBy">User or system identifier that triggered the replay</param>
        /// <param name="overrideTimestamp">Optional timestamp override for the replay</param>
        /// <param name="notes">Additional notes for the replay operation</param>
        /// <returns>True if replay was successful</returns>
        Task<bool> ReplaySnapshotAsync(string snapshotHash, string triggeredBy, DateTime? overrideTimestamp = null, string notes = "");
        
        /// <summary>
        /// Queues a recovery scenario for execution
        /// </summary>
        /// <param name="scenarioName">Name of the recovery scenario</param>
        /// <param name="userId">User ID who queued the scenario</param>
        /// <param name="scheduledForUtc">UTC time when scenario should be executed</param>
        /// <param name="snapshotHash">Associated snapshot hash</param>
        /// <param name="notes">Additional notes for the scenario</param>
        /// <returns>Task representing the async operation</returns>
        Task QueueRecoveryScenarioAsync(string scenarioName, string userId, DateTime scheduledForUtc, string snapshotHash, string notes);
    }
}
