using Microsoft.AspNetCore.Mvc.RazorPages;
using Revitalize.Models;
using Revitalize.Services;

namespace Pages.Revitalize.Dashboard;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Page model for Revitalize platform dashboard
/// Provides comprehensive overview of tenant-specific metrics and system health
/// UX Trigger: Main landing page after Revitalize platform login
/// Model Connection: Aggregates data from service requests, tenants, and technicians
/// Expected Outcome: Executive dashboard showing key performance indicators
/// Cognitive Impact: Data feeds into Nova AI performance optimization recommendations
/// Empathy Replay Impact: Recent requests analyzed for customer satisfaction trends
/// </summary>
public class IndexModel : PageModel
{
    private readonly IServiceRequestService _serviceRequestService;
    private readonly ITenantService _tenantService;

    /// <summary>
    /// Constructor for dashboard page model with required services
    /// </summary>
    /// <param name="serviceRequestService">Service for managing service requests</param>
    /// <param name="tenantService">Service for managing tenant operations</param>
    public IndexModel(IServiceRequestService serviceRequestService, ITenantService tenantService)
    {
        _serviceRequestService = serviceRequestService;
        _tenantService = tenantService;
    }

    /// <summary>
    /// Total count of service requests across current tenant
    /// UX Trigger: Displayed as primary KPI metric card
    /// Model Connection: Calculated from RevitalizeServiceRequest collection
    /// Expected Outcome: Shows overall service volume for business insights
    /// </summary>
    public int TotalServiceRequests { get; set; }
    
    /// <summary>
    /// Count of currently active technicians for the tenant
    /// UX Trigger: Displayed in resource availability section
    /// Model Connection: Calculated from RevitalizeTechnicianProfile with Active status
    /// Expected Outcome: Shows available workforce capacity
    /// Cognitive Impact: Used by Nova AI for workload distribution planning
    /// </summary>
    public int ActiveTechnicians { get; set; }
    
    /// <summary>
    /// Count of service requests awaiting technician assignment
    /// UX Trigger: Highlighted as actionable metric requiring attention
    /// Model Connection: RevitalizeServiceRequest with Pending status
    /// Expected Outcome: Drives assignment workflow prioritization
    /// Empathy Impact: High pending counts trigger proactive customer communications
    /// </summary>
    public int PendingRequests { get; set; }
    
    /// <summary>
    /// Total number of active tenants in the platform
    /// UX Trigger: Platform-wide metric for admin users
    /// Model Connection: Count of RevitalizeTenant with IsActive = true
    /// Expected Outcome: Shows platform growth and adoption metrics
    /// </summary>
    public int TotalTenants { get; set; }
    
    /// <summary>
    /// Collection of most recently created service requests
    /// UX Trigger: Displayed in "Recent Activity" section for quick access
    /// Model Connection: Latest RevitalizeServiceRequest entities ordered by CreatedAt
    /// Expected Outcome: Enables quick navigation to new service requests
    /// Cognitive Impact: Recent patterns analyzed for service trend identification
    /// </summary>
    public List<RevitalizeServiceRequest> RecentRequests { get; set; } = new();

    /// <summary>
    /// Current user ID for personalized features like Mentor Mode
    /// UX Trigger: Used by Lyra adaptive learning components
    /// Model Connection: Retrieved from authenticated user context
    /// Expected Outcome: Enables user-specific adaptive features
    /// </summary>
    public int CurrentUserId { get; set; }

    /// <summary>
    /// Handles GET request for dashboard page load
    /// Aggregates metrics and recent data for display
    /// UX Trigger: Page load or dashboard refresh action
    /// Expected Outcome: Populates all dashboard metrics and recent activity
    /// Side Effects: May trigger background Nova AI analysis of current metrics
    /// </summary>
    public async Task OnGetAsync()
    {
        // Get current user ID (mock for demo purposes)
        CurrentUserId = 1; // In production, this would come from User.Identity or claims
        
        // Get dashboard metrics
        // In a real implementation, these would be filtered by current tenant
        var allRequests = await _serviceRequestService.GetServiceRequestsByTenantAsync(1); // Default tenant for demo
        var allTenants = await _tenantService.GetAllTenantsAsync();

        TotalServiceRequests = allRequests.Count();
        ActiveTechnicians = 5; // Mock data - would come from technician service
        PendingRequests = allRequests.Count(r => r.Status == RevitalizeServiceRequestStatus.Pending);
        TotalTenants = allTenants.Count();

        // Get recent requests (last 10)
        RecentRequests = allRequests
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .ToList();
    }
}