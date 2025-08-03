namespace MVP_Core.Services;

/// <summary>
/// Seed License class for Sprint122_CertumDNSBypass
/// Defines license types and seeding mechanisms
/// </summary>
public class SeedLicense
{
    public enum LicenseType
    {
        Free = 0,
        Basic = 1,
        Professional = 2,
        Enterprise = 3,
        AI_Autonomous = 4
    }
    
    public string LicenseId { get; private set; }
    public LicenseType Type { get; private set; }
    public DateTime IssuedDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string IssuedTo { get; private set; }
    public bool IsActive { get; private set; }
    public Dictionary<string, object> Permissions { get; private set; }
    
    public SeedLicense(LicenseType licenseType, string issuedTo, TimeSpan validity)
    {
        LicenseId = GenerateLicenseId(licenseType);
        Type = licenseType;
        IssuedDate = DateTime.UtcNow;
        ExpiryDate = IssuedDate.Add(validity);
        IssuedTo = issuedTo ?? "Anonymous";
        IsActive = true;
        Permissions = InitializePermissions(licenseType);
        
        LogLicenseCreation();
    }
    
    /// <summary>
    /// Creates a seed license for AI autonomous operations
    /// </summary>
    public static SeedLicense CreateAISeedLicense(string aiSystemId)
    {
        var license = new SeedLicense(LicenseType.AI_Autonomous, aiSystemId, TimeSpan.FromDays(365));
        license.AddPermission("autonomous_operation", true);
        license.AddPermission("self_diagnosis", true);
        license.AddPermission("patch_automation", true);
        license.AddPermission("ethical_constraints", true);
        
        return license;
    }
    
    /// <summary>
    /// Creates a free license with basic permissions
    /// </summary>
    public static SeedLicense CreateFreeLicense(string userId)
    {
        return new SeedLicense(LicenseType.Free, userId, TimeSpan.FromDays(30));
    }
    
    /// <summary>
    /// Validates if the license is currently valid
    /// </summary>
    public bool IsValid()
    {
        return IsActive && DateTime.UtcNow <= ExpiryDate;
    }
    
    /// <summary>
    /// Checks if a specific permission is granted
    /// </summary>
    public bool HasPermission(string permission)
    {
        if (!IsValid())
            return false;
            
        return Permissions.ContainsKey(permission) && 
               Permissions[permission] is bool value && value;
    }
    
    /// <summary>
    /// Adds a permission to the license
    /// </summary>
    public void AddPermission(string permission, object value)
    {
        Permissions[permission] = value;
        LogPermissionChange(permission, "ADDED", value);
    }
    
    /// <summary>
    /// Removes a permission from the license
    /// </summary>
    public void RemovePermission(string permission)
    {
        if (Permissions.ContainsKey(permission))
        {
            var oldValue = Permissions[permission];
            Permissions.Remove(permission);
            LogPermissionChange(permission, "REMOVED", oldValue);
        }
    }
    
    /// <summary>
    /// Deactivates the license
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] LICENSE_DEACTIVATED: {LicenseId} for {IssuedTo}");
    }
    
    /// <summary>
    /// Extends the license validity
    /// </summary>
    public void Extend(TimeSpan additionalTime)
    {
        if (IsActive)
        {
            var oldExpiry = ExpiryDate;
            ExpiryDate = ExpiryDate.Add(additionalTime);
            Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] LICENSE_EXTENDED: {LicenseId} from {oldExpiry:yyyy-MM-dd} to {ExpiryDate:yyyy-MM-dd}");
        }
    }
    
    /// <summary>
    /// Gets license status summary
    /// </summary>
    public string GetStatus()
    {
        var status = IsValid() ? "VALID" : "INVALID";
        var daysRemaining = (ExpiryDate - DateTime.UtcNow).Days;
        
        return $"License {LicenseId}: {status}, Type: {Type}, Days Remaining: {daysRemaining}, Permissions: {Permissions.Count}";
    }
    
    private string GenerateLicenseId(LicenseType licenseType)
    {
        var prefix = licenseType switch
        {
            LicenseType.Free => "FREE",
            LicenseType.Basic => "BASIC", 
            LicenseType.Professional => "PRO",
            LicenseType.Enterprise => "ENT",
            LicenseType.AI_Autonomous => "AI_AUTO",
            _ => "UNKNOWN"
        };
        
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        
        return $"{prefix}-{timestamp}-{random}";
    }
    
    private Dictionary<string, object> InitializePermissions(LicenseType licenseType)
    {
        var permissions = new Dictionary<string, object>();
        
        switch (licenseType)
        {
            case LicenseType.Free:
                permissions["read_access"] = true;
                permissions["basic_operations"] = true;
                break;
                
            case LicenseType.Basic:
                permissions["read_access"] = true;
                permissions["write_access"] = true;
                permissions["basic_operations"] = true;
                break;
                
            case LicenseType.Professional:
                permissions["read_access"] = true;
                permissions["write_access"] = true;
                permissions["advanced_operations"] = true;
                permissions["reporting"] = true;
                break;
                
            case LicenseType.Enterprise:
                permissions["read_access"] = true;
                permissions["write_access"] = true;
                permissions["advanced_operations"] = true;
                permissions["admin_access"] = true;
                permissions["reporting"] = true;
                permissions["bulk_operations"] = true;
                break;
                
            case LicenseType.AI_Autonomous:
                permissions["autonomous_operation"] = true;
                permissions["self_diagnosis"] = true;
                permissions["patch_automation"] = true;
                permissions["system_access"] = true;
                permissions["ethical_constraints"] = true;
                break;
        }
        
        return permissions;
    }
    
    private void LogLicenseCreation()
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] LICENSE_CREATED: {LicenseId} Type: {Type} For: {IssuedTo} Valid until: {ExpiryDate:yyyy-MM-dd}");
    }
    
    private void LogPermissionChange(string permission, string action, object value)
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] PERMISSION_{action}: {LicenseId} {permission} = {value}");
    }
}