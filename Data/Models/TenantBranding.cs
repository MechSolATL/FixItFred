using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TenantBranding
    {
        [Key] public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
        public string FaviconUrl { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = "#000000";
        public string SecondaryColor { get; set; } = "#FFFFFF";
    }
}