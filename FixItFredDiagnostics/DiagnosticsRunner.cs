using System;
using System.Collections.Generic;
using System.Reflection;

namespace MVP_Core.Services.Diagnostics
{
    /// <summary>
    /// Entry point for executing all registered diagnostic modules via IServiceModule.
    /// </summary>
    public static class DiagnosticsRunner
    {
        public static void RunAllDiagnostics(IServiceProvider serviceProvider)
        {
            Console.WriteLine("🔍 Running diagnostics for all IServiceModules...");
            var scanner = new ServiceModuleScanner();
            var modules = scanner.ScanForModules();

            foreach (var module in modules)
            {
                Console.WriteLine($"🛠️ Diagnosing: {module.FullName}");
                var instance = Activator.CreateInstance(module);
                var runMethod = module.GetMethod("RunDiagnostics", BindingFlags.Public | BindingFlags.Instance);

                if (runMethod != null)
                {
                    runMethod.Invoke(instance, new object[] { serviceProvider });
                }
                else
                {
                    Console.WriteLine($"⚠️ Skipped: {module.FullName} has no 'RunDiagnostics' method.");
                }
            }

            Console.WriteLine("✅ Diagnostics complete.");
        }
    }
}