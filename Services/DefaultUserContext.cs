using Interfaces;
using System.Security.Principal;

namespace Services
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Default implementation of IUserContext
    /// Provides fallback user context when no authenticated user is available
    /// </summary>
    public class DefaultUserContext : IUserContext
    {
        /// <summary>
        /// [Sprint123_FixItFred_OmegaSweep] Mock user principal for default context
        /// </summary>
        public IPrincipal? User { get; } = new GenericPrincipal(new GenericIdentity("system"), new[] { "Admin" });

        /// <summary>
        /// [Sprint123_FixItFred_OmegaSweep] Gets current user identifier
        /// </summary>
        /// <returns>System user identifier</returns>
        public string GetCurrentUser()
        {
            return User?.Identity?.Name ?? "system";
        }
    }
}