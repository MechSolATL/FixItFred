namespace MVP_Core.Services.Leaderboard.Models
{
    public class LeaderboardEntry
    {
        public string Name { get; set; } = string.Empty;
        public int CompletedJobs { get; set; }
        public double Rating { get; set; }
    }
}