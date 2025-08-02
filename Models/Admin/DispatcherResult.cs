// FixItFred Patch Log: CS8618 property patch, required/default initializers added to resolve warnings. All DTOs now safe for nullability.
// FixItFred Patch: Resolved duplicate DTO conflicts by using canonical DTOs from Models.Admin
// FixItFred Patch Log — DTO Required Member Defaults
// 2024-07-24
// Error Codes Addressed: CS9035
// Purpose: Added default values for required members to ensure all object initializers compile.
using System;
using System.Collections.Generic;

namespace Models.Admin
{
    public class DispatcherResult
    {
        public required string Message { get; set; }
        public required string RequestDetails { get; set; } = string.Empty;
        public required List<string> TechnicianList { get; set; } = new();
        public required string ETA { get; set; } = string.Empty;
        public required string GeoLink { get; set; } = string.Empty;
        public required string RequestSummary { get; set; } = string.Empty;
        public required string AssignedTechName { get; set; } = string.Empty;
    }

    public class RequestSummaryDto
    {
        public int Id { get; set; }
        public required string ServiceType { get; set; } = string.Empty;
        public required string Technician { get; set; } = string.Empty;
        public required string Status { get; set; } = string.Empty;
        public required string Priority { get; set; } = string.Empty;
        public required string Zip { get; set; } = string.Empty;
        public int DelayMinutes { get; set; }
        public bool IsEmergency { get; set; }
        public bool DispatcherOverrideApplied { get; set; }
        public string? OverrideReason { get; set; }
    }
}
