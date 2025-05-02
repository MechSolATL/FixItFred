// Generated for Service-Atlanta.com - Service Request Dashboard
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.Extensions.Logging;

namespace MVP_Core.Services
{
    /// <summary>
    /// Responsible for logging user changes and actions to the audit log table.
    /// </summary>
    public class AuditLogger
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditLogger> _logger;

        public AuditLogger(ApplicationDbContext context, ILogger<AuditLogger> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Logs a change to the AuditLog table with full traceability.
        /// </summary>
        /// <param name="userId">The ID of the user making the change (0 if anonymous).</param>
        /// <param name="changeType">Type of change (e.g. Update, Delete).</param>
        /// <param name="oldValue">The previous value before the change.</param>
        /// <param name="newValue">The new value after the change.</param>
        /// <param name="ipAddress">The IP address of the requester.</param>
        public async Task LogChangeAsync(int userId, string changeType, string oldValue, string newValue, string ipAddress)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = userId,
                    ChangeType = changeType,
                    OldValue = oldValue,
                    NewValue = newValue,
                    IPAddress = string.IsNullOrWhiteSpace(ipAddress) ? "Unknown" : ipAddress,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Audit logging failed: {ChangeType}", changeType);
            }
        }
    }
}
