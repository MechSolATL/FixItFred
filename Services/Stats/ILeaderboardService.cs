namespace MVP_Core.Services.Stats
{
    public interface ILeaderboardService
    {
        Task<List<TechProfile>> GetTopTechniciansAsync(int count = 6);
        Task<string> GetEfficiencyRateAsync();
        Task<string> GetMonthlyChallengeAsync();
    }
}
