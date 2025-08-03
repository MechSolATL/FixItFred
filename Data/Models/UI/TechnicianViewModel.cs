namespace Data.Models.UI
{
    public class TechnicianViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty; // FixItFred: Add missing Avatar property for Leaderboard
        public int Rank { get; set; }
        public int JobsCompleted { get; set; }
        public string SalesTotal { get; set; } = "$0.00";
        public double CustomerRating { get; set; }
    }
}
