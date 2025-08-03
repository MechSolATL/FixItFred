// Sprint91_27 - Enhanced diagnostics runner for CI warnings collection
using MVP_Core.Services.Diagnostics;

namespace MVP_Core.Services.Diagnostics
{
    public class DiagnosticsRunner
    {
        private readonly IServiceModuleScanner _moduleScanner;
        private readonly ILogger<DiagnosticsRunner> _logger;

        public DiagnosticsRunner(IServiceModuleScanner moduleScanner, ILogger<DiagnosticsRunner> logger)
        {
            _moduleScanner = moduleScanner;
            _logger = logger;
        }

        /// <summary>
        /// Execute comprehensive diagnostics including CI warnings collection
        /// Sprint91_27 - FixItFred diagnostics enhancement
        /// </summary>
        public async Task<DiagnosticResults> ExecuteFullDiagnostics()
        {
            _logger.LogInformation("FixItFred: Starting comprehensive diagnostics execution");

            var results = new DiagnosticResults
            {
                StartTime = DateTime.UtcNow,
                Warnings = new List<string>(),
                Errors = new List<string>(),
                UnregisteredServices = new List<string>()
            };

            try
            {
                // Service module scanning
                results.UnregisteredServices = await _moduleScanner.ScanForUnregisteredServices();
                
                // CI warnings collection
                await CollectCIWarnings(results);
                
                // Additional diagnostics can be added here
                await CheckNamespaceCompliance(results);
                
                results.EndTime = DateTime.UtcNow;
                results.Success = !results.Errors.Any();

                _logger.LogInformation($"FixItFred diagnostics completed: {results.Warnings.Count} warnings, {results.Errors.Count} errors");
            }
            catch (Exception ex)
            {
                results.Errors.Add($"Diagnostics execution failed: {ex.Message}");
                results.Success = false;
                _logger.LogError(ex, "FixItFred diagnostics execution failed");
            }

            return results;
        }

        private async Task CollectCIWarnings(DiagnosticResults results)
        {
            // Sprint91_27 - CI warnings collector
            _logger.LogInformation("Collecting CI warnings");

            // Check for obsolete code
            if (await HasObsoleteCode())
            {
                results.Warnings.Add("Obsolete code detected - consider cleanup");
            }

            // Check for TODO comments
            var todoCount = await CountTodoComments();
            if (todoCount > 0)
            {
                results.Warnings.Add($"{todoCount} TODO comments found - consider addressing");
            }

            // Check for missing XML documentation
            if (await HasMissingDocumentation())
            {
                results.Warnings.Add("Missing XML documentation detected");
            }
        }

        private async Task CheckNamespaceCompliance(DiagnosticResults results)
        {
            // Sprint91_27 - Namespace compliance check
            _logger.LogInformation("Checking namespace compliance");

            var nonCompliantFiles = await GetNonCompliantNamespaceFiles();
            if (nonCompliantFiles.Any())
            {
                results.Warnings.Add($"Non-compliant namespaces found in {nonCompliantFiles.Count} files");
                results.NonCompliantNamespaces = nonCompliantFiles;
            }
        }

        private async Task<bool> HasObsoleteCode()
        {
            // Check for [Obsolete] attributes in code
            var searchPattern = "[Obsolete]";
            var files = Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                if (content.Contains(searchPattern))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<int> CountTodoComments()
        {
            var todoCount = 0;
            var files = Directory.GetFiles(".", "*.cs", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                todoCount += content.Split("TODO").Length - 1;
            }
            
            return todoCount;
        }

        private async Task<bool> HasMissingDocumentation()
        {
            // Simplified check - could be enhanced
            return await Task.FromResult(false);
        }

        private async Task<List<string>> GetNonCompliantNamespaceFiles()
        {
            var nonCompliantFiles = new List<string>();
            var files = Directory.GetFiles("./Services", "*.cs", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles("./Pages", "*.cs", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles("./Data", "*.cs", SearchOption.AllDirectories));

            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                if (content.Contains("namespace Services") || 
                    content.Contains("namespace Pages") ||
                    content.Contains("namespace Data"))
                {
                    if (!content.Contains("namespace MVP_Core."))
                    {
                        nonCompliantFiles.Add(file);
                    }
                }
            }

            return nonCompliantFiles;
        }
    }

    public class DiagnosticResults
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Success { get; set; }
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public List<string> UnregisteredServices { get; set; } = new();
        public List<string> NonCompliantNamespaces { get; set; } = new();
        public TimeSpan Duration => EndTime - StartTime;
    }
}