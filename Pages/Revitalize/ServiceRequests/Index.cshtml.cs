using Microsoft.AspNetCore.Mvc.RazorPages;
using Revitalize.Models;
using Revitalize.Services;

namespace Pages.Revitalize.ServiceRequests;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Page model for service requests listing and filtering
/// Provides comprehensive service request management interface with advanced filtering
/// UX Trigger: Navigation from dashboard or direct access via service requests menu
/// Model Connection: Primary interface for RevitalizeServiceRequest CRUD operations
/// Expected Outcome: Filterable, sortable list of service requests with action capabilities
/// CLI Flag: Data accessible via RevitalizeCLI service list commands
/// Cognitive Impact: Filtering patterns analyzed by Nova AI for workflow optimization
/// Empathy Replay Impact: Service request data feeds LyraEmpathyIntakeNarrator context
/// </summary>
public class IndexModel : PageModel
{
    private readonly IServiceRequestService _serviceRequestService;

    /// <summary>
    /// Constructor for service requests page model
    /// </summary>
    /// <param name="serviceRequestService">Service for managing service request operations</param>
    public IndexModel(IServiceRequestService serviceRequestService)
    {
        _serviceRequestService = serviceRequestService;
    }

    /// <summary>
    /// Collection of service requests matching current filter criteria
    /// UX Trigger: Populated on page load and filter application
    /// Model Connection: Filtered subset of RevitalizeServiceRequest entities
    /// Expected Outcome: Displayed in data grid with sorting and action buttons
    /// Empathy Impact: Each request analyzed for customer sentiment and urgency indicators
    /// </summary>
    public List<RevitalizeServiceRequest> ServiceRequests { get; set; } = new();
    
    /// <summary>
    /// Current status filter selection
    /// UX Trigger: Set via status dropdown selection or URL parameter
    /// Model Connection: Filters RevitalizeServiceRequest by Status property
    /// Expected Outcome: Shows only requests matching selected status
    /// CLI Flag: Corresponds to --status filter in RevitalizeCLI
    /// </summary>
    public string StatusFilter { get; set; } = string.Empty;
    
    /// <summary>
    /// Current service type filter selection
    /// UX Trigger: Set via service type dropdown or URL parameter
    /// Model Connection: Filters RevitalizeServiceRequest by ServiceType property
    /// Expected Outcome: Shows only requests of selected service type
    /// Cognitive Impact: Filter patterns help Nova AI predict service demand
    /// </summary>
    public string ServiceTypeFilter { get; set; } = string.Empty;
    
    /// <summary>
    /// Current priority filter selection
    /// UX Trigger: Set via priority dropdown or URL parameter
    /// Model Connection: Filters RevitalizeServiceRequest by Priority property
    /// Expected Outcome: Shows only requests matching priority level
    /// Empathy Impact: High priority filters trigger enhanced LyraEmpathy responses
    /// </summary>
    public string PriorityFilter { get; set; } = string.Empty;

    /// <summary>
    /// Handles GET request for service requests page with optional filtering
    /// Retrieves and filters service requests based on provided criteria
    /// UX Trigger: Page load, filter changes, or direct URL navigation
    /// Expected Outcome: Displays filtered service requests in sortable grid
    /// Side Effects: Filter usage patterns logged for Nova AI workflow optimization
    /// </summary>
    /// <param name="status">Optional status filter parameter</param>
    /// <param name="serviceType">Optional service type filter parameter</param>
    /// <param name="priority">Optional priority filter parameter</param>
    public async Task OnGetAsync(string? status, string? serviceType, string? priority)
    {
        StatusFilter = status ?? string.Empty;
        ServiceTypeFilter = serviceType ?? string.Empty;
        PriorityFilter = priority ?? string.Empty;

        // Get all service requests for the current tenant (using tenant 1 for demo)
        var allRequests = await _serviceRequestService.GetServiceRequestsByTenantAsync(1);
        ServiceRequests = allRequests.ToList();

        // Apply filters
        if (!string.IsNullOrEmpty(StatusFilter))
        {
            if (Enum.TryParse<RevitalizeServiceRequestStatus>(StatusFilter, out var statusEnum))
            {
                ServiceRequests = ServiceRequests.Where(r => r.Status == statusEnum).ToList();
            }
        }

        if (!string.IsNullOrEmpty(ServiceTypeFilter))
        {
            if (Enum.TryParse<RevitalizeServiceType>(ServiceTypeFilter, out var typeEnum))
            {
                ServiceRequests = ServiceRequests.Where(r => r.ServiceType == typeEnum).ToList();
            }
        }

        if (!string.IsNullOrEmpty(PriorityFilter))
        {
            if (Enum.TryParse<RevitalizePriority>(PriorityFilter, out var priorityEnum))
            {
                ServiceRequests = ServiceRequests.Where(r => r.Priority == priorityEnum).ToList();
            }
        }

        // Sort by creation date (newest first)
        ServiceRequests = ServiceRequests.OrderByDescending(r => r.CreatedAt).ToList();
    }
}