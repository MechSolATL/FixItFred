using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Xunit;

namespace Tests.Revitalize;

/// <summary>
/// Tests for FixItFred diagnostic coverage using DI-enabled test framework
/// Sprint121: Tactical add-on for FixItFred CLI testing
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
}