using System;
using System.IO;

namespace BuildTasks
{
    public static class FixItFredExtension
    {
        public static void PrintBuildInfo()
        {
            string projectRoot = @"E:\source\MechSolATL\MVP-Core";
            Console.WriteLine($"[FixItFred] Build started for project at: {projectRoot}");

            string[] monitoredPaths =
            {
                @"E:\source\MechSolATL\MVP-Core\Services",
                @"E:\source\MechSolATL\MVP-Core\Pages\Admin",
                @"E:\source\MechSolATL\MVP-Core\Data\Models"
            };

            foreach (var path in monitoredPaths)
            {
                if (Directory.Exists(path))
                {
                    int fileCount = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Length;
                    Console.WriteLine($"[FixItFred] Detected {fileCount} .cs files in {path}");
                }
                else
                {
                    Console.WriteLine($"[FixItFred] Warning: Path not found: {path}");
                }
            }

            Console.WriteLine($"[FixItFred] Timestamp: {DateTime.Now:u}");
        }
    }
}
