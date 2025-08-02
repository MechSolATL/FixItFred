using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Data;
using Interfaces;
using Tests.Mocks;
using Tests.TestSeeds;

namespace Tests.Revitalize;

/// <summary>
/// Enhanced test base class with all tactical add-ons for DI-enabled testing
/// Sprint121: Complete test framework with empathy data, mocks, and configuration
/// </summary>
public abstract class RevitalizeTestBase
{
    /// <summary>
    /// [Sprint123_FixItFred] Fixed return type to ServiceProvider to enable using statement disposal
    /// Creates a test service provider with all necessary dependencies for Revitalize testing
    /// </summary>
    protected ServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging();

        // Add in-memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        // Enhanced configuration with feature flags and Lyra tuning options
        var configurationData = new Dictionary<string, string?>
        {
            // Revitalize configuration
            ["Revitalize:PlatformName"] = "Test Revitalize Platform",
            ["Revitalize:Version"] = "1.0.0-Test",
            ["Revitalize:SaaSMode"] = "true",
            ["Revitalize:MaxTenants"] = "50",
            ["Revitalize:EnableDebugReplay"] = "true",
            
            // Lyra configuration
            ["Lyra:PromptMode"] = "Expanded",
            ["Lyra:EnableEmpathy"] = "true",
            ["Lyra:ResponseTimeout"] = "5000",
            
            // FixItFred configuration
            ["FixItFred:DiagnosticsEnabled"] = "true",
            ["FixItFred:HealthCheckInterval"] = "30",
            
            // Feature flags
            ["Features:MultiTenant"] = "true",
            ["Features:TechnicianTracking"] = "true",
            ["Features:CustomerPortal"] = "true",
            ["Features:Analytics"] = "true",
            ["Features:EmpathyMode"] = "true"
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Register Revitalize services (if interfaces exist)
        RegisterRevitalizeServices(services);

        // Register mock services for testing
        services.AddScoped<ILyraCognition, LyraCognitionMock>();
        services.AddScoped<IFixItFredCLI, FixItFredCLIMock>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Seeds test data into the database context
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving dependencies</param>
    protected void SeedTestData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Seed empathy test data
        TestDataSeeder.SeedTestData(context);
        
        // Seed Revitalize test data
        TestDataSeeder.SeedRevitalizeTestData(context);
    }

    /// <summary>
    /// Registers Revitalize services if their interfaces exist
    /// Override in derived classes to add specific service registrations
    /// </summary>
    /// <param name="services">Service collection</param>
    protected virtual void RegisterRevitalizeServices(IServiceCollection services)
    {
        // This method can be overridden by specific test classes to register
        // their required services. Base implementation is empty to avoid
        // dependencies on services that may not exist yet.
    }
}