namespace Data.Models
{
    public class TechProfile
    {
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int JobsCompleted { get; set; }
        public string SalesTotal { get; set; } = "$0.00";
        public double CustomerRating { get; set; }
    }
}
