namespace MVP_Core.Services.Services;

/// <summary>
/// Trust License Service for Sprint122_CertumDNSBypass
/// Provides reputation-based verification for licensing
/// </summary>
public class TrustLicenseService
{
    private readonly Dictionary<string, TrustLevel> _trustRegistry;
    private readonly DateTime _serviceStartTime;
    
    public enum TrustLevel
    {
        Untrusted = 0,
        Basic = 1,
        Verified = 2,
        Trusted = 3,
        HighlyTrusted = 4
    }
    
    public TrustLicenseService()
    {
        _trustRegistry = new Dictionary<string, TrustLevel>();
        _serviceStartTime = DateTime.UtcNow;
        
        // Initialize with some basic trust entries
        InitializeTrustRegistry();
    }
    
    /// <summary>
    /// Initializes the trust registry with default entries
    /// </summary>
    private void InitializeTrustRegistry()
    {
        _trustRegistry["localhost"] = TrustLevel.Trusted;
        _trustRegistry["127.0.0.1"] = TrustLevel.Trusted;
        _trustRegistry["FixItFred.AI"] = TrustLevel.HighlyTrusted;
        _trustRegistry["MechSolATL.system"] = TrustLevel.HighlyTrusted;
    }
    
    /// <summary>
    /// Verifies trust level for a given entity
    /// </summary>
    public TrustLevel VerifyTrust(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            return TrustLevel.Untrusted;
            
        return _trustRegistry.GetValueOrDefault(entityId, TrustLevel.Untrusted);
    }
    
    /// <summary>
    /// Registers a new trust entry with reputation-based verification
    /// </summary>
    public bool RegisterTrust(string entityId, TrustLevel trustLevel, string verificationSource)
    {
        if (string.IsNullOrWhiteSpace(entityId) || string.IsNullOrWhiteSpace(verificationSource))
            return false;
            
        // Reputation-based verification logic
        var currentTrust = VerifyTrust(verificationSource);
        if (currentTrust < TrustLevel.Verified)
            return false; // Source not trusted enough to register others
            
        _trustRegistry[entityId] = trustLevel;
        LogTrustRegistration(entityId, trustLevel, verificationSource);
        
        return true;
    }
    
    /// <summary>
    /// Updates trust level based on reputation feedback
    /// </summary>
    public void UpdateTrustLevel(string entityId, TrustLevel newLevel, string reason)
    {
        if (_trustRegistry.ContainsKey(entityId))
        {
            var oldLevel = _trustRegistry[entityId];
            _trustRegistry[entityId] = newLevel;
            LogTrustUpdate(entityId, oldLevel, newLevel, reason);
        }
    }
    
    /// <summary>
    /// Gets all trusted entities above a certain level
    /// </summary>
    public Dictionary<string, TrustLevel> GetTrustedEntities(TrustLevel minimumLevel = TrustLevel.Verified)
    {
        return _trustRegistry
            .Where(kvp => kvp.Value >= minimumLevel)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    
    /// <summary>
    /// Validates if an operation is authorized for the given entity
    /// </summary>
    public bool IsOperationAuthorized(string entityId, string operation)
    {
        var trustLevel = VerifyTrust(entityId);
        
        return operation.ToLower() switch
        {
            "read" => trustLevel >= TrustLevel.Basic,
            "write" => trustLevel >= TrustLevel.Verified,
            "admin" => trustLevel >= TrustLevel.Trusted,
            "system" => trustLevel >= TrustLevel.HighlyTrusted,
            _ => false
        };
    }
    
    private void LogTrustRegistration(string entityId, TrustLevel trustLevel, string verificationSource)
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] TRUST_REGISTER: {entityId} -> {trustLevel} (Verified by: {verificationSource})");
    }
    
    private void LogTrustUpdate(string entityId, TrustLevel oldLevel, TrustLevel newLevel, string reason)
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] TRUST_UPDATE: {entityId} {oldLevel} -> {newLevel} (Reason: {reason})");
    }
    
    /// <summary>
    /// Gets service status and statistics
    /// </summary>
    public string GetServiceStatus()
    {
        var totalEntries = _trustRegistry.Count;
        var trustedCount = _trustRegistry.Count(kvp => kvp.Value >= TrustLevel.Trusted);
        var uptime = DateTime.UtcNow - _serviceStartTime;
        
        return $"TrustLicenseService: {totalEntries} total entries, {trustedCount} trusted, Uptime: {uptime:hh\\:mm\\:ss}";
    }
}