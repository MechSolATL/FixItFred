using Revitalize.Models;

namespace Revitalize.Services;

/// <summary>
/// [Sprint123_FixItFred] Interface for tenant management operations
/// Core multi-tenancy support for Revitalize SaaS platform
/// Provides CRUD operations and tenant isolation for service requests
/// </summary>
public interface ITenantService
{
    Task<RevitalizeTenant?> GetTenantAsync(int tenantId);
    Task<RevitalizeTenant?> GetTenantByCodeAsync(string tenantCode);
    Task<IEnumerable<RevitalizeTenant>> GetAllTenantsAsync();
    Task<RevitalizeTenant> CreateTenantAsync(RevitalizeTenant tenant);
    Task<RevitalizeTenant> UpdateTenantAsync(RevitalizeTenant tenant);
    Task<bool> DeleteTenantAsync(int tenantId);
    Task<bool> IsTenantActiveAsync(int tenantId);
}

/// <summary>
/// Service for managing tenants in the Revitalize SaaS platform
/// </summary>
public class TenantService : ITenantService
{
    // In a real implementation, this would use Entity Framework DbContext
    // For now, using in-memory storage for minimal implementation
    private static readonly List<RevitalizeTenant> _tenants = new();
    private static int _nextId = 1;

    public async Task<RevitalizeTenant?> GetTenantAsync(int tenantId)
    {
        await Task.Delay(1); // Simulate async operation
        return _tenants.FirstOrDefault(t => t.TenantId == tenantId);
    }

    public async Task<RevitalizeTenant?> GetTenantByCodeAsync(string tenantCode)
    {
        await Task.Delay(1);
        return _tenants.FirstOrDefault(t => t.TenantCode == tenantCode);
    }

    public async Task<IEnumerable<RevitalizeTenant>> GetAllTenantsAsync()
    {
        await Task.Delay(1);
        return _tenants.Where(t => t.IsActive).ToList();
    }

    public async Task<RevitalizeTenant> CreateTenantAsync(RevitalizeTenant tenant)
    {
        await Task.Delay(1);
        tenant.TenantId = _nextId++;
        tenant.CreatedAt = DateTime.UtcNow;
        _tenants.Add(tenant);
        return tenant;
    }

    public async Task<RevitalizeTenant> UpdateTenantAsync(RevitalizeTenant tenant)
    {
        await Task.Delay(1);
        var existing = _tenants.FirstOrDefault(t => t.TenantId == tenant.TenantId);
        if (existing != null)
        {
            existing.CompanyName = tenant.CompanyName;
            existing.Description = tenant.Description;
            existing.IsActive = tenant.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        return tenant;
    }

    public async Task<bool> DeleteTenantAsync(int tenantId)
    {
        await Task.Delay(1);
        var tenant = _tenants.FirstOrDefault(t => t.TenantId == tenantId);
        if (tenant != null)
        {
            tenant.IsActive = false;
            tenant.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    public async Task<bool> IsTenantActiveAsync(int tenantId)
    {
        await Task.Delay(1);
        var tenant = _tenants.FirstOrDefault(t => t.TenantId == tenantId);
        return tenant?.IsActive ?? false;
    }
}