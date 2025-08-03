// Sprint91_27 - Operation System Fusion Integration Test
using Microsoft.Extensions.Logging;
using Xunit;

namespace MVP_Core.Tests.Integration
{
    public class OperationSystemFusionTests
    {
        private readonly ILogger<OperationSystemFusionTests> _logger;

        public OperationSystemFusionTests()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<OperationSystemFusionTests>();
        }

        [Fact]
        public async Task FieldAssessmentReportService_GenerateReport_ReturnsMarkdown()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MVP_Core.Services.FieldAssessmentReportService>();
            var service = new MVP_Core.Services.FieldAssessmentReportService(logger);

            // Act
            var result = await service.GenerateAssessmentReport(jobId);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("# Field Assessment Report", result);
            Assert.Contains($"**Job ID:** {jobId}", result);
            Assert.Contains("Assessment → Estimate → Invoice", result);
        }

        [Fact]
        public async Task TechViewPatchOverlayService_GenerateJobSummary_ReturnsValidSummary()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var technicianId = "TECH123";
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MVP_Core.Services.TechViewPatchOverlayService>();
            var service = new MVP_Core.Services.TechViewPatchOverlayService(logger);

            // Act
            var summary = await service.GenerateJobSummary(jobId, technicianId);

            // Assert
            Assert.NotNull(summary);
            Assert.Equal(jobId, summary.JobId);
            Assert.Equal(technicianId, summary.TechnicianId);
            Assert.NotNull(summary.Flags);
            Assert.NotNull(summary.Suggestions);
            Assert.NotNull(summary.SafetyAlerts);
        }

        [Fact]
        public async Task LyraIntegration_OnEmpathyReportGenerated_ExportsToCorpus()
        {
            // Arrange
            var assessmentId = Guid.NewGuid();
            var technicianId = "TECH456";
            var customerContext = "Customer was initially frustrated but became cooperative";

            // Act
            await MVP_Core.Services.Lyra.OnEmpathyReportGenerated(assessmentId, technicianId, customerContext);

            // Assert
            // Check that empathy corpus directory exists and has files
            var corpusPath = Path.Combine("Logs", "EmpathyCorpus");
            Assert.True(Directory.Exists(corpusPath));
            
            var files = Directory.GetFiles(corpusPath, "EmpathyExport_*.json");
            Assert.True(files.Length > 0);
        }

        [Fact]
        public async Task CalendarSyncService_SyncGoogleEvent_CompletesWithoutError()
        {
            // Arrange
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MVP_Core.Services.Integrations.GoogleCalendarSyncService>();
            var service = new MVP_Core.Services.Integrations.GoogleCalendarSyncService(logger);
            
            var serviceRequest = new Data.Models.ServiceRequest
            {
                Id = 123,
                CustomerName = "Test Customer",
                ServiceType = "Plumbing"
            };

            // Act & Assert - Should not throw
            await service.SyncGoogleEvent(serviceRequest);
        }
    }
}