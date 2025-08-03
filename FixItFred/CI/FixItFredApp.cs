// Sprint91_27 - FixItFred GitHub App for auto-patch suggestions and empathy diagnostics
using MVP_Core.Services.Diagnostics;

namespace MVP_Core.FixItFred.CI
{
    public class FixItFredApp
    {
        private readonly DiagnosticsRunner _diagnosticsRunner;
        private readonly ILogger<FixItFredApp> _logger;

        public FixItFredApp(DiagnosticsRunner diagnosticsRunner, ILogger<FixItFredApp> logger)
        {
            _diagnosticsRunner = diagnosticsRunner;
            _logger = logger;
        }

        /// <summary>
        /// Process pull request for auto-patch suggestions
        /// Sprint91_27 - GitHub App integration
        /// </summary>
        public async Task<PullRequestAnalysis> AnalyzePullRequest(string prNumber, string branchName)
        {
            _logger.LogInformation($"FixItFred analyzing PR #{prNumber} on branch {branchName}");

            var analysis = new PullRequestAnalysis
            {
                PrNumber = prNumber,
                BranchName = branchName,
                AnalyzedAt = DateTime.UtcNow,
                Suggestions = new List<string>(),
                EmpathyInsights = new List<string>()
            };

            try
            {
                // Run diagnostics
                var diagnostics = await _diagnosticsRunner.ExecuteFullDiagnostics();
                
                // Generate patch suggestions based on diagnostics
                analysis.Suggestions = GeneratePatchSuggestions(diagnostics);
                
                // Generate empathy insights
                analysis.EmpathyInsights = GenerateEmpathyInsights(diagnostics);
                
                // Log analysis results
                await LogAnalysisResults(analysis);

                _logger.LogInformation($"FixItFred PR analysis completed for #{prNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FixItFred PR analysis failed for #{prNumber}");
                analysis.HasErrors = true;
                analysis.ErrorMessage = ex.Message;
            }

            return analysis;
        }

        private List<string> GeneratePatchSuggestions(DiagnosticResults diagnostics)
        {
            var suggestions = new List<string>();

            // Sprint91_27 - Auto-patch suggestion logic
            if (diagnostics.UnregisteredServices.Any())
            {
                suggestions.Add($"🔧 Consider registering {diagnostics.UnregisteredServices.Count} unregistered services in Program.cs");
            }

            if (diagnostics.NonCompliantNamespaces?.Any() == true)
            {
                suggestions.Add($"📝 Update {diagnostics.NonCompliantNamespaces.Count} files to use MVP_Core.* namespace convention");
            }

            if (diagnostics.Warnings.Any(w => w.Contains("Obsolete")))
            {
                suggestions.Add("🧹 Clean up obsolete code to improve maintainability");
            }

            if (diagnostics.Warnings.Any(w => w.Contains("TODO")))
            {
                suggestions.Add("📋 Address TODO comments for better code quality");
            }

            return suggestions;
        }

        private List<string> GenerateEmpathyInsights(DiagnosticResults diagnostics)
        {
            var insights = new List<string>();

            // Sprint91_27 - Empathy-driven development insights
            if (diagnostics.Errors.Any())
            {
                insights.Add("🤝 The team might benefit from extra support - errors detected that could impact customer experience");
            }

            if (diagnostics.Duration.TotalMinutes > 5)
            {
                insights.Add("⏱️ Long diagnostic runtime detected - consider optimizing for better developer experience");
            }

            if (diagnostics.UnregisteredServices.Count > 5)
            {
                insights.Add("🎯 Multiple service registration gaps - team might need guidance on DI patterns");
            }

            if (!diagnostics.Warnings.Any() && !diagnostics.Errors.Any())
            {
                insights.Add("✨ Excellent code quality detected - team is doing great work for customers!");
            }

            return insights;
        }

        private async Task LogAnalysisResults(PullRequestAnalysis analysis)
        {
            var logPath = Path.Combine("Logs", "FixItFred", $"PR_Analysis_{analysis.PrNumber}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
            
            var logContent = $"FixItFred PR Analysis Report\n";
            logContent += $"PR Number: #{analysis.PrNumber}\n";
            logContent += $"Branch: {analysis.BranchName}\n";
            logContent += $"Analyzed: {analysis.AnalyzedAt:yyyy-MM-dd HH:mm:ss} UTC\n\n";

            if (analysis.Suggestions.Any())
            {
                logContent += "PATCH SUGGESTIONS:\n";
                logContent += string.Join("\n", analysis.Suggestions.Select(s => $"  {s}"));
                logContent += "\n\n";
            }

            if (analysis.EmpathyInsights.Any())
            {
                logContent += "EMPATHY INSIGHTS:\n";
                logContent += string.Join("\n", analysis.EmpathyInsights.Select(i => $"  {i}"));
                logContent += "\n\n";
            }

            await File.WriteAllTextAsync(logPath, logContent);
        }
    }

    public class PullRequestAnalysis
    {
        public string PrNumber { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public DateTime AnalyzedAt { get; set; }
        public List<string> Suggestions { get; set; } = new();
        public List<string> EmpathyInsights { get; set; } = new();
        public bool HasErrors { get; set; }
        public string? ErrorMessage { get; set; }
    }
}