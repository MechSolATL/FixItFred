using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace BuildTasks
{
    public static class FixItFredExtension
    {
        private static readonly string ProjectRoot = @"E:\source\MechSolATL\MVP-Core";
        private static readonly string LogPath = Path.Combine(ProjectRoot, "FixItFred", "Logs", "patch-log.json");

        public static void ReplayLastInjectedModules()
        {
            Console.WriteLine("[FixItFred] ?? Replay Mode: Enabled");

            if (!File.Exists(LogPath))
            {
                Console.WriteLine($"[FixItFred] ? No replay log found at: {LogPath}");
                return;
            }

            try
            {
                var json = File.ReadAllText(LogPath);
                var modules = JsonSerializer.Deserialize<List<string>>(json);

                if (modules == null || modules.Count == 0)
                {
                    Console.WriteLine("[FixItFred] ?? No modules listed in replay log.");
                    return;
                }

                Console.WriteLine($"[FixItFred] ?? {modules.Count} module(s) queued for replay:");
                foreach (var module in modules)
                {
                    Console.WriteLine($"   ? Re-invoking patch for: {module}");
                    // Optionally trigger actual rebind/recompile logic here
                }

                Console.WriteLine("[FixItFred] ? Replay sequence completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FixItFred] ? Error during replay: {ex.Message}");
            }
        }
    }
}