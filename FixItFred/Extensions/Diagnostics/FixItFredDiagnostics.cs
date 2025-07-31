using System;
using Microsoft.AspNetCore.Builder;

namespace FixItFred.Extensions.Diagnostics
{
    public static class StartupLogger
    {
        public static void Log(WebApplicationBuilder builder)
        {
            var env = builder.Environment.EnvironmentName;
            var services = builder.Services;
            Console.WriteLine($"[FixItFredDiagnostics] :: Environment = {env}");

            // Optional: Add more scoped logging below
            Console.WriteLine($"[FixItFredDiagnostics] :: Registered Services Count = {services.Count}");
        }
    }
}
