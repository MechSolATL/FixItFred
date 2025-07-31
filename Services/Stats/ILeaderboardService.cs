using Data.Models;

namespace Services.Stats
{
    public interface ILeaderboardService
    {
        Task<List<TechnicianViewModel>> GetTopTechniciansAsync(int count = 6);
        Task<string> GetEfficiencyRateAsync();
        Task<string> GetMonthlyChallengeAsync();
    }
}
