namespace MVP_Core.Data.Models.UI
{
    public class LeaderboardViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Rank { get; set; } = 0;
        public int JobsCompleted { get; set; } = 0;
        public decimal SalesTotal { get; set; } = 0;
        public double CustomerRating { get; set; } = 0.0;
    }
}
