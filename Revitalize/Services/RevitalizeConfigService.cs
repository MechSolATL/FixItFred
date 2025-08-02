namespace Revitalize.Services;

/// <summary>
/// Configuration service for Revitalize platform settings
/// </summary>
public interface IRevitalizeConfigService
{
    string GetPlatformName();
    string GetPlatformVersion();
    bool IsSaaSModeEnabled();
    int GetMaxTenantsAllowed();
    string GetDefaultTenantTheme();
}

/// <summary>
/// Configuration service for the Revitalize SaaS platform
/// </summary>
public class RevitalizeConfigService : IRevitalizeConfigService
{
    private readonly IConfiguration _configuration;

    public RevitalizeConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetPlatformName()
    {
        return _configuration.GetValue<string>("Revitalize:PlatformName") ?? "Revitalize SaaS";
    }

    public string GetPlatformVersion()
    {
        return _configuration.GetValue<string>("Revitalize:Version") ?? "1.0.0";
    }

    public bool IsSaaSModeEnabled()
    {
        return _configuration.GetValue<bool>("Revitalize:SaaSMode", true);
    }

    public int GetMaxTenantsAllowed()
    {
        return _configuration.GetValue<int>("Revitalize:MaxTenants", 100);
    }

    public string GetDefaultTenantTheme()
    {
        return _configuration.GetValue<string>("Revitalize:DefaultTheme") ?? "blue";
    }
}