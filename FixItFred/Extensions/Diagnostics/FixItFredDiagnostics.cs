// [Sprint1002_FixItFred] Created FixItFredDiagnostics implementation to resolve Program.cs compilation error
// This provides startup logging for build environment and DI inspection
// Sprint1002: Add minimal diagnostics implementation

namespace FixItFred.Extensions.Diagnostics
{
    public static class FixItFredDiagnostics
    {
        public static class StartupLogger
        {
            /// <summary>
            /// Logs startup diagnostics information
            /// </summary>
            /// <param name="builder">The web application builder</param>
            public static void Log(WebApplicationBuilder builder)
            {
                if (builder?.Environment != null)
                {
                    Console.WriteLine($"[FixItFred] Environment: {builder.Environment.EnvironmentName}");
                    Console.WriteLine($"[FixItFred] Application: {builder.Environment.ApplicationName}");
                    Console.WriteLine($"[FixItFred] Content Root: {builder.Environment.ContentRootPath}");
                }
            }
        }
    }
}
