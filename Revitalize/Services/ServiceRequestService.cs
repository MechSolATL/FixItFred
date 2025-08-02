using Revitalize.Models;

namespace Revitalize.Services;

/// <summary>
/// [Sprint123_FixItFred] Interface for service request management operations
/// Core interface for Revitalize SaaS platform service request handling
/// Supports multi-tenant architecture and Nova AI integration
/// </summary>
public interface IServiceRequestService
{
    Task<RevitalizeServiceRequest?> GetServiceRequestAsync(int serviceRequestId);
    Task<IEnumerable<RevitalizeServiceRequest>> GetServiceRequestsByTenantAsync(int tenantId);
    Task<IEnumerable<RevitalizeServiceRequest>> GetServiceRequestsByTechnicianAsync(int technicianId);
    Task<RevitalizeServiceRequest> CreateServiceRequestAsync(RevitalizeServiceRequest request);
    Task<RevitalizeServiceRequest> UpdateServiceRequestAsync(RevitalizeServiceRequest request);
    Task<bool> AssignTechnicianAsync(int serviceRequestId, int technicianId);
    Task<bool> CompleteServiceRequestAsync(int serviceRequestId);
    Task<IEnumerable<RevitalizeServiceRequest>> GetPendingRequestsAsync(int tenantId);
}

/// <summary>
/// Service for managing service requests in the Revitalize platform
/// </summary>
public class ServiceRequestService : IServiceRequestService
{
    // In a real implementation, this would use Entity Framework DbContext
    private static readonly List<RevitalizeServiceRequest> _requests = new();
    private static int _nextId = 1;

    public async Task<RevitalizeServiceRequest?> GetServiceRequestAsync(int serviceRequestId)
    {
        await Task.Delay(1);
        return _requests.FirstOrDefault(r => r.ServiceRequestId == serviceRequestId);
    }

    public async Task<IEnumerable<RevitalizeServiceRequest>> GetServiceRequestsByTenantAsync(int tenantId)
    {
        await Task.Delay(1);
        return _requests.Where(r => r.TenantId == tenantId).ToList();
    }

    public async Task<IEnumerable<RevitalizeServiceRequest>> GetServiceRequestsByTechnicianAsync(int technicianId)
    {
        await Task.Delay(1);
        return _requests.Where(r => r.AssignedTechnicianId == technicianId).ToList();
    }

    public async Task<RevitalizeServiceRequest> CreateServiceRequestAsync(RevitalizeServiceRequest request)
    {
        await Task.Delay(1);
        request.ServiceRequestId = _nextId++;
        request.CreatedAt = DateTime.UtcNow;
        request.Status = RevitalizeServiceRequestStatus.Pending;
        _requests.Add(request);
        return request;
    }

    public async Task<RevitalizeServiceRequest> UpdateServiceRequestAsync(RevitalizeServiceRequest request)
    {
        await Task.Delay(1);
        var existing = _requests.FirstOrDefault(r => r.ServiceRequestId == request.ServiceRequestId);
        if (existing != null)
        {
            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.ServiceType = request.ServiceType;
            existing.Priority = request.Priority;
            existing.Status = request.Status;
            existing.CustomerName = request.CustomerName;
            existing.CustomerPhone = request.CustomerPhone;
            existing.CustomerEmail = request.CustomerEmail;
            existing.ServiceAddress = request.ServiceAddress;
            existing.ScheduledDate = request.ScheduledDate;
        }
        return request;
    }

    public async Task<bool> AssignTechnicianAsync(int serviceRequestId, int technicianId)
    {
        await Task.Delay(1);
        var request = _requests.FirstOrDefault(r => r.ServiceRequestId == serviceRequestId);
        if (request != null)
        {
            request.AssignedTechnicianId = technicianId;
            request.Status = RevitalizeServiceRequestStatus.Assigned;
            return true;
        }
        return false;
    }

    public async Task<bool> CompleteServiceRequestAsync(int serviceRequestId)
    {
        await Task.Delay(1);
        var request = _requests.FirstOrDefault(r => r.ServiceRequestId == serviceRequestId);
        if (request != null)
        {
            request.Status = RevitalizeServiceRequestStatus.Completed;
            request.CompletedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    public async Task<IEnumerable<RevitalizeServiceRequest>> GetPendingRequestsAsync(int tenantId)
    {
        await Task.Delay(1);
        return _requests.Where(r => r.TenantId == tenantId && r.Status == RevitalizeServiceRequestStatus.Pending).ToList();
    }
}