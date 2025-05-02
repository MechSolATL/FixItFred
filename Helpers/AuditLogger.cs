using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVP_Core.Helpers
{
    public class AuditLogger
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditLogger> _logger;

        public AuditLogger(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<AuditLogger> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task LogChangeAsync(string changeType, string oldValue, string newValue)
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(userIdClaim, out int userId);

                var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    ChangeType = changeType,
                    OldValue = oldValue,
                    NewValue = newValue,
                    Timestamp = DateTime.UtcNow,
                    IPAddress = ipAddress
                };

                _dbContext.AuditLogs.Add(auditLog);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit entry.");
            }
        }
    }
}
