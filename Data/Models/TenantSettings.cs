using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TenantSettings
    {
        [Key] public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public bool EnableBilling { get; set; } = false;
        public bool EnableGratitudeWall { get; set; } = false;
        public bool EnableCustomDomains { get; set; } = false;
    }
}