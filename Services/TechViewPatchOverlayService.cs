// Sprint91_27 - Tech View Patch Overlay for technician-facing job guidance
using Data.Models;

namespace MVP_Core.Services
{
    public class TechViewPatchOverlayService
    {
        private readonly ILogger<TechViewPatchOverlayService> _logger;

        public TechViewPatchOverlayService(ILogger<TechViewPatchOverlayService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Generate technician-facing job summary with patch guidance
        /// Sprint91_27 - NEVER auto-shown to client, only for tech guidance
        /// </summary>
        public async Task<TechnicianJobSummary> GenerateJobSummary(Guid jobId, string technicianId)
        {
            _logger.LogInformation($"Generating tech patch overlay for job {jobId}, technician {technicianId}");

            var summary = new TechnicianJobSummary
            {
                JobId = jobId,
                TechnicianId = technicianId,
                GeneratedAt = DateTime.UtcNow,
                Flags = new List<string>(),
                Suggestions = new List<string>(),
                SafetyAlerts = new List<string>()
            };

            // Anti-microbial suggestion logic
            if (await IsDuctworkFlagged(jobId))
            {
                summary.Suggestions.Add("🦠 ANTI-MICROBIAL TREATMENT: Consider UV sterilization or antimicrobial coating for ductwork");
                summary.Flags.Add("DUCTWORK_MICROBIAL_RISK");
            }

            // Crawlspace/pet risk evaluation
            if (await HasCrawlspacePetRisk(jobId))
            {
                summary.SafetyAlerts.Add("🐕 PET/CRAWLSPACE ALERT: Previous reports indicate pet access to work area - take appropriate precautions");
                summary.Flags.Add("CRAWLSPACE_PET_RISK");
            }

            // Line pressure test reminder
            if (await RequiresPipeRepair(jobId))
            {
                summary.SafetyAlerts.Add("🔧 PRESSURE TEST REQUIRED: Line pressure test mandatory after pipe repair - do not skip this step");
                summary.Flags.Add("PIPE_REPAIR_PRESSURE_TEST");
            }

            // General tech guidance
            summary.Suggestions.AddRange(await GetGeneralTechGuidance(jobId));

            _logger.LogInformation($"Tech patch overlay generated with {summary.Flags.Count} flags and {summary.Suggestions.Count} suggestions");

            return summary;
        }

        /// <summary>
        /// Get mobile-compatible overlay data for tech app
        /// Sprint91_27 - All overlays must be mobile-compatible
        /// </summary>
        public async Task<MobileOverlayData> GetMobileOverlay(Guid jobId, string technicianId)
        {
            var jobSummary = await GenerateJobSummary(jobId, technicianId);

            return new MobileOverlayData
            {
                JobId = jobId,
                Title = "Tech Guidance",
                Priority = jobSummary.SafetyAlerts.Any() ? "HIGH" : "NORMAL",
                Sections = new List<MobileOverlaySection>
                {
                    new MobileOverlaySection
                    {
                        Title = "🚨 Safety Alerts",
                        Items = jobSummary.SafetyAlerts,
                        Color = "#ff4444",
                        Collapsible = false
                    },
                    new MobileOverlaySection
                    {
                        Title = "💡 Suggestions",
                        Items = jobSummary.Suggestions,
                        Color = "#4CAF50",
                        Collapsible = true
                    },
                    new MobileOverlaySection
                    {
                        Title = "🏷️ Job Flags",
                        Items = jobSummary.Flags,
                        Color = "#2196F3",
                        Collapsible = true
                    }
                }
            };
        }

        /// <summary>
        /// Process "REPAIR APPROVED" outcome
        /// Sprint91_27 - Display on estimate conversion
        /// </summary>
        public async Task<ApprovalOutcome> ProcessRepairApproval(Guid jobId, string technicianId)
        {
            _logger.LogInformation($"Processing repair approval for job {jobId}");

            var outcome = new ApprovalOutcome
            {
                JobId = jobId,
                TechnicianId = technicianId,
                ApprovedAt = DateTime.UtcNow,
                Status = "REPAIR APPROVED",
                AuthorizationMessage = "Technician authorized to proceed",
                RequiredActions = new List<string>
                {
                    "Log job photos in system",
                    "Record all parts used",
                    "Track labor hours",
                    "Auto-timestamp CRM entry"
                }
            };

            // Auto-timestamp CRM entry
            await LogCRMEntry(jobId, technicianId, outcome);

            return outcome;
        }

        private async Task<bool> IsDuctworkFlagged(Guid jobId)
        {
            // Sprint91_27 - Check if previous assessments flagged ductwork issues
            // TODO: Implement when assessment repository is available
            await Task.Delay(1); // Placeholder for async
            return false; // Placeholder implementation
        }

        private async Task<bool> HasCrawlspacePetRisk(Guid jobId)
        {
            // Sprint91_27 - Check if previous reports logged pet access issues
            // Replace random logic with actual database/configuration lookup
            return await GetPetRiskFromDatabase(jobId);
        }

        // Stub for actual database/configuration lookup for pet risk
        private async Task<bool> GetPetRiskFromDatabase(Guid jobId)
        {
            // TODO: Replace with actual data access logic
            await Task.Delay(50); // Simulate async database call
            // For now, always return false (no pet risk)
            return false;
        }
        private async Task<bool> RequiresPipeRepair(Guid jobId)
        {
            // Sprint91_27 - Check if current job involves pipe repair
            // TODO: Replace with actual database query or configuration-based logic
            // Example: Query job details from database to determine if pipe repair is required
            // return await _jobRepository.JobRequiresPipeRepair(jobId);
            await Task.Delay(50); // Simulate async operation
            return false; // Default to false until implemented
        }

        private async Task<List<string>> GetGeneralTechGuidance(Guid jobId)
        {
            await Task.Delay(50);
            return new List<string>
            {
                "📋 Verify customer contact information before starting",
                "📸 Take before/after photos for documentation",
                "🔍 Inspect surrounding areas for related issues",
                "✅ Confirm all safety protocols are followed"
            };
        }

        private async Task LogCRMEntry(Guid jobId, string technicianId, ApprovalOutcome outcome)
        {
            // Sprint91_27 - Auto-timestamped CRM entry injection
            var crmEntry = new
            {
                JobId = jobId,
                TechnicianId = technicianId,
                Status = outcome.Status,
                Timestamp = outcome.ApprovedAt,
                AutoGenerated = true,
                Source = "TechViewPatchOverlay"
            };

            _logger.LogInformation($"CRM entry auto-generated for job {jobId}: {outcome.Status}");
            await Task.CompletedTask; // Placeholder for actual CRM integration
        }
    }

    public class TechnicianJobSummary
    {
        public Guid JobId { get; set; }
        public string TechnicianId { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public List<string> Flags { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
        public List<string> SafetyAlerts { get; set; } = new();
    }

    public class MobileOverlayData
    {
        public Guid JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = "NORMAL";
        public List<MobileOverlaySection> Sections { get; set; } = new();
    }

    public class MobileOverlaySection
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new();
        public string Color { get; set; } = "#000000";
        public bool Collapsible { get; set; } = true;
    }

    public class ApprovalOutcome
    {
        public Guid JobId { get; set; }
        public string TechnicianId { get; set; } = string.Empty;
        public DateTime ApprovedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AuthorizationMessage { get; set; } = string.Empty;
        public List<string> RequiredActions { get; set; } = new();
    }
}