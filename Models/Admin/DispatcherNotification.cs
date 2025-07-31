// FixItFred Patch: Synced DTO references, resolved ambiguities, and corrected collection access for Dispatcher UI.
using System;

namespace Models.Admin
{
    public class DispatcherNotification
    {
        public DateTime Timestamp { get; set; }
        public required string Type { get; set; }
        public required string Message { get; set; }
        // ExpiresAt property for broadcast expiry logic
        public DateTime? ExpiresAt { get; set; }
    }
}
