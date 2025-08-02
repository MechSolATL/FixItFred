using System.ComponentModel.DataAnnotations;

namespace Revitalize.Models;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Represents a service request in the Revitalize platform
/// Core entity for tracking customer service needs and technician assignments
/// Model Connection: Central hub connecting tenants, technicians, and customer interactions
/// UX Trigger: Created via customer portal, admin interface, or mobile app
/// CLI Flag: Manageable via RevitalizeCLI service commands
/// Expected Outcome: Drives entire service delivery workflow from creation to completion
/// Cognitive Impact: Primary data source for Nova AI optimization and LyraEmpathyIntakeNarrator
/// Empathy Replay Impact: Each request feeds empathy learning models for better customer interactions
/// </summary>
public class RevitalizeServiceRequest
{
    /// <summary>
    /// Primary key identifier for the service request
    /// Model Connection: Referenced in technician assignments and audit logs
    /// UX Trigger: Auto-generated on service request creation
    /// CLI Flag: Used as identifier in RevitalizeCLI service operations
    /// </summary>
    [Key]
    public int ServiceRequestId { get; set; }
    
    /// <summary>
    /// Foreign key to the tenant (company) this request belongs to
    /// Model Connection: Links to RevitalizeTenant.TenantId for data isolation
    /// UX Trigger: Auto-set based on authenticated tenant context
    /// Expected Outcome: Ensures multi-tenant data segregation
    /// Side Effects: Controls technician assignment eligibility to same-tenant only
    /// </summary>
    [Required]
    public int TenantId { get; set; }
    
    /// <summary>
    /// Brief descriptive title of the service request
    /// Model Connection: Displayed in all service request lists and dashboards
    /// UX Trigger: Customer enters via service request creation form
    /// Expected Outcome: Primary identifier for service request in all UI displays
    /// Empathy Impact: Analyzed by LyraEmpathyIntakeNarrator for initial response tone
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the service issue or request
    /// Model Connection: Used by technicians for service preparation and Nova AI analysis
    /// UX Trigger: Customer provides via detailed description field
    /// Expected Outcome: Guides technician preparation and service approach
    /// Cognitive Impact: Primary input for Nova AI problem diagnosis and solution recommendations
    /// Empathy Replay Impact: Text analyzed for customer emotional state and frustration indicators
    /// </summary>
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Categorization of the type of service required
    /// Model Connection: Used for technician specialization matching
    /// UX Trigger: Customer selects from predefined service type dropdown
    /// Expected Outcome: Enables smart technician assignment based on specializations
    /// CLI Flag: Filterable in RevitalizeCLI service list operations
    /// </summary>
    [Required]
    public RevitalizeServiceType ServiceType { get; set; }
    
    /// <summary>
    /// Priority level of the service request
    /// Model Connection: Affects service request ordering and technician assignment priority
    /// UX Trigger: Auto-calculated or manually set by admin based on service type
    /// Expected Outcome: Controls service request queue positioning and SLA expectations
    /// Cognitive Impact: Influences Nova AI resource allocation recommendations
    /// </summary>
    [Required]
    public RevitalizePriority Priority { get; set; } = RevitalizePriority.Medium;
    
    /// <summary>
    /// Current status of the service request in the workflow
    /// Model Connection: Controls available actions and UI state transitions
    /// UX Trigger: Updated automatically as service progresses through workflow stages
    /// Expected Outcome: Drives customer notifications and technician work queue visibility
    /// Empathy Impact: Status changes trigger appropriate LyraEmpathyIntakeNarrator communications
    /// </summary>
    [Required]
    public RevitalizeServiceRequestStatus Status { get; set; } = RevitalizeServiceRequestStatus.Pending;
    
    /// <summary>
    /// Foreign key to the assigned technician profile
    /// Model Connection: Links to RevitalizeTechnicianProfile.TechnicianId
    /// UX Trigger: Set via admin assignment interface or automatic assignment algorithms
    /// Expected Outcome: Enables technician-specific workflows and customer communication
    /// CLI Flag: Used in RevitalizeCLI technician workload commands
    /// </summary>
    public int? AssignedTechnicianId { get; set; }
    
    /// <summary>
    /// Customer's full name for service delivery
    /// Model Connection: Used in technician communications and service documentation
    /// UX Trigger: Customer enters during service request creation
    /// Expected Outcome: Personalizes all customer-facing communications
    /// Empathy Impact: Used by LyraEmpathyIntakeNarrator for personalized responses
    /// </summary>
    [StringLength(500)]
    public string CustomerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's contact phone number
    /// Model Connection: Used for technician-customer communication and service notifications
    /// UX Trigger: Customer provides during service request or profile setup
    /// Expected Outcome: Enables direct technician contact and service updates
    /// </summary>
    [StringLength(15)]
    public string CustomerPhone { get; set; } = string.Empty;
    
