namespace Diagnostics.FixItFredDiagnostics
{
    public static class StartupLogger
    {
        public static void Log(WebApplicationBuilder builder)
        {
            Console.WriteLine("📦 [FixItFred] Environment: " + builder.Environment.EnvironmentName);
            Console.WriteLine("🔧 [FixItFred] Registered Services:");
            foreach (var svc in builder.Services)
            {
                Console.WriteLine($"   - {svc.ServiceType?.FullName}");
            }
        }
    }
}
