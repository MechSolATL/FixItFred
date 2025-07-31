namespace Data.DTO.Analytics
{
    public class KpiSummaryDto
    {
        public int OverdueJobs { get; set; }
        public int IdleTechnicians { get; set; }
        public int MissedTransfers { get; set; }
        public int TotalTechnicians { get; set; }
        public int TotalTransfers { get; set; }
    }
}