    /// <summary>
    /// Customer's email address for digital communications
    /// Model Connection: Used for service notifications and follow-up communications
    /// UX Trigger: Customer provides during account setup or service request
    /// Expected Outcome: Receives automated service updates and completion notifications
    /// Empathy Impact: Recipient of LyraEmpathyIntakeNarrator personalized email responses
    /// </summary>
    [StringLength(100)]
    public string CustomerEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// Physical address where service will be performed
    /// Model Connection: Used for technician routing and travel time calculations
    /// UX Trigger: Customer enters service location during request creation
    /// Expected Outcome: Enables GPS navigation and travel time estimation for technicians
    /// Cognitive Impact: Used by Nova AI for optimal technician assignment based on location
    /// </summary>
    [StringLength(500)]
    public string ServiceAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// UTC timestamp when the service request was created
    /// Model Connection: Used for SLA tracking and performance analytics
    /// UX Trigger: Auto-set when customer submits service request
    /// Expected Outcome: Baseline for all service timing metrics and reporting
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Scheduled date and time for service delivery
    /// Model Connection: Used for technician calendar management and customer expectations
    /// UX Trigger: Set during scheduling workflow or customer self-scheduling
    /// Expected Outcome: Drives technician schedule and customer appointment confirmations
    /// </summary>
    public DateTime? ScheduledDate { get; set; }
    
    /// <summary>
    /// UTC timestamp when the service was marked as completed
    /// Model Connection: Used for SLA compliance tracking and billing calculations
    /// UX Trigger: Auto-set when technician marks service as complete
    /// Expected Outcome: Triggers completion workflows and customer satisfaction surveys
    /// Cognitive Impact: Feeds into Nova AI performance optimization models
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Navigation property to the associated tenant (company)
    /// Model Connection: Links to RevitalizeTenant for multi-tenant operations
    /// </summary>
    public virtual RevitalizeTenant Tenant { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to the assigned technician profile
    /// Model Connection: Links to RevitalizeTechnicianProfile when assigned
    /// UX Trigger: Populated when technician assignment is made
    /// </summary>
    public virtual RevitalizeTechnicianProfile? AssignedTechnician { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Enumeration of service types available in Revitalize platform
/// Categorizes services for technician specialization matching and resource planning
/// UX Impact: Displayed in service request creation dropdown and filtering options
/// Cognitive Impact: Used by Nova AI for predictive service duration and resource allocation
/// </summary>
public enum RevitalizeServiceType
{
    /// <summary>
    /// Plumbing services including repairs, installations, and maintenance
    /// </summary>
    Plumbing = 1,
    
    /// <summary>
    /// Heating, ventilation, and air conditioning services
    /// </summary>
    HVAC = 2,
    
    /// <summary>
    /// Water filtration system services and maintenance
    /// </summary>
    WaterFiltration = 3,
    
    /// <summary>
    /// Emergency services requiring immediate response
    /// </summary>
    Emergency = 4,
    
    /// <summary>
    /// Scheduled maintenance and preventive services
    /// </summary>
    Maintenance = 5
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Priority levels for service request handling
/// Controls service request queue ordering and resource allocation
/// UX Impact: Affects display order and visual indicators in service request lists
/// Expected Outcome: Drives SLA expectations and technician assignment priority
/// </summary>
public enum RevitalizePriority
{
    /// <summary>
    /// Low priority - non-urgent requests with flexible scheduling
    /// </summary>
    Low = 1,
    
    /// <summary>
    /// Medium priority - standard service requests with normal scheduling
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// High priority - urgent requests requiring expedited handling
    /// </summary>
    High = 3,
    
    /// <summary>
    /// Emergency priority - immediate response required
    /// </summary>
    Emergency = 4
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Workflow status values for service request lifecycle
/// Tracks service request progress through the complete fulfillment process
/// UX Impact: Controls available actions and status indicators in interfaces
/// Empathy Impact: Status changes trigger appropriate LyraEmpathyIntakeNarrator responses
/// </summary>
public enum RevitalizeServiceRequestStatus
{
    /// <summary>
    /// Request created but not yet assigned to a technician
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// Request assigned to a technician but work not yet started
    /// </summary>
    Assigned = 2,
    
    /// <summary>
    /// Technician is actively working on the service request
    /// </summary>
    InProgress = 3,
    
    /// <summary>
    /// Service request has been completed successfully
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// Service request was cancelled before completion
    /// </summary>
    Cancelled = 5
}