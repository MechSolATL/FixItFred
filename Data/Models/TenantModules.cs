using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class TenantModules
    {
        [Key] public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public bool TechnicianTracking { get; set; } = true;
        public bool AccountingView { get; set; } = false;
        public bool CommunityFeed { get; set; } = false;
        public bool HeatPumpMatchups { get; set; } = false;
    }
}