using System.Security.Claims;
using Interfaces;

namespace Services
{
    /// <summary>
    /// Concrete implementation of IUserContext providing access to current user information
    /// [Sprint123_FixItFred_OmegaSweep] Created UserContext service to resolve ClaimsPrincipal access
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current ClaimsPrincipal representing the authenticated user
        /// </summary>
        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

        /// <summary>
        /// Gets the current user's identity name or returns "Anonymous" if not authenticated
        /// </summary>
        public string GetCurrentUser()
        {
            return User.Identity?.Name ?? "Anonymous";
        }
    }
}