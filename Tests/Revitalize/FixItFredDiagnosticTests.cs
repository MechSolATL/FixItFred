using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Xunit;

namespace Tests.Revitalize;

/// <summary>
/// Tests for FixItFred diagnostic coverage using DI-enabled test framework
/// Sprint121+122: Tactical add-on for FixItFred CLI testing with enhanced simulation
/// </summary>
[Trait("Category", "FixItFred")]
[Trait("Layer", "Diagnostic")]
public class FixItFredDiagnosticTests : RevitalizeTestBase
{
    /// <summary>
    /// Test that FixItFred CLI can be resolved via DI (even if it's empty for now)
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public void FixItFredCLI_Should_Resolve()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var fred = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
        Assert.NotNull(fred);
    }

    /// <summary>
    /// Test FixItFred diagnostic execution
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    public async Task Should_Execute_FixItFred_Diagnostics()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var fred = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
        
        var result = await fred.RunDiagnosticsAsync();
        Assert.NotNull(result);
        Assert.Contains("diagnostics", result.ToLower());
    }

    /// <summary>
    /// Test FixItFred health check functionality
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public async Task Should_Check_FixItFred_Health_Status()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var fred = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
        
        var isHealthy = await fred.IsHealthyAsync();
        Assert.True(isHealthy); // Mock should return healthy by default
    }

    /// <summary>
    /// Test that all diagnostic services are registered in DI container
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public void Should_Resolve_All_Diagnostic_Services_From_DI()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        // Verify FixItFred CLI service can be resolved
        var fredCLI = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
        Assert.NotNull(fredCLI);
        
        // Future: Add other diagnostic services as they are implemented
        // var otherDiagnostic = scope.ServiceProvider.GetRequiredService<IOtherDiagnostic>();
    }

    /// <summary>
    /// Test mock CLI patch logic simulation
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("PatchType", "MockSimulation")]
    public async Task Should_Simulate_CLI_Patch_Logic()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var fred = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
        
        // Cast to mock to access simulation features
        if (fred is Tests.Mocks.FixItFredCLIMock mockFred)
        {
            // Test normal patch scenario
            mockFred.SetDiagnosticResult("Patch applied successfully - System optimization complete");
            var result = await fred.RunDiagnosticsAsync();
            Assert.Contains("patch applied successfully", result.ToLower());
            
            // Test patch failure scenario
            mockFred.SetDiagnosticResult("Patch failed - System requires manual intervention");
            var failureResult = await fred.RunDiagnosticsAsync();
            Assert.Contains("patch failed", failureResult.ToLower());
        }
    }

    /// <summary>
    /// Test health check edge cases with configurable status
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("HealthCheck", "EdgeCases")]
    public async Task Should_Handle_Health_Check_Edge_Cases()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var fred = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
        
        // Cast to mock to access configuration features
        if (fred is Tests.Mocks.FixItFredCLIMock mockFred)
        {
            // Test healthy status
            mockFred.SetHealthStatus(true);
            var healthyResult = await fred.IsHealthyAsync();
            Assert.True(healthyResult);
            
            // Test unhealthy status
            mockFred.SetHealthStatus(false);
            var unhealthyResult = await fred.IsHealthyAsync();
            Assert.False(unhealthyResult);
        }
    }

    /// <summary>
    /// Test diagnostic execution with various system states
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    [Trait("SystemState", "Various")]
    public async Task Should_Execute_Diagnostics_Under_Various_System_States()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var fred = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
        
        // Cast to mock for state simulation
        if (fred is Tests.Mocks.FixItFredCLIMock mockFred)
        {
            var systemStates = new[]
            {
                "System operational - All services running",
                "System degraded - Some services experiencing issues", 
                "System critical - Multiple service failures detected",
                "System maintenance - Scheduled maintenance in progress"
            };
            
            foreach (var state in systemStates)
            {
                mockFred.SetDiagnosticResult(state);
                var result = await fred.RunDiagnosticsAsync();
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Contains(state.Split('-')[0].Trim().ToLower(), result.ToLower());
            }
        }
    }
}