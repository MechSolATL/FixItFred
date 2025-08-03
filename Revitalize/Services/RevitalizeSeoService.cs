using Services;

namespace Revitalize.Services;

/// <summary>
/// SEO service extension for Revitalize SaaS pages
/// </summary>
public interface IRevitalizeSeoService
{
    Task<string> GeneratePageTitleAsync(string basePage, string? tenantName = null);
    Task<string> GenerateMetaDescriptionAsync(string pageType, string? context = null);
    Task<string> GenerateKeywordsAsync(string serviceType);
    Task<Dictionary<string, string>> GetSeoMetaTagsAsync(string page, object? model = null);
}

/// <summary>
/// Enhanced SEO service for Revitalize multi-tenant SaaS platform
/// </summary>
public class RevitalizeSeoService : IRevitalizeSeoService
{
    private readonly SEOService _baseSeoService;
    private readonly IRevitalizeConfigService _configService;

    public RevitalizeSeoService(SEOService baseSeoService, IRevitalizeConfigService configService)
    {
        _baseSeoService = baseSeoService;
        _configService = configService;
    }

    public async Task<string> GeneratePageTitleAsync(string basePage, string? tenantName = null)
    {
        await Task.Delay(1); // Simulate async operation
        
        var platformName = _configService.GetPlatformName();
        var title = basePage switch
        {
            "Dashboard" => "Dashboard",
            "ServiceRequests" => "Service Requests",
            "TenantManagement" => "Tenant Management",
            "TechnicianProfile" => "Technician Profile",
            _ => basePage
        };

        if (!string.IsNullOrEmpty(tenantName))
        {
            return $"{title} - {tenantName} | {platformName}";
        }

        return $"{title} | {platformName}";
    }

    public async Task<string> GenerateMetaDescriptionAsync(string pageType, string? context = null)
    {
        await Task.Delay(1);
        
        return pageType switch
        {
            "Dashboard" => $"Manage your service operations with {_configService.GetPlatformName()}. Track requests, technicians, and performance metrics.",
            "ServiceRequests" => "View and manage all service requests including plumbing, HVAC, and water filtration services. Assign technicians and track progress.",
            "TenantManagement" => "Multi-tenant SaaS platform management. Configure tenants, branding, and access controls.",
            "TechnicianProfile" => "Technician management and performance tracking. View skills, ratings, and job history.",
            _ => $"Professional service management platform for plumbing, HVAC, and water filtration businesses."
        };
    }

    public async Task<string> GenerateKeywordsAsync(string serviceType)
    {
        await Task.Delay(1);
        
        var baseKeywords = "service management, saas platform, technician management, customer portal";
        
        return serviceType switch
        {
            "Plumbing" => $"{baseKeywords}, plumbing services, plumbing repair, emergency plumbing",
            "HVAC" => $"{baseKeywords}, hvac services, heating, air conditioning, hvac repair",
            "WaterFiltration" => $"{baseKeywords}, water filtration, water treatment, water quality",
            "Emergency" => $"{baseKeywords}, emergency services, 24/7 service, urgent repair",
            _ => baseKeywords
        };
    }

    public async Task<Dictionary<string, string>> GetSeoMetaTagsAsync(string page, object? model = null)
    {
        await Task.Delay(1);
        
        var title = await GeneratePageTitleAsync(page);
        var description = await GenerateMetaDescriptionAsync(page);
        var keywords = await GenerateKeywordsAsync("General");

        return new Dictionary<string, string>
        {
            ["title"] = title,
            ["description"] = description,
            ["keywords"] = keywords,
            ["og:title"] = title,
            ["og:description"] = description,
            ["og:type"] = "website",
            ["twitter:card"] = "summary",
            ["twitter:title"] = title,
            ["twitter:description"] = description,
            ["viewport"] = "width=device-width, initial-scale=1.0",
            ["robots"] = "index, follow"
        };
    }
}