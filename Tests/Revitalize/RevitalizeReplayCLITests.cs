using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Interfaces;
using Revitalize.Services;
using Xunit;

namespace Tests.Revitalize;

/// <summary>
/// Tests for RevitalizeReplayCLI with empathy prompt streaming and persona annotation
/// Sprint121+122: CLI integration with cognitive seeds and persona traits
/// </summary>
[Trait("Category", "Revitalize")]
[Trait("Layer", "CLI")]
public class RevitalizeReplayCLITests : RevitalizeTestBase
{
    /// <summary>
    /// Test CLI empathy prompt streaming with persona annotations
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("Persona", "AnxiousCustomer")]
    public async Task Should_Stream_Empathy_Prompt_With_AnxiousCustomer_Annotation()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RevitalizeReplayCLI>>();
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        var cli = new RevitalizeReplayCLI(configuration, logger, lyraCognition);
        
        var result = await cli.StreamEmpathyPromptAsync("AnxiousCustomer", "service failure");
        
        Assert.NotNull(result);
        Assert.Contains("Persona: AnxiousCustomer", result);
        Assert.Contains("extra reassurance", result);
        Assert.Contains("sorry", result.ToLower());
    }

    /// <summary>
    /// Test CLI empathy prompt streaming with FrustratedCustomer persona
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("Persona", "FrustratedCustomer")]
    public async Task Should_Stream_Empathy_Prompt_With_FrustratedCustomer_Annotation()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RevitalizeReplayCLI>>();
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        var cli = new RevitalizeReplayCLI(configuration, logger, lyraCognition);
        
        var result = await cli.StreamEmpathyPromptAsync("FrustratedCustomer", "billing issue");
        
        Assert.NotNull(result);
        Assert.Contains("Persona: FrustratedCustomer", result);
        Assert.Contains("acknowledgment of frustration", result);
        Assert.Contains("understand", result.ToLower());
    }

    /// <summary>
    /// Test CLI empathy prompt streaming with TechnicallySavvy persona
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("Persona", "TechnicallySavvy")]
    public async Task Should_Stream_Empathy_Prompt_With_TechnicallySavvy_Annotation()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RevitalizeReplayCLI>>();
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        var cli = new RevitalizeReplayCLI(configuration, logger, lyraCognition);
        
        var result = await cli.StreamEmpathyPromptAsync("TechnicallySavvy", "scheduling conflict");
        
        Assert.NotNull(result);
        Assert.Contains("Persona: TechnicallySavvy", result);
        Assert.Contains("detailed technical explanations", result);
    }

    /// <summary>
    /// Test processing of cognitive seeds JSON file
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    public async Task Should_Process_Cognitive_Seeds_From_JSON()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RevitalizeReplayCLI>>();
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        var cli = new RevitalizeReplayCLI(configuration, logger, lyraCognition);
        
        // Use the cognitive seeds file in the TestSeeds folder
        var cognitiveJsonPath = "/home/runner/work/MVP-Core/MVP-Core/Tests/TestSeeds/RevitalizeCognitiveSeeds.json";
        
        var scenarios = await cli.ProcessCognitiveSeedsAsync(cognitiveJsonPath);
        
        Assert.NotNull(scenarios);
        Assert.NotEmpty(scenarios);
        Assert.True(scenarios.Count >= 3); // Should have multiple scenarios
        
        // Verify persona annotations are present
        var anxiousScenarios = scenarios.Where(s => s.Persona == "AnxiousCustomer").ToList();
        Assert.NotEmpty(anxiousScenarios);
        
        var frustratedScenarios = scenarios.Where(s => s.Persona == "FrustratedCustomer").ToList();
        Assert.NotEmpty(frustratedScenarios);
        
        var techSavvyScenarios = scenarios.Where(s => s.Persona == "TechnicallySavvy").ToList();
        Assert.NotEmpty(techSavvyScenarios);
    }

    /// <summary>
    /// Test debug replay information retrieval
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public void Should_Get_Debug_Replay_Information()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RevitalizeReplayCLI>>();
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        var cli = new RevitalizeReplayCLI(configuration, logger, lyraCognition);
        
        var debugInfo = cli.GetDebugReplayInfo();
        
        Assert.NotNull(debugInfo);
        Assert.Contains("Debug Replay Enabled", debugInfo);
        Assert.Contains("Test Revitalize Platform", debugInfo);
        Assert.Contains("Empathy Mode", debugInfo);
    }

    /// <summary>
    /// Test error handling when cognitive seeds file doesn't exist
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public async Task Should_Handle_Missing_Cognitive_Seeds_File()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RevitalizeReplayCLI>>();
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        var cli = new RevitalizeReplayCLI(configuration, logger, lyraCognition);
        
        var scenarios = await cli.ProcessCognitiveSeedsAsync("/nonexistent/path.json");
        
        Assert.NotNull(scenarios);
        Assert.Empty(scenarios);
    }
}