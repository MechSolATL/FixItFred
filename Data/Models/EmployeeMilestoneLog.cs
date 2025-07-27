namespace MVP_Core.Data.Models
{
    public class EmployeeMilestoneLog
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string MilestoneType { get; set; } = string.Empty; // "Birthday" or "Anniversary"
        public DateTime DateRecognized { get; set; }
        public bool Broadcasted { get; set; }
        public string CustomMessage { get; set; } = string.Empty;
    }
}
