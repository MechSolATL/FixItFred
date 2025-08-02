using Microsoft.AspNetCore.Mvc.RazorPages;
using Revitalize.Models;
using Revitalize.Services;

namespace Pages.Revitalize.Dashboard;

public class IndexModel : PageModel
{
    private readonly IServiceRequestService _serviceRequestService;
    private readonly ITenantService _tenantService;

    public IndexModel(IServiceRequestService serviceRequestService, ITenantService tenantService)
    {
        _serviceRequestService = serviceRequestService;
        _tenantService = tenantService;
    }

    public int TotalServiceRequests { get; set; }
    public int ActiveTechnicians { get; set; }
    public int PendingRequests { get; set; }
    public int TotalTenants { get; set; }
    public List<RevitalizeServiceRequest> RecentRequests { get; set; } = new();

    public async Task OnGetAsync()
    {
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