using Microsoft.AspNetCore.Mvc.RazorPages;
using Revitalize.Models;
using Revitalize.Services;

namespace Pages.Revitalize.ServiceRequests;

public class IndexModel : PageModel
{
    private readonly IServiceRequestService _serviceRequestService;

    public IndexModel(IServiceRequestService serviceRequestService)
    {
        _serviceRequestService = serviceRequestService;
    }

    public List<RevitalizeServiceRequest> ServiceRequests { get; set; } = new();
    public string StatusFilter { get; set; } = string.Empty;
    public string ServiceTypeFilter { get; set; } = string.Empty;
    public string PriorityFilter { get; set; } = string.Empty;

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