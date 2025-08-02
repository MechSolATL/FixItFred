using System.Security.Principal;

namespace Interfaces
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Interface for user context management
    /// Provides access to current user information and authentication state
    /// Used by: Services requiring user authentication and authorization
    /// CLI Trigger: All authenticated CLI operations
    /// UI Trigger: All authenticated web interface operations
    /// Side Effects: Tracks user activity, maintains security context
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// [Sprint123_FixItFred_OmegaSweep] Current user principal with identity and roles
        /// </summary>
        IPrincipal? User { get; }

        /// <summary>
        /// [Sprint123_FixItFred_OmegaSweep] Gets current user identifier string
        /// </summary>
        /// <returns>Current user name or identifier</returns>
        string GetCurrentUser();
    }
}
