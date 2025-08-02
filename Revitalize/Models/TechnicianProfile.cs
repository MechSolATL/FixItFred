using System.ComponentModel.DataAnnotations;

namespace Revitalize.Models;

/// <summary>
/// Represents a technician profile in the Revitalize platform
/// </summary>
public class RevitalizeTechnicianProfile
{
    [Key]
    public int TechnicianId { get; set; }
    
    [Required]
    public int TenantId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(15)]
    public string Phone { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string EmployeeId { get; set; } = string.Empty;
    
    public RevitalizeTechnicianStatus Status { get; set; } = RevitalizeTechnicianStatus.Active;
    
    public decimal TrustScore { get; set; } = 100.0m;
    
    public int CompletedJobs { get; set; } = 0;
    
    public decimal AverageRating { get; set; } = 0.0m;
    
    [StringLength(500)]
    public string Specializations { get; set; } = string.Empty;
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastActiveAt { get; set; }
    
    // Navigation properties
    public virtual RevitalizeTenant Tenant { get; set; } = null!;
    public virtual ICollection<RevitalizeServiceRequest> AssignedRequests { get; set; } = new List<RevitalizeServiceRequest>();
}

public enum RevitalizeTechnicianStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Training = 4
}