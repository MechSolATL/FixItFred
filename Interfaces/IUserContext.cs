using System.Security.Claims;

namespace Interfaces
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Interface for user context management
    /// Provides access to the authenticated user's claims and identity.
    /// Used by: ReplayEngineService, EmpathyNarrator, CLI Auth, FixItFred triggers
    /// UI Trigger: All authenticated overlay and empathy interactions
    /// CLI Trigger: Any authenticated replay or transcript requests
    /// Side Effects: Enables persona analytics, secure DI propagation
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// [Sprint123_FixItFred_OmegaSweep] Gets the current ClaimsPrincipal representing the authenticated user
        /// </summary>
        ClaimsPrincipal? User { get; }

        /// <summary>
        /// [Sprint123_FixItFred_OmegaSweep] Gets current user identifier string (e.g., name or tenant key)
        /// </summary>
        /// <returns>Authenticated user identifier</returns>
        string GetCurrentUser();
    }
}
