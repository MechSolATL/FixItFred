// FixItFred Patch Log — Sprint 26.4E
// [2025-07-25T00:00:00Z] — ETAHistoryEntry model for ETA accuracy reporting.
using System;

namespace MVP_Core.Models.Dispatch
{
    public class ETAHistoryEntry
    {
        public string TechnicianName { get; set; } = string.Empty;
        public int ServiceRequestId { get; set; }
        public DateTime PredictedETA { get; set; }
        public DateTime? ActualArrival { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
