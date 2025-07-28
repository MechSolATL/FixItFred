using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class Tenant
    {
        [Key] public Guid Id { get; set; }
        [Required] public string CompanyName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Tier { get; set; } = "Starter";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}