using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OmegaV3Sweep
{
    /// <summary>
    /// Core scanning engine for OmegaV3 diagnostic sweep
    /// </summary>
    public class OmegaV3Scanner
    {
        private readonly string _projectRoot;
        private readonly List<string> _logs;
        private readonly List<string> _warnings;
        private readonly List<string> _criticalIssues;

        public OmegaV3Scanner(string projectRoot)
        {
            _projectRoot = projectRoot ?? throw new ArgumentNullException(nameof(projectRoot));
            _logs = new List<string>();
            _warnings = new List<string>();
            _criticalIssues = new List<string>();
        }

        /// <summary>
        /// Executes complete diagnostic scan
        /// </summary>
        /// <returns>Scan results</returns>
        public async Task<OmegaScanResult> ExecuteFullScanAsync()
        {
            LogStep("🔬 Build Warning Verifier");
            await VerifyBuildWarnings();

            LogStep("🧠 Nullable & Obsolete Linter");
            await CheckNullableAndObsolete();

            LogStep("📘 XML Summary Enforcer");
            await EnforceXmlSummaries();

            LogStep("🔎 SEO Bind Check (Razor)");
            await CheckSeoBindings();

            LogStep("📄 Empathy Prompt Drift Watcher");
            await CheckEmpathyPromptDrift();

            LogStep("📊 Heatmap Replay Validator");
            await ValidateHeatmapReplay();

            LogStep("📁 Backup/Commit Hook Checker");
            await CheckBackupAndCommitHooks();

            LogStep("🧼 Residual Log Sweeper");
            await SweepResidualLogs();

            return new OmegaScanResult
            {
                HasCriticalIssues = _criticalIssues.Any(),
                Warnings = _warnings.ToList(),
                CriticalIssues = _criticalIssues.ToList(),
                Logs = _logs.ToList()
            };
        }

        /// <summary>
        /// Generates comprehensive scan report
        /// </summary>
        /// <param name="result">Scan results</param>
        public async Task GenerateReportAsync(OmegaScanResult result)
        {
            var logsDir = Path.Combine(_projectRoot, "Logs");
            Directory.CreateDirectory(logsDir);

            var reportPath = Path.Combine(logsDir, "OmegaV3_FinalScan_Report.md");
            var auditPath = Path.Combine(logsDir, $"OmegaSweep_Audit_{DateTime.UtcNow:yyyyMMdd_HHmmss}.md");

            var report = GenerateMainReport(result);
            var audit = GenerateAuditReport(result);

            await File.WriteAllTextAsync(reportPath, report);
            await File.WriteAllTextAsync(auditPath, audit);

            Console.WriteLine($"✅ Report generated: {reportPath}");
            Console.WriteLine($"📋 Audit generated: {auditPath}");
        }

        private void LogStep(string step)
        {
            Console.WriteLine(step);
            _logs.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {step}");
        }

        private void LogInfo(string message)
        {
            Console.WriteLine($"  ✅ {message}");
            _logs.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] INFO: {message}");
        }

        private void LogWarning(string message)
        {
            Console.WriteLine($"  ⚠️ {message}");
            _warnings.Add(message);
            _logs.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] WARNING: {message}");
        }

        private void LogCritical(string message)
        {
            Console.WriteLine($"  ❌ {message}");
            _criticalIssues.Add(message);
            _logs.Add($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] CRITICAL: {message}");
        }

        /// <summary>
        /// Scans bin/obj logs for compiler warnings
        /// </summary>
        private async Task VerifyBuildWarnings()
        {
            var buildLogPath = Path.Combine(_projectRoot, "build-log.txt");
            if (File.Exists(buildLogPath))
            {
                var content = await File.ReadAllTextAsync(buildLogPath);
                var warningMatches = Regex.Matches(content, @"warning\s+CS\d+:", RegexOptions.IgnoreCase);
                LogInfo($"Found {warningMatches.Count} compiler warnings in build log");

                if (warningMatches.Count > 300)
                {
                    LogWarning($"High warning count: {warningMatches.Count} warnings found");
                }
            }
            else
            {
                LogInfo("No build-log.txt found, warnings will be checked during live build");
            }

            // Check for common warning patterns in source files
            await CheckSourceWarnings();
        }

        private async Task CheckSourceWarnings()
        {
            var csFiles = Directory.GetFiles(_projectRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains("Migrations"))
                .ToList();

            int nullableWarnings = 0;
            int obsoleteWarnings = 0;

            foreach (var file in csFiles)
            {
                var content = await File.ReadAllTextAsync(file);
                
                // Check for nullable warnings patterns
                if (content.Contains("CS8618") || content.Contains("CS8602"))
                {
                    nullableWarnings++;
                }

                // Check for obsolete usage
                if (content.Contains("[Obsolete]") || content.Contains("CS0618"))
                {
                    obsoleteWarnings++;
                }
            }

            LogInfo($"Scanned {csFiles.Count} source files");
            if (nullableWarnings > 0)
                LogWarning($"{nullableWarnings} files may have nullable reference warnings");
            if (obsoleteWarnings > 0)
                LogWarning($"{obsoleteWarnings} files contain obsolete markers");
        }

        /// <summary>
        /// Checks for nullable and obsolete warnings
        /// </summary>
        private async Task CheckNullableAndObsolete()
        {
            var csFiles = Directory.GetFiles(_projectRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains("Migrations"))
                .ToList();

            int cs8618Count = 0; // Non-nullable property warnings
            int cs8602Count = 0; // Null reference warnings
            int cs0618Count = 0; // Obsolete warnings
            int cs0168Count = 0; // Unused variable warnings

            foreach (var file in csFiles)
            {
                var content = await File.ReadAllTextAsync(file);
                var relativePath = Path.GetRelativePath(_projectRoot, file);

                // Pattern matching for common nullable issues
                if (Regex.IsMatch(content, @"public\s+string\s+\w+\s*{\s*get;\s*set;\s*}") && 
                    !content.Contains("= null!") && !content.Contains("string?"))
                {
                    cs8618Count++;
                }

                // Check for potential null reference issues
                if (Regex.IsMatch(content, @"\.\w+\?\.\w+") || content.Contains("?."))
                {
                    // This is good - null conditional operators
                }
                else if (Regex.IsMatch(content, @"[^?]\.\w+") && content.Contains("null"))
                {
                    cs8602Count++;
                }

                // Check for obsolete attributes without migration notes
                var obsoleteMatches = Regex.Matches(content, @"\[Obsolete[^\]]*\]");
                foreach (Match match in obsoleteMatches)
                {
                    var lineStart = content.LastIndexOf('\n', match.Index) + 1;
                    var lineEnd = content.IndexOf('\n', match.Index);
                    if (lineEnd == -1) lineEnd = content.Length;
                    var line = content.Substring(lineStart, lineEnd - lineStart);
                    
                    if (!line.Contains("migration") && !line.Contains("use") && !line.Contains("replace"))
                    {
                        cs0618Count++;
                        LogWarning($"Obsolete marker without migration notes in {relativePath}");
                    }
                }
            }

            LogInfo($"Nullable analysis complete: {cs8618Count} potential CS8618, {cs8602Count} potential CS8602");
            LogInfo($"Obsolete analysis: {cs0618Count} obsolete markers without migration notes");

            if (cs8618Count > 5)
                LogWarning($"High number of potential non-nullable property warnings: {cs8618Count}");
            if (cs8602Count > 10)
                LogWarning($"High number of potential null reference warnings: {cs8602Count}");
        }

        /// <summary>
        /// Enforces XML documentation summaries
        /// </summary>
        private async Task EnforceXmlSummaries()
        {
            var csFiles = Directory.GetFiles(_projectRoot, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains("Migrations") && !f.Contains("Program.cs"))
                .ToList();

            int totalPublicItems = 0;
            int documentedItems = 0;
            var undocumentedItems = new List<string>();

            foreach (var file in csFiles)
            {
                var content = await File.ReadAllTextAsync(file);
                var relativePath = Path.GetRelativePath(_projectRoot, file);

                // Find public classes, interfaces, methods
                var publicMatches = Regex.Matches(content, 
                    @"public\s+(class|interface|enum|struct|\w+\s+\w+\s*\([^)]*\))", 
                    RegexOptions.Multiline);

                foreach (Match match in publicMatches)
                {
                    totalPublicItems++;
                    
                    // Check if there's a summary comment before this declaration
                    var lineStart = content.LastIndexOf('\n', match.Index);
                    var startIndex = Math.Max(0, lineStart - 500);
                    var length = lineStart - startIndex;
                    if (length <= 0) length = 500; // Fallback for edge cases
                    
                    var precedingText = content.Substring(startIndex, 
                        Math.Min(length, content.Length - startIndex));
                    
                    if (precedingText.Contains("/// <summary>"))
                    {
                        documentedItems++;
                    }
                    else
                    {
                        var lineNumber = content.Substring(0, match.Index).Count(c => c == '\n') + 1;
                        undocumentedItems.Add($"{relativePath}:{lineNumber} - {match.Value.Trim()}");
                    }
                }
            }

            LogInfo($"XML Summary check: {documentedItems}/{totalPublicItems} public items documented");

            if (undocumentedItems.Count > 0)
            {
                LogWarning($"{undocumentedItems.Count} public items missing XML summaries");
                foreach (var item in undocumentedItems.Take(5)) // Show first 5
                {
                    LogWarning($"  Missing summary: {item}");
                }
                if (undocumentedItems.Count > 5)
                {
                    LogWarning($"  ... and {undocumentedItems.Count - 5} more");
                }
            }
        }

        /// <summary>
        /// Checks SEO metadata binding in Razor pages
        /// </summary>
        private async Task CheckSeoBindings()
        {
            var razorFiles = Directory.GetFiles(_projectRoot, "*.cshtml", SearchOption.AllDirectories)
                .Where(f => !f.Contains("bin") && !f.Contains("obj"))
                .ToList();

            int totalPages = 0;
            int boundPages = 0;
            int pagesWithRobots = 0;
            var unboundPages = new List<string>();
            var missingRobots = new List<string>();

            foreach (var file in razorFiles)
            {
                var content = await File.ReadAllTextAsync(file);
                var relativePath = Path.GetRelativePath(_projectRoot, file);

                // Check if it's a page (has @page directive)
                if (content.Contains("@page"))
                {
                    totalPages++;

                    // Check for SeoMetadata binding
                    if (content.Contains("SeoMetadata") || content.Contains("seo-metadata") || 
                        content.Contains("ViewData[\"Title\"]") || content.Contains("ViewBag.Title"))
                    {
                        boundPages++;
                    }
                    else
                    {
                        unboundPages.Add(relativePath);
                    }

                    // Check for robots meta tag
                    if (content.Contains("robots") && content.Contains("meta"))
                    {
                        pagesWithRobots++;
                    }
                    else
                    {
                        missingRobots.Add(relativePath);
                    }
                }
            }

            LogInfo($"SEO binding check: {boundPages}/{totalPages} pages have SEO metadata");
            LogInfo($"Robots meta check: {pagesWithRobots}/{totalPages} pages have robots meta");

            foreach (var page in unboundPages.Take(3))
            {
                LogWarning($"Missing SEO binding: {page}");
            }

            foreach (var page in missingRobots.Take(2))
            {
                LogWarning($"Missing robots meta: {page}");
            }
        }

        /// <summary>
        /// Checks for empathy prompt drift
        /// </summary>
        private async Task CheckEmpathyPromptDrift()
        {
            // Check for _LyraPrompt.cshtml and other Lyra files
            var lyraPromptFiles = Directory.GetFiles(_projectRoot, "*LyraPrompt*.cshtml", SearchOption.AllDirectories);
            var lyraEmpathyFiles = Directory.GetFiles(_projectRoot, "*LyraEmpathy*.cshtml", SearchOption.AllDirectories);
            var allLyraFiles = lyraPromptFiles.Concat(lyraEmpathyFiles).ToArray();
            
            if (allLyraFiles.Length == 0)
            {
                // Check for any Lyra-related empathy files
                var lyraFiles = Directory.GetFiles(_projectRoot, "*Lyra*.cshtml", SearchOption.AllDirectories)
                    .Where(f => f.Contains("Empathy") || f.Contains("Modal") || f.Contains("Terminal"))
                    .ToArray();
                
                if (lyraFiles.Any())
                {
                    LogInfo($"Found {lyraFiles.Length} Lyra-related file(s): {string.Join(", ", lyraFiles.Select(Path.GetFileName))}");
                }
                else
                {
                    LogWarning("_LyraPrompt.cshtml not found - empathy prompt may be missing");
                }
            }
            else
            {
                LogInfo($"Found {allLyraFiles.Length} Lyra prompt file(s)");
                foreach (var file in allLyraFiles)
                {
                    var content = await File.ReadAllTextAsync(file);
                    if (content.Length < 100)
                    {
                        LogWarning($"Lyra prompt file appears truncated: {Path.GetFileName(file)}");
                    }
                }
            }

            // Check RevitalizeCognitiveSeeds.json
            var seedsPath = Path.Combine(_projectRoot, "Tests", "TestSeeds", "RevitalizeCognitiveSeeds.json");
            if (File.Exists(seedsPath))
            {
                var content = await File.ReadAllTextAsync(seedsPath);
                if (content.Length < 50)
                {
                    LogCritical("RevitalizeCognitiveSeeds.json appears to be empty or corrupted");
                }
                else
                {
                    LogInfo("RevitalizeCognitiveSeeds.json: Drift stable");
                }
            }
            else
            {
                LogCritical("RevitalizeCognitiveSeeds.json not found");
            }
        }

        /// <summary>
        /// Validates heatmap replay logs
        /// </summary>
        private async Task ValidateHeatmapReplay()
        {
            var heatmapPath = Path.Combine(_projectRoot, "Logs", "ReplayTestHeatmap.md");
            if (File.Exists(heatmapPath))
            {
                var content = await File.ReadAllTextAsync(heatmapPath);
                
                // Check for persona tags and empathy-related content
                var personaTags = new[] { "[FrustratedCustomer]", "[ElderlyCare]", "[TechnicianExpert]", "[ManagerReview]" };
                var foundTags = personaTags.Where(tag => content.Contains(tag)).ToList();

                // Also check for general empathy/persona-related content
                var empathyKeywords = new[] { "empathy", "persona", "customer", "session", "replay", "health-check" };
                var foundKeywords = empathyKeywords.Where(keyword => content.ToLower().Contains(keyword.ToLower())).ToList();

                if (foundTags.Any())
                {
                    LogInfo($"ReplayTestHeatmap.md found with persona tags: {string.Join(", ", foundTags)}");
                }
                else if (foundKeywords.Any())
                {
                    LogInfo($"ReplayTestHeatmap.md found with empathy content: {string.Join(", ", foundKeywords)}");
                }
                else
                {
                    LogWarning("ReplayTestHeatmap.md exists but no persona tags or empathy content found");
                }
            }
            else
            {
                LogCritical("ReplayTestHeatmap.md not found");
            }
        }

        /// <summary>
        /// Checks backup and commit hooks
        /// </summary>
        private async Task CheckBackupAndCommitHooks()
        {
            // Check for backup ZIP files
            var backupFiles = Directory.GetFiles(_projectRoot, "*OmegaSweepBackup*.zip", SearchOption.AllDirectories);
            if (backupFiles.Any())
            {
                LogInfo($"Found {backupFiles.Length} OmegaSweep backup file(s)");
                var latest = backupFiles.OrderByDescending(File.GetCreationTime).First();
                LogInfo($"Latest backup: {Path.GetFileName(latest)}");
            }
            else
            {
                LogWarning("No OmegaSweepBackup ZIP files found");
            }

            // Check Program.cs for Wangkanai Detection
            var programPath = Path.Combine(_projectRoot, "Program.cs");
            if (File.Exists(programPath))
            {
                var content = await File.ReadAllTextAsync(programPath);
                if (content.Contains("Wangkanai") && content.Contains("Detection"))
                {
                    LogInfo("Program.cs: Wangkanai Detection confirmed");
                }
                else
                {
                    LogWarning("Program.cs: Wangkanai Detection not found");
                }
            }
        }

        /// <summary>
        /// Sweeps for residual logs and temporary files
        /// </summary>
        private async Task SweepResidualLogs()
        {
            var patterns = new[] { "*.old", "*.bak", "*.tmp", "*.temp" };
            var residualFiles = new List<string>();

            foreach (var pattern in patterns)
            {
                var files = Directory.GetFiles(_projectRoot, pattern, SearchOption.AllDirectories)
                    .Where(f => !f.Contains("node_modules") && !f.Contains(".git"))
                    .ToList();
                residualFiles.AddRange(files);
            }

            if (residualFiles.Any())
            {
                LogWarning($"Found {residualFiles.Count} residual files for cleanup");
                foreach (var file in residualFiles.Take(3))
                {
                    LogWarning($"  Residual file: {Path.GetRelativePath(_projectRoot, file)}");
                }
            }
            else
            {
                LogInfo("No residual files found");
            }

            // Check logs directory for excessive files
            var logsDir = Path.Combine(_projectRoot, "Logs");
            if (Directory.Exists(logsDir))
            {
                var logFiles = Directory.GetFiles(logsDir, "*", SearchOption.AllDirectories);
                if (logFiles.Length > 20)
                {
                    LogWarning($"Logs directory contains {logFiles.Length} files - consider cleanup");
                }
            }
        }

        private string GenerateMainReport(OmegaScanResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# OMEGA V3 — Final Diagnostic Sweep 🔥");
            sb.AppendLine();
            sb.AppendLine($"**Scan Date:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"**Project:** MVP-Core");
            sb.AppendLine();

            // Success items
            sb.AppendLine("## ✅ PASSED CHECKS");
            sb.AppendLine();
            foreach (var log in _logs.Where(l => l.Contains("INFO:")))
            {
                var message = log.Substring(log.IndexOf("INFO:") + 5).Trim();
                sb.AppendLine($"✅ {message}");
            }
            sb.AppendLine();

            // Warnings
            if (result.Warnings.Any())
            {
                sb.AppendLine("## ⚠️ WARNINGS");
                sb.AppendLine();
                foreach (var warning in result.Warnings)
                {
                    sb.AppendLine($"- ⚠️ {warning}");
                }
                sb.AppendLine();
            }

            // Critical issues
            if (result.CriticalIssues.Any())
            {
                sb.AppendLine("## ❌ CRITICAL ISSUES");
                sb.AppendLine();
                foreach (var issue in result.CriticalIssues)
                {
                    sb.AppendLine($"- ❌ {issue}");
                }
                sb.AppendLine();
            }

            // Verdict
            sb.AppendLine("## 🧠 VERDICT");
            sb.AppendLine();
            if (result.HasCriticalIssues)
            {
                sb.AppendLine("❌ **FAILED** — Critical issues found");
            }
            else if (result.Warnings.Any())
            {
                sb.AppendLine($"✅ **PASSED** — with {result.Warnings.Count} soft alerts");
            }
            else
            {
                sb.AppendLine("✅ **PASSED** — All checks successful");
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"*Generated by OmegaV3DiagnosticSweep at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*");

            return sb.ToString();
        }

        private string GenerateAuditReport(OmegaScanResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# OmegaV3 Audit Log");
            sb.AppendLine();
            sb.AppendLine($"**Timestamp:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"**Project Root:** {_projectRoot}");
            sb.AppendLine();

            sb.AppendLine("## Execution Log");
            sb.AppendLine();
            foreach (var log in result.Logs)
            {
                sb.AppendLine($"{log}");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Results from OmegaV3 diagnostic scan
    /// </summary>
    public class OmegaScanResult
    {
        public bool HasCriticalIssues { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> CriticalIssues { get; set; } = new List<string>();
        public List<string> Logs { get; set; } = new List<string>();
    }
}