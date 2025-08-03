using Interfaces;
using System.Security.Claims;

namespace Services
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Default implementation of IUserContext
    /// Provides fallback user context when no authenticated user is available
    /// </summary>
    public class DefaultUserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DefaultUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// [Sprint123_FixItFred_OmegaSweep] Gets current ClaimsPrincipal representing the authenticated user
        /// </summary>
        public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

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