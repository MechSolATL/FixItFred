using Microsoft.AspNetCore.Http;

namespace Helpers
{
    public class SessionTracker
    {
        private readonly ISession? _session;

        public SessionTracker(IHttpContextAccessor accessor)
        {
            _session = accessor?.HttpContext?.Session;
        }

        // Verification
        public bool IsVerified
        {
            get => _session?.GetString("IsVerified") == "true";
            set { if (_session != null) _session.SetString("IsVerified", value ? "true" : "false"); }
        }

        public string CustomerName
        {
            get => _session?.GetString("CustomerName") ?? string.Empty;
            set { if (_session != null) _session.SetString("CustomerName", value ?? ""); }
        }

        public string CustomerEmail
        {
            get => _session?.GetString("CustomerEmail") ?? string.Empty;
            set { if (_session != null) _session.SetString("CustomerEmail", value ?? ""); }
        }

        public int? ServiceRequestId
        {
            get => _session?.GetInt32("ServiceRequestId");
            set { if (_session != null && value.HasValue) _session.SetInt32("ServiceRequestId", value.Value); }
        }

        public bool AnswersSubmitted
        {
            get => _session?.GetString("AnswersSubmitted") == "true";
            set { if (_session != null) _session.SetString("AnswersSubmitted", value ? "true" : "false"); }
        }

        public int VerificationAttempts
        {
            get => _session?.GetInt32("VerificationAttempts") ?? 0;
            set { if (_session != null) _session.SetInt32("VerificationAttempts", value); }
        }

        public void ClearAll()
        {
            if (_session == null) return;
            _session.Remove("IsVerified");
            _session.Remove("CustomerName");
            _session.Remove("CustomerEmail");
            _session.Remove("ServiceRequestId");
            _session.Remove("AnswersSubmitted");
            _session.Remove("VerificationAttempts");
        }
    }
}
