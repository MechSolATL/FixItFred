namespace Interfaces
{
    public interface IReplayEngineService
    {
        Task<bool> ReplaySnapshotAsync(string snapshotHash, string triggeredBy);
    }
}
