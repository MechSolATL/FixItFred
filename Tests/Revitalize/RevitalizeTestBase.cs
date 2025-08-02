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
/// [Sprint123_FixItFred_OmegaSweep] Enhanced test base class with comprehensive DI and empathy testing framework
/// Provides complete testing infrastructure for Revitalize module including empathy data and mocks
/// UX Connection: Supports testing of all customer-facing functionality and workflows
/// Model Connection: Creates in-memory database with full Revitalize entity framework
/// CLI Flag: Supports testing of RevitalizeCLI operations and cognitive seed processing
/// Expected Outcome: Enables comprehensive testing of empathy features and service operations
/// Cognitive Impact: Validates Nova AI integration and LyraEmpathyIntakeNarrator functionality
/// Empathy Replay Impact: Tests persona-based response generation and customer satisfaction patterns
/// </summary>
public abstract class RevitalizeTestBase
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Creates a comprehensive test service provider with all dependencies
    /// Sets up complete DI container with in-memory database, configuration, and mock services
    /// UX Connection: Enables testing of complete user workflows from UI to data layer
    /// Expected Outcome: Fully configured service provider for integration testing
    /// Cognitive Impact: Includes Nova AI mock services for testing optimization features
    /// Side Effects: Creates isolated test environment with empathy data seeding
    /// </summary>
    /// <returns>Configured ServiceProvider for testing with proper disposal pattern</returns>
    protected ServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();

        // Add logging for test diagnostics
        services.AddLogging();

        // Add in-memory database for isolated testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        // Enhanced configuration with complete feature flags and empathy settings
        var configurationData = new Dictionary<string, string?>
        {
            // Revitalize platform configuration
            ["Revitalize:PlatformName"] = "Test Revitalize Platform",
            ["Revitalize:Version"] = "1.0.0-Test",
            ["Revitalize:SaaSMode"] = "true",
            ["Revitalize:MaxTenants"] = "50",
            ["Revitalize:EnableDebugReplay"] = "true",
            ["Revitalize:DefaultTheme"] = "test-theme",
            
            // Lyra empathy configuration
            ["Lyra:PromptMode"] = "Expanded",
            ["Lyra:EnableEmpathy"] = "true",
            ["Lyra:ResponseTimeout"] = "5000",
            ["Lyra:PersonaTraining"] = "true",
            
            // FixItFred diagnostics configuration
            ["FixItFred:DiagnosticsEnabled"] = "true",
            ["FixItFred:HealthCheckInterval"] = "30",
            ["FixItFred:LogLevel"] = "Debug",
            
            // Comprehensive feature flags for testing
            ["Features:MultiTenant"] = "true",
            ["Features:TechnicianTracking"] = "true",
            ["Features:CustomerPortal"] = "true",
            ["Features:Analytics"] = "true",
            ["Features:EmpathyMode"] = "true",
            ["Features:NovaAI"] = "true",
            ["Features:ReplayTranscriptStore"] = "true"
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Register Revitalize services for testing
        RegisterRevitalizeServices(services);

        // Register comprehensive mock services for empathy testing
        services.AddScoped<ILyraCognition, LyraCognitionMock>();
        services.AddScoped<IFixItFredCLI, FixItFredCLIMock>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Seeds comprehensive test data for empathy and Revitalize testing
    /// Populates in-memory database with empathy prompts, cognitive scenarios, and sample service data
    /// UX Connection: Creates realistic test data for testing complete user workflows
    /// Model Connection: Seeds all Revitalize entities with relationships for integration testing
    /// Expected Outcome: Fully populated test database ready for comprehensive testing scenarios
    /// Empathy Impact: Includes persona-based test data for validating empathy response accuracy
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving database context and dependencies</param>
    protected void SeedTestData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created and schema is applied
        context.Database.EnsureCreated();
        
        // Seed comprehensive empathy test data
        TestDataSeeder.SeedTestData(context);
        
        // Seed Revitalize-specific test data
        TestDataSeeder.SeedRevitalizeTestData(context);
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Registers Revitalize services for testing scenarios
    /// Override in derived test classes to register specific service dependencies
    /// UX Connection: Enables testing of complete service layer functionality
    /// Expected Outcome: Properly configured DI container for service testing
    /// Purpose: Provides extensibility point for test-specific service registration
    /// </summary>
    /// <param name="services">Service collection for dependency registration</param>
    protected virtual void RegisterRevitalizeServices(IServiceCollection services)
    {
        // Base implementation provides framework for derived classes
        // Specific test classes override this to register their required services
        // This pattern avoids dependencies on services that may not exist in all test scenarios
    }
}