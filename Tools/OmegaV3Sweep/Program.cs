using System;
using System.IO;
using System.Threading.Tasks;

namespace OmegaV3Sweep
{
    /// <summary>
    /// OMEGA V3 — Final Diagnostic Sweep
    /// A comprehensive backend enforcer tool for post-patch inspection
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point for OmegaV3 Diagnostic Sweep
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code: 0 for success, 1 for failures found</returns>
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("🔥 OMEGA V3 — FINAL DIAGNOSTIC SCAN INITIATED");
                Console.WriteLine("============================================");
                Console.WriteLine();

                var projectRoot = GetProjectRoot();
                if (string.IsNullOrEmpty(projectRoot))
                {
                    Console.WriteLine("❌ ERROR: Could not find MVP-Core project root");
                    return 1;
                }

                Console.WriteLine($"📂 Project Root: {projectRoot}");
                Console.WriteLine($"🕐 Scan Start: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                Console.WriteLine();

                var scanner = new OmegaV3Scanner(projectRoot);
                var result = await scanner.ExecuteFullScanAsync();

                // Generate report
                await scanner.GenerateReportAsync(result);

                Console.WriteLine();
                Console.WriteLine("🔥 OMEGA V3 DIAGNOSTIC SCAN COMPLETE");
                Console.WriteLine($"📊 Report: {Path.Combine(projectRoot, "Logs", "OmegaV3_FinalScan_Report.md")}");

                return result.HasCriticalIssues ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 1;
            }
        }

        /// <summary>
        /// Finds the MVP-Core project root directory
        /// </summary>
        /// <returns>Path to project root or null if not found</returns>
        private static string? GetProjectRoot()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dir = new DirectoryInfo(currentDir);

            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "MVP-Core.csproj")))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }

            return null;
        }
    }
}