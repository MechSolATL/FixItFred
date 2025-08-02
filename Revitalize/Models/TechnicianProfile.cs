using System.ComponentModel.DataAnnotations;

namespace Revitalize.Models;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Represents a technician profile in the Revitalize platform
/// Provides comprehensive technician data for multi-tenant SaaS operations
/// Connects to service request assignments and trust scoring mechanisms
/// UX: Displayed in technician dashboard and admin management interfaces
/// Cognitive Impact: Feeds into Nova AI technician performance analysis
/// </summary>
public class RevitalizeTechnicianProfile
{
    /// <summary>
    /// Primary key identifier for the technician
    /// Model Connection: Maps to AssignedTechnicianId in RevitalizeServiceRequest
    /// UX Trigger: Auto-generated on technician profile creation
    /// </summary>
    [Key]
    public int TechnicianId { get; set; }
    
    /// <summary>
    /// Foreign key to the tenant (company) this technician belongs to
    /// Model Connection: Links to RevitalizeTenant.TenantId for multi-tenant isolation
    /// UX Trigger: Set during technician onboarding by admin interface
    /// </summary>
    [Required]
    public int TenantId { get; set; }
    
    /// <summary>
    /// Full display name of the technician
    /// Model Connection: Used in service request assignments and UI displays
    /// UX Trigger: Entered during technician registration form
    /// Expected Outcome: Displayed across all customer-facing interfaces
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary email address for technician communications
    /// Model Connection: Used for notifications and empathy intake processes
    /// UX Trigger: Required field in technician profile form
    /// Empathy Impact: Receives LyraEmpathyIntakeNarrator feedback emails
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Contact phone number for technician communication
    /// Model Connection: Displayed in service request details for customer contact
    /// UX Trigger: Optional field in technician profile form
    /// Expected Outcome: Available for emergency technician contact
    /// </summary>
    [StringLength(15)]
    public string Phone { get; set; } = string.Empty;
    
    /// <summary>
    /// Company-specific employee identifier
    /// Model Connection: Links to external HR systems and payroll integration
    /// UX Trigger: Admin-assigned during technician onboarding
    /// CLI Flag: Used in RevitalizeCLI for technician lookup operations
    /// </summary>
    [StringLength(20)]
    public string EmployeeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Current status of the technician in the system
    /// Model Connection: Affects service request assignment eligibility
    /// UX Trigger: Admin can change via technician management interface
    /// Expected Outcome: Controls technician visibility in assignment workflows
    /// </summary>
    public RevitalizeTechnicianStatus Status { get; set; } = RevitalizeTechnicianStatus.Active;
    
    /// <summary>
    /// AI-calculated trust score based on performance and customer feedback
    /// Model Connection: Feeds into Nova AI recommendation engine
    /// UX Trigger: Automatically calculated after each service completion
    /// Cognitive Impact: Influences technician assignment priority and customer satisfaction predictions
    /// </summary>
    public decimal TrustScore { get; set; } = 100.0m;
    
    /// <summary>
    /// Total number of successfully completed service requests
    /// Model Connection: Incremented when RevitalizeServiceRequest.Status = Completed
    /// UX Trigger: Auto-updated on service request completion
    /// Expected Outcome: Displayed in technician performance dashboards
    /// </summary>
    public int CompletedJobs { get; set; } = 0;
    
    /// <summary>
    /// Average customer rating across all completed service requests
    /// Model Connection: Calculated from customer feedback data
    /// UX Trigger: Updated when customer submits service rating
    /// Empathy Impact: Influences LyraEmpathyIntakeNarrator response personalization
    /// </summary>
    public decimal AverageRating { get; set; } = 0.0m;
    
    /// <summary>
    /// Comma-separated list of technician service specializations
    /// Model Connection: Used for smart service request assignment matching
    /// UX Trigger: Editable in technician profile management interface
    /// CLI Flag: Filterable via RevitalizeCLI technician search operations
    /// </summary>
    [StringLength(500)]
    public string Specializations { get; set; } = string.Empty;
    
    /// <summary>
    /// Current availability status for new service request assignments
    /// Model Connection: Controls visibility in service request assignment workflows
    /// UX Trigger: Technician can toggle via mobile app or web interface
    /// Expected Outcome: Prevents assignment of new requests when unavailable
    /// </summary>
    public bool IsAvailable { get; set; } = true;
    
    /// <summary>
    /// UTC timestamp when this technician profile was created
    /// Model Connection: Used for reporting and analytics on technician onboarding
    /// UX Trigger: Auto-set during technician profile creation
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// UTC timestamp of technician's last system activity
    /// Model Connection: Updated on login, service updates, and mobile app usage
    /// UX Trigger: Auto-updated on any technician system interaction
    /// Expected Outcome: Used for activity monitoring and availability assessment
    /// </summary>
    public DateTime? LastActiveAt { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Navigation property to the associated tenant (company)
    /// Model Connection: Links to RevitalizeTenant for multi-tenant data isolation
    /// </summary>
    public virtual RevitalizeTenant Tenant { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to all service requests assigned to this technician
    /// Model Connection: Collection of RevitalizeServiceRequest entities
    /// UX Trigger: Displayed in technician workload and history views
    /// </summary>
    public virtual ICollection<RevitalizeServiceRequest> AssignedRequests { get; set; } = new List<RevitalizeServiceRequest>();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Enumeration of possible technician statuses
/// Controls technician availability and system access levels
/// UX Impact: Affects UI display and available actions for technician profiles
/// </summary>
public enum RevitalizeTechnicianStatus
{
    /// <summary>
    /// Technician is active and available for service request assignments
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Technician is inactive but profile is retained for historical data
    /// </summary>
    Inactive = 2,
    
    /// <summary>
    /// Technician is suspended and cannot receive new assignments
    /// </summary>
    Suspended = 3,
    
    /// <summary>
    /// Technician is in training mode with limited assignment capabilities
    /// </summary>
    Training = 4
}