namespace Data.ViewModels
{
    public class ResponseScorecard
    {
        public string TechnicianName { get; set; } = string.Empty;
        public int AverageResponseSeconds { get; set; }
        public int TotalResponses { get; set; }
        public int LateCount { get; set; }
        public int Rank { get; set; }
    }
}
