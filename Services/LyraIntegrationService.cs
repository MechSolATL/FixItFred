// Sprint91_27 - Lyra integration service for empathy-driven narratives
namespace MVP_Core.Services
{
    public class Lyra
    {
        private readonly ILogger<Lyra> _logger;
        private static readonly string _empathyCorpusPath = Path.Combine("Logs", "EmpathyCorpus");

        public Lyra(ILogger<Lyra> logger)
        {
            _logger = logger;
            if (!Directory.Exists(_empathyCorpusPath))
            {
                Directory.CreateDirectory(_empathyCorpusPath);
            }
        }

        /// <summary>
        /// Trigger empathy report generation hook
        /// Sprint91_27 - Called on assessment completion
        /// </summary>
        public static async Task OnEmpathyReportGenerated(Guid assessmentId, string technicianId, string customerContext = "")
        {
            // Use console logging for static method since no logger instance available
            Console.WriteLine($"Lyra: Empathy report triggered for assessment {assessmentId}");

            var empathyData = new
            {
                AssessmentId = assessmentId,
                TechnicianId = technicianId,
                CustomerContext = customerContext,
                GeneratedAt = DateTime.UtcNow,
                EmpathyMetrics = GenerateEmpathyMetrics(customerContext),
                NarrativeElements = GenerateNarrativeElements()
            };

            // Export to empathy corpus
            await ExportToEmpathyCorpus(empathyData);
        }

        /// <summary>
        /// CLI narration export functionality
        /// Sprint91_27 - Enable CLI tools to contribute to empathy corpus
        /// </summary>
        public static async Task ExportNarrationFromCLI(string toolName, string narration, Dictionary<string, object>? metadata = null)
        {
            // Use console logging for static method since no logger instance available
            Console.WriteLine($"Lyra: CLI narration export from {toolName}");

            var cliData = new
            {
                ToolName = toolName,
                Narration = narration,
                ExportedAt = DateTime.UtcNow,
                Metadata = metadata ?? new Dictionary<string, object>(),
                Source = "CLI"
            };

            await ExportToEmpathyCorpus(cliData);
        }

        private static async Task ExportToEmpathyCorpus(object data)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = $"EmpathyExport_{timestamp}_{Guid.NewGuid().ToString("N").Substring(0, 8)}.json";
            var filePath = Path.Combine(_empathyCorpusPath, fileName);

            var jsonData = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, jsonData);
            Console.WriteLine($"Lyra: Empathy data exported to {filePath}");
        }

        private static Dictionary<string, double> GenerateEmpathyMetrics(string context)
        {
            // Sprint91_27 - Basic empathy metrics generation
            return new Dictionary<string, double>
            {
                ["CustomerSatisfactionIndicator"] = string.IsNullOrEmpty(context) ? 0.5 : 0.8,
                ["TechnicianEmpathyScore"] = 0.75,
                ["CommunicationClarity"] = 0.85,
                ["ProblemResolutionConfidence"] = 0.9
            };
        }

        private static List<string> GenerateNarrativeElements()
        {
            return new List<string>
            {
                "Professional assessment completed",
                "Customer concerns addressed",
                "Technical expertise demonstrated",
                "Clear communication maintained",
                "Service quality prioritized"
            };
        }
    }

    /// <summary>
    /// Lyra hooks for Revitalize pages
    /// Sprint91_27 - Integration points for documentation and CLI tools
    /// </summary>
    public static class LyraHooks
    {
        public static async Task DocumentationHook(string documentPath, string content)
        {
            await Lyra.ExportNarrationFromCLI("RevitalizeDocs", content, new Dictionary<string, object>
            {
                ["DocumentPath"] = documentPath,
                ["ContentLength"] = content.Length,
                ["ProcessedAt"] = DateTime.UtcNow
            });
        }

        public static async Task CLIToolHook(string toolName, string operation, object result)
        {
            var narration = $"CLI operation '{operation}' completed by {toolName}";
            await Lyra.ExportNarrationFromCLI(toolName, narration, new Dictionary<string, object>
            {
                ["Operation"] = operation,
                ["Result"] = result,
                ["ToolVersion"] = "Sprint91_27"
            });
        }
    }
}