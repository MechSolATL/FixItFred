namespace MVP_Core.Services
{
    public class AuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;

        public AuditLogger(ILogger<AuditLogger> logger)
        {
            _logger = logger;
        }

        public void Log(string action, string user, string? notes = null)
        {
            string log = $"[AUDIT] User: {user} | Action: {action} | Notes: {notes ?? "None"}";
            _logger.LogInformation(log);
        }
    }
}
