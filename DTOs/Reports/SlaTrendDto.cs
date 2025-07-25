// FixItFred — Sprint 46.1 Build Stabilization
namespace MVP_Core.DTOs.Reports
{
    public class SlaTrendDto
    {
        public string GroupKey { get; set; } = string.Empty;
        public string GroupType { get; set; } = string.Empty;
        public double AverageSlaMinutes { get; set; }
        public int Count { get; set; }
    }
}
