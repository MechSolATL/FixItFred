using System.ComponentModel.DataAnnotations;

namespace Revitalize.Models;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Represents a tenant (company) in the Revitalize SaaS platform
/// Provides multi-tenant isolation and company-specific configuration
/// Model Connection: Central entity for tenant-based data segregation
/// UX Trigger: Created via admin tenant onboarding workflow
/// CLI Flag: Manageable via RevitalizeCLI tenant commands
/// Expected Outcome: Enables complete data isolation between customer companies
/// Cognitive Impact: Feeds into Nova AI tenant-specific optimization recommendations
/// </summary>
public class RevitalizeTenant
{
    /// <summary>
    /// Primary key identifier for the tenant
    /// Model Connection: Referenced by all tenant-scoped entities
    /// UX Trigger: Auto-generated during tenant creation
    /// CLI Flag: Used as identifier in RevitalizeCLI tenant operations
    /// </summary>
    [Key]
    public int TenantId { get; set; }
    
    /// <summary>
    /// Display name of the company/organization
    /// Model Connection: Used throughout UI for branding and identification
    /// UX Trigger: Required field in tenant onboarding form
    /// Expected Outcome: Displayed in headers, reports, and customer communications
    /// Empathy Impact: Personalized in LyraEmpathyIntakeNarrator communications
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CompanyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique alphanumeric code for tenant identification
    /// Model Connection: Used for URL routing and API namespace isolation
    /// UX Trigger: Admin-defined during tenant setup or auto-generated
    /// CLI Flag: Primary identifier for RevitalizeCLI tenant commands
    /// Expected Outcome: Creates tenant-specific URLs and API endpoints
    /// </summary>
    [Required]
    [StringLength(10)]
    public string TenantCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional description or notes about the tenant
    /// Model Connection: Used for internal tenant management and reporting
    /// UX Trigger: Optional field in tenant configuration interface
    /// Expected Outcome: Available for admin reference and tenant categorization
    /// </summary>
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Current active status of the tenant
    /// Model Connection: Controls access to all tenant-scoped features
    /// UX Trigger: Admin can activate/deactivate via tenant management interface
    /// Expected Outcome: Inactive tenants cannot access system features
    /// Side Effects: Cascades to technician availability and service request processing
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// UTC timestamp when this tenant was created
    /// Model Connection: Used for billing calculation and analytics reporting
    /// UX Trigger: Auto-set during tenant onboarding completion
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// UTC timestamp of last tenant configuration update
    /// Model Connection: Tracks tenant maintenance and configuration changes
    /// UX Trigger: Auto-updated when tenant settings are modified
    /// Expected Outcome: Used for audit trails and change tracking
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Navigation property to all service requests belonging to this tenant
    /// Model Connection: Collection of RevitalizeServiceRequest entities
    /// UX Trigger: Displayed in tenant service request dashboard
    /// CLI Flag: Accessible via RevitalizeCLI service list commands
    /// </summary>
    public virtual ICollection<RevitalizeServiceRequest> ServiceRequests { get; set; } = new List<RevitalizeServiceRequest>();
    
    /// <summary>
    /// Navigation property to all technicians employed by this tenant
    /// Model Connection: Collection of RevitalizeTechnicianProfile entities
    /// UX Trigger: Displayed in tenant technician management interface
    /// Expected Outcome: Enables tenant-specific technician assignment and management
    /// </summary>
    public virtual ICollection<RevitalizeTechnicianProfile> Technicians { get; set; } = new List<RevitalizeTechnicianProfile>();
}