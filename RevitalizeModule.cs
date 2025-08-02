using Microsoft.Extensions.DependencyInjection;

namespace Revitalize
{
    /// <summary>
    /// Revitalize module for service registration and configuration
    /// [Sprint123_FixItFred_OmegaSweep] Created RevitalizeModule to centralize Revitalize service bindings
    /// </summary>
    public static class RevitalizeModule
    {
        /// <summary>
        /// Registers all Revitalize module services including Empathy, CLI, and Replay services
        /// [Sprint123_FixItFred_OmegaSweep] Consolidated service registration for better DI management
        /// </summary>
        /// <param name="services">The service collection to register services with</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddRevitalizeServices(this IServiceCollection services)
        {
            // Core Revitalize services
            services.AddScoped<Services.ITenantService, Services.TenantService>();
            services.AddScoped<Services.IServiceRequestService, Services.ServiceRequestService>();
            services.AddScoped<Services.IRevitalizeConfigService, Services.RevitalizeConfigService>();
            services.AddScoped<Services.IRevitalizeSeoService, Services.RevitalizeSeoService>();
            
            // Nova planning services  
            services.AddScoped<Services.Nova.INovaRevitalizePlanningService, Services.Nova.NovaRevitalizePlanningService>();
            
            // CLI and Replay engine services
            services.AddScoped<Services.RevitalizeReplayCLI>();
            
            return services;
        }

        /// <summary>
        /// Configures Revitalize module with additional options
        /// [Sprint123_FixItFred_OmegaSweep] Added configuration method for module setup
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Configuration options</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection ConfigureRevitalize(this IServiceCollection services, Action<RevitalizeOptions>? configuration = null)
        {
            var options = new RevitalizeOptions();
            configuration?.Invoke(options);
            
            services.AddSingleton(options);
            
            return services;
        }
    }

    /// <summary>
    /// Configuration options for Revitalize module
    /// [Sprint123_FixItFred_OmegaSweep] Added options class for module configuration
    /// </summary>
    public class RevitalizeOptions
    {
        public bool EnableEmpathyService { get; set; } = true;
        public bool EnableCLIService { get; set; } = true;
        public bool EnableReplayService { get; set; } = true;
    }
}