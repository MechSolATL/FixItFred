namespace MVP_Core.Data.Models
{
    public class PageVisit
    {
        public int Id { get; set; }
        public int SessionInfoId { get; set; }
        public string? PageName { get; set; }
        public DateTime VisitTimestamp { get; set; }
        public string? UserClick { get; set; }
    }
}
