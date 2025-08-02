using Interfaces;

namespace Data.Models;

/// <summary>
/// Default implementation of IUserContext for dependency injection
/// Provides basic user context functionality for the application
/// </summary>
public class DefaultUserContext : IUserContext
{
    /// <summary>
    /// Gets the current user identifier
    /// Returns system default when no authenticated user is available
    /// </summary>
    /// <returns>Current user identifier or "System" default</returns>
    public string GetCurrentUser()
    {
        return "System"; // Default implementation
    }
}