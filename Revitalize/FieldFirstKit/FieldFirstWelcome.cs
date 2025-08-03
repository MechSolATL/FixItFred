namespace MVP_Core.Services.Revitalize.FieldFirstKit;

/// <summary>
/// Field First Welcome class for Sprint122_CertumDNSBypass
/// Provides welcome functionality for field-first vision implementation
/// </summary>
public class FieldFirstWelcome
{
    public string WelcomeMessage { get; private set; }
    public DateTime InitializationTime { get; private set; }
    
    public FieldFirstWelcome()
    {
        WelcomeMessage = "Welcome to Field First Vision - Sprint122_CertumDNSBypass";
        InitializationTime = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Initializes the field first welcome sequence
    /// </summary>
    public void Initialize()
    {
        Console.WriteLine($"[{InitializationTime:yyyy-MM-dd HH:mm:ss}] {WelcomeMessage}");
    }
    
    /// <summary>
    /// Gets the current status of the field first system
    /// </summary>
    public string GetStatus()
    {
        return $"Field First System Active - Initialized at {InitializationTime:yyyy-MM-dd HH:mm:ss}";
    }
}