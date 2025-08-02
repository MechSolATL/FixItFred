using System.ComponentModel.DataAnnotations;

namespace Revitalize.Models;

/// <summary>
/// Represents a tenant (company) in the Revitalize SaaS platform
/// </summary>
public class RevitalizeTenant
{
    [Key]
    public int TenantId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    public string TenantCode { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<RevitalizeServiceRequest> ServiceRequests { get; set; } = new List<RevitalizeServiceRequest>();
    public virtual ICollection<RevitalizeTechnicianProfile> Technicians { get; set; } = new List<RevitalizeTechnicianProfile>();
}