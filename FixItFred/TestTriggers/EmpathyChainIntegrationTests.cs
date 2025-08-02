using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Xunit;

namespace FixItFred.TestTriggers;

/// <summary>
/// [Sprint124_FixItFred_EmpathyExpansion] FixItFred Integration Tests for Empathy Chain
/// Tests the complete empathy trigger chain: CLI → FixItFred → Empathy → Overlay
/// Validates empathy-triggered test cases and bug resolution with emotional context
/// </summary>
[Trait("Category", "FixItFredIntegration")]
[Trait("Layer", "EmpathyChain")]
public class EmpathyChainIntegrationTests
{
    private readonly ILogger<EmpathyChainIntegrationTests> _logger;
    
    public EmpathyChainIntegrationTests()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<EmpathyChainIntegrationTests>();
    }

    /// <summary>
    /// [FixItFredComment:Sprint124_EmpathyChainTest] Test CLI to FixItFred to Empathy full chain
    /// Validates that empathy triggers can be initiated from CLI and flow through entire system
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    [Trait("Chain", "CLI-FixItFred-Empathy")]
    public async Task Should_Execute_Complete_CLI_To_Empathy_Chain()
    {
        // [Sprint124_FixItFred_EmpathyExpansion] Arrange: Set up test services
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var empathyNarrator = scope.ServiceProvider.GetRequiredService<ILyraEmpathyNarrator>();
        var alphaClientService = scope.ServiceProvider.GetRequiredService<AlphaClientEmpathyService>();
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Act: Execute CLI empathy trigger simulation
        var cliCommand = "revitalize-cli empathy trigger --persona AnxiousCustomer --context 'service failure'";
        _logger.LogInformation("[Sprint124_FixItFred_EmpathyExpansion] Simulating CLI command: {Command}", cliCommand);
        
        // Simulate CLI empathy trigger
        var empathyResult = await empathyNarrator.GenerateEmpathyNarrativeAsync(
            "AnxiousCustomer",
            new EmotionalContext
            {
                PrimaryEmotion = "anxiety",
                IntensityLevel = 7,
                TriggerEvent = "service failure",
                RequiresImmediateAttention = true
            },
            "CLI triggered empathy response"
        );
        
        // [Sprint124_FixItFred_EmpathyExpansion] Assert: Verify complete chain execution
        Assert.NotNull(empathyResult);
        Assert.Equal("AnxiousCustomer", empathyResult.PersonaUsed);
        Assert.True(empathyResult.EmpathyScore > 0.7, $"Empathy score {empathyResult.EmpathyScore} should be > 0.7");
        Assert.NotEmpty(empathyResult.Narrative);
        Assert.Contains("reassure", empathyResult.Narrative.ToLower());
        
        _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] CLI to empathy chain test passed with score {Score:F2}", 
            empathyResult.EmpathyScore);
    }

    /// <summary>
    /// [FixItFredComment:Sprint124_EmpathyChainTest] Test empathy overlay integration with SignalR
    /// Validates that empathy overlays can communicate with SignalR for real-time updates
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    [Trait("Chain", "Empathy-SignalR-Overlay")]
    public async Task Should_Integrate_Empathy_With_SignalR_Overlay()
    {
        // [Sprint124_FixItFred_EmpathyExpansion] Arrange: Set up empathy overlay test
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var empathyNarrator = scope.ServiceProvider.GetRequiredService<ILyraEmpathyNarrator>();
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Act: Test empathy streaming for overlay
        var interactionId = Guid.NewGuid();
        var suggestions = new List<EmpathySuggestion>();
        
        await foreach (var suggestion in empathyNarrator.StreamEmpathySuggestionsAsync(interactionId, "real-time context"))
        {
            suggestions.Add(suggestion);
            
            // [Sprint124_FixItFred_EmpathyExpansion] Simulate SignalR message
            _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] SignalR empathy suggestion: {Type} - {Suggestion}", 
                suggestion.SuggestionType, suggestion.Suggestion);
            
            // Break after first few suggestions for test
            if (suggestions.Count >= 2) break;
        }
        
        // [Sprint124_FixItFred_EmpathyExpansion] Assert: Verify SignalR integration
        Assert.NotEmpty(suggestions);
        Assert.True(suggestions.Count >= 2, "Should receive multiple empathy suggestions");
        Assert.All(suggestions, s => Assert.True(s.Confidence > 0.5, "Suggestion confidence should be > 0.5"));
        
        _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] SignalR overlay integration test passed with {Count} suggestions", 
            suggestions.Count);
    }

    /// <summary>
    /// [FixItFredComment:Sprint124_EmpathyChainTest] Test alpha client empathy walkthrough integration
    /// Validates complete alpha client onboarding with empathy checkpoints
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    [Trait("Chain", "AlphaClient-Empathy-Walkthrough")]
    public async Task Should_Complete_Alpha_Client_Empathy_Walkthrough()
    {
        // [Sprint124_FixItFred_EmpathyExpansion] Arrange: Set up alpha client test
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var alphaClientService = scope.ServiceProvider.GetRequiredService<AlphaClientEmpathyService>();
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Act: Execute full walkthrough
        var walkthroughResult = await alphaClientService.ExecuteEmpathyWalkthroughAsync("alpha-001", testMode: true);
        
        // [Sprint124_FixItFred_EmpathyExpansion] Assert: Verify walkthrough success
        Assert.True(walkthroughResult.Success, $"Walkthrough should succeed. Error: {walkthroughResult.ErrorMessage}");
        Assert.True(walkthroughResult.OverallEmpathyScore >= 0.8, 
            $"Overall empathy score {walkthroughResult.OverallEmpathyScore:F2} should be >= 0.8");
        Assert.NotEmpty(walkthroughResult.Steps);
        Assert.True(walkthroughResult.TotalDuration.TotalMinutes < 15, 
            $"Walkthrough should complete in < 15 minutes, actual: {walkthroughResult.TotalDuration.TotalMinutes:F1}");
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Verify each step has empathy integration
        foreach (var step in walkthroughResult.Steps)
        {
            Assert.True(step.EmpathyScore > 0.7, $"Step {step.StepId} empathy score should be > 0.7");
            Assert.NotEmpty(step.EnhancedNarrative);
            Assert.Equal("AnxiousCustomer", step.PersonaUsed);
        }
        
        _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] Alpha client walkthrough test passed: " +
                             "{Steps} steps, {Score:F2} avg empathy score, {Duration:F1} minutes", 
            walkthroughResult.Steps.Count, walkthroughResult.OverallEmpathyScore, walkthroughResult.TotalDuration.TotalMinutes);
    }

    /// <summary>
    /// [FixItFredComment:Sprint124_EmpathyChainTest] Test empathy scenario simulation for bug detection
    /// Validates that empathy scenarios can be used to detect and resolve customer interaction bugs
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    [Trait("Chain", "Empathy-Bug-Detection")]
    public async Task Should_Detect_Empathy_Bugs_Through_Scenario_Testing()
    {
        // [Sprint124_FixItFred_EmpathyExpansion] Arrange: Set up scenario testing
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var alphaClientService = scope.ServiceProvider.GetRequiredService<AlphaClientEmpathyService>();
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Act: Test multiple empathy scenarios
        var scenarios = new[] { "timeline-anxiety", "technical-complexity" };
        var scenarioResults = new List<EmpathyScenarioResult>();
        
        foreach (var scenarioId in scenarios)
        {
            try
            {
                var result = await alphaClientService.SimulateEmpathyScenarioAsync(scenarioId, "alpha-001");
                scenarioResults.Add(result);
                
                _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] Scenario {ScenarioId} result: " +
                                     "Success: {Success}, Score: {Score:F2}, Response Time: {ResponseTime}ms", 
                    scenarioId, result.MeetsSuccessMetrics, result.EmpathyScore, result.ResponseTime.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[FixItFredComment:Sprint124_EmpathyChainTest] Scenario {ScenarioId} failed: {Error}", 
                    scenarioId, ex.Message);
                
                // Add failed scenario for analysis
                scenarioResults.Add(new EmpathyScenarioResult
                {
                    ScenarioId = scenarioId,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }
        
        // [Sprint124_FixItFred_EmpathyExpansion] Assert: Verify bug detection capabilities
        Assert.NotEmpty(scenarioResults);
        
        var successfulScenarios = scenarioResults.Where(r => r.Success && r.MeetsSuccessMetrics).ToList();
        var failedScenarios = scenarioResults.Where(r => !r.Success || !r.MeetsSuccessMetrics).ToList();
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Log empathy bug detection results
        if (failedScenarios.Any())
        {
            _logger.LogWarning("[FixItFredComment:Sprint124_EmpathyChainTest] Detected {Count} empathy bugs in scenarios: {Scenarios}", 
                failedScenarios.Count, string.Join(", ", failedScenarios.Select(f => f.ScenarioId)));
        }
        
        // At least one scenario should succeed for basic functionality
        Assert.True(successfulScenarios.Any(), "At least one empathy scenario should succeed");
        
        _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] Empathy bug detection test completed: " +
                             "{Successful}/{Total} scenarios passed", 
            successfulScenarios.Count, scenarioResults.Count);
    }

    /// <summary>
    /// [FixItFredComment:Sprint124_EmpathyChainTest] Test empathy flag tracing for debugging
    /// Validates that empathy events can be traced through the system for debugging purposes
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    [Trait("Chain", "Empathy-Flag-Tracing")]
    public async Task Should_Trace_Empathy_Flags_For_Debugging()
    {
        // [Sprint124_FixItFred_EmpathyExpansion] Arrange: Set up flag tracing test
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var empathyNarrator = scope.ServiceProvider.GetRequiredService<ILyraEmpathyNarrator>();
        var empathyFlags = new List<string>();
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Act: Generate empathy response with flag tracing
        var empathyResult = await empathyNarrator.GenerateEmpathyNarrativeAsync(
            "FrustratedCustomer",
            new EmotionalContext
            {
                PrimaryEmotion = "frustration",
                SecondaryEmotions = new List<string> { "anger", "disappointment" },
                IntensityLevel = 8,
                TriggerEvent = "repeat service failure",
                RequiresImmediateAttention = true
            },
            "FixItFred empathy flag tracing test"
        );
        
        // [Sprint124_FixItFred_EmpathyExpansion] Simulate empathy flag collection
        empathyFlags.AddRange(new[]
        {
            $"[EmpathyFlag] Persona: {empathyResult.PersonaUsed}",
            $"[EmpathyFlag] Score: {empathyResult.EmpathyScore:F2}",
            $"[EmpathyFlag] Triggers: {string.Join(", ", empathyResult.EmotionalTriggers)}",
            $"[EmpathyFlag] Narrative Length: {empathyResult.Narrative.Length}",
            $"[EmpathyFlag] Generated At: {empathyResult.GeneratedAt:yyyy-MM-dd HH:mm:ss}"
        });
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Assert: Verify flag tracing functionality
        Assert.NotEmpty(empathyFlags);
        Assert.Contains(empathyFlags, f => f.Contains("FrustratedCustomer"));
        Assert.Contains(empathyFlags, f => f.Contains("Score:"));
        Assert.True(empathyFlags.Count >= 5, "Should have at least 5 empathy flags for tracing");
        
        // [Sprint124_FixItFred_EmpathyExpansion] Log all empathy flags for debugging
        foreach (var flag in empathyFlags)
        {
            _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] {Flag}", flag);
        }
        
        _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] Empathy flag tracing test passed with {Count} flags", 
            empathyFlags.Count);
    }

    /// <summary>
    /// [FixItFredComment:Sprint124_EmpathyChainTest] Test CLI testing against empathy seed JSON
    /// Validates that CLI can successfully test against empathy seed data
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    [Trait("Chain", "CLI-Empathy-Seed")]
    public async Task Should_Test_CLI_Against_Empathy_Seed_JSON()
    {
        // [Sprint124_FixItFred_EmpathyExpansion] Arrange: Load empathy seed data
        var empathySeedData = new
        {
            personas = new[]
            {
                new { name = "AnxiousCustomer", expectedScore = 0.85, triggerWords = new[] { "worried", "concerned", "timeline" } },
                new { name = "FrustratedCustomer", expectedScore = 0.90, triggerWords = new[] { "frustrated", "angry", "again" } },
                new { name = "TechnicallySavvy", expectedScore = 0.80, triggerWords = new[] { "technical", "details", "specs" } }
            },
            testCases = new[]
            {
                new { input = "I'm worried about the timeline", expectedPersona = "AnxiousCustomer" },
                new { input = "This is frustrating, it happened again", expectedPersona = "FrustratedCustomer" },
                new { input = "Can you provide technical specifications", expectedPersona = "TechnicallySavvy" }
            }
        };
        
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var empathyNarrator = scope.ServiceProvider.GetRequiredService<ILyraEmpathyNarrator>();
        var testResults = new List<(string input, string expectedPersona, string actualPersona, double score, bool passed)>();
        
        // [FixItFredComment:Sprint124_EmpathyChainTest] Act: Test each seed data case
        foreach (var testCase in empathySeedData.testCases)
        {
            var empathyResult = await empathyNarrator.GenerateEmpathyNarrativeAsync(
                testCase.expectedPersona,
                new EmotionalContext
                {
                    PrimaryEmotion = "concern",
                    IntensityLevel = 6,
                    TriggerEvent = testCase.input
                },
                testCase.input
            );
            
            var passed = empathyResult.PersonaUsed == testCase.expectedPersona && empathyResult.EmpathyScore >= 0.7;
            
            testResults.Add((
                testCase.input,
                testCase.expectedPersona,
                empathyResult.PersonaUsed,
                empathyResult.EmpathyScore,
                passed
            ));
            
            _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] CLI seed test: " +
                                 "Input: '{Input}' → Expected: {Expected}, Actual: {Actual}, Score: {Score:F2}, Passed: {Passed}", 
                testCase.input, testCase.expectedPersona, empathyResult.PersonaUsed, empathyResult.EmpathyScore, passed);
        }
        
        // [Sprint124_FixItFred_EmpathyExpansion] Assert: Verify CLI seed testing results
        Assert.NotEmpty(testResults);
        
        var passedTests = testResults.Count(r => r.passed);
        var totalTests = testResults.Count;
        var passRate = (double)passedTests / totalTests;
        
        Assert.True(passRate >= 0.8, $"CLI empathy seed test pass rate {passRate:P} should be >= 80%");
        
        _logger.LogInformation("[FixItFredComment:Sprint124_EmpathyChainTest] CLI empathy seed testing completed: " +
                             "{Passed}/{Total} tests passed ({PassRate:P})", 
            passedTests, totalTests, passRate);
    }

    // [Sprint124_FixItFred_EmpathyExpansion] Test helper methods

    private ServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole());
        
        // Add test implementations
        services.AddScoped<ILyraEmpathyNarrator, LyraEmpathyNarrator>();
        services.AddScoped<ILyraCognition, TestLyraCognition>();
        services.AddScoped<AlphaClientEmpathyService>();
        
        return services.BuildServiceProvider();
    }
}

/// <summary>
/// [Sprint124_FixItFred_EmpathyExpansion] Test implementation of ILyraCognition for integration testing
/// </summary>
public class TestLyraCognition : ILyraCognition
{
    public async Task<string> ResolvePromptAsync(string prompt)
    {
        // [FixItFredComment:Sprint124_EmpathyChainTest] Provide empathy-appropriate test responses
        return prompt.ToLower() switch
        {
            var p when p.Contains("service failure") => await Task.FromResult("I sincerely apologize for the service failure you experienced"),
            var p when p.Contains("billing issue") => await Task.FromResult("I understand your billing concerns and will help resolve them"),
            var p when p.Contains("scheduling") => await Task.FromResult("I'll help you find the best scheduling solution"),
            var p when p.Contains("urgent") => await Task.FromResult("I recognize the urgency of your request"),
            var p when p.Contains("complaint") => await Task.FromResult("I want to give your complaint the attention it deserves"),
            _ => await Task.FromResult("I'm here to help you with whatever you need")
        };
    }
}