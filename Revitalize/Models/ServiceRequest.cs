using System.ComponentModel.DataAnnotations;

namespace Revitalize.Models;

/// <summary>
/// Represents a service request in the Revitalize platform
/// </summary>
public class RevitalizeServiceRequest
{
    [Key]
    public int ServiceRequestId { get; set; }
    
    [Required]
    public int TenantId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public RevitalizeServiceType ServiceType { get; set; }
    
    [Required]
    public RevitalizePriority Priority { get; set; } = RevitalizePriority.Medium;
    
    [Required]
    public RevitalizeServiceRequestStatus Status { get; set; } = RevitalizeServiceRequestStatus.Pending;
    
    public int? AssignedTechnicianId { get; set; }
    
    [StringLength(500)]
    public string CustomerName { get; set; } = string.Empty;
    
    [StringLength(15)]
    public string CustomerPhone { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string ServiceAddress { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ScheduledDate { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public virtual RevitalizeTenant Tenant { get; set; } = null!;
    public virtual RevitalizeTechnicianProfile? AssignedTechnician { get; set; }
}

public enum RevitalizeServiceType
{
    Plumbing = 1,
    HVAC = 2,
    WaterFiltration = 3,
    Emergency = 4,
    Maintenance = 5
}

public enum RevitalizePriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Emergency = 4
}

public enum RevitalizeServiceRequestStatus
{
    Pending = 1,
    Assigned = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}