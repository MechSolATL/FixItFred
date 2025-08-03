using Microsoft.Extensions.DependencyInjection;
using Interfaces;
using Data;
using Data.Models;
using Tests.TestSeeds;
using Xunit;

namespace Tests.Revitalize;

/// <summary>
/// Tests for empathy intake functionality using DI-enabled test framework
/// Sprint121+122: Tactical add-on for Lyra cognition testing with persona traits
/// </summary>
[Trait("Category", "Empathy")]
[Trait("Layer", "Service")]
public class EmpathyIntakeTests : RevitalizeTestBase
{
    /// <summary>
    /// Test empathy prompt resolution via Lyra cognition mock
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public async Task Should_Resolve_Empathy_Prompt_Via_LyraCognition()
    {
        using var serviceProvider = CreateTestServiceProvider();
        SeedTestData(serviceProvider);
        using var scope = serviceProvider.CreateScope();
        
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        // Test basic prompt resolution
        var result = await lyraCognition.ResolvePromptAsync("service failure");
        Assert.NotNull(result);
        Assert.Contains("sorry", result.ToLower());
    }

    /// <summary>
    /// Test that empathy prompts are properly seeded in database
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    public void Should_Seed_Empathy_Prompts_In_Database()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created and schema is applied
        context.Database.EnsureCreated();
        
        // Seed comprehensive empathy test data directly in the same context
        TestDataSeeder.SeedTestData(context);
        
        // Verify empathy prompts were seeded in the same context
        var prompts = context.EmpathyPrompts.ToList();
        Assert.NotEmpty(prompts);
        Assert.True(prompts.Count >= 4); // We seed 4 prompts
        
        // Verify specific categories exist
        var categories = prompts.Select(p => p.Category).Distinct().ToList();
        Assert.Contains("Apology", categories);
        Assert.Contains("Understanding", categories);
        Assert.Contains("Support", categories);
        Assert.Contains("Gratitude", categories);
    }

    /// <summary>
    /// Test empathy prompt retrieval from database
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    public void Should_Retrieve_Empathy_Prompts_By_Category()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created and schema is applied
        context.Database.EnsureCreated();
        
        // Seed comprehensive empathy test data directly in the same context
        TestDataSeeder.SeedTestData(context);
        
        // Test retrieval by category
        var apologyPrompts = context.EmpathyPrompts
            .Where(p => p.Category == "Apology")
            .ToList();
        
        Assert.NotEmpty(apologyPrompts);
        Assert.All(apologyPrompts, p => Assert.Equal("Apology", p.Category));
    }

    /// <summary>
    /// Test Lyra cognition with different context scenarios
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public async Task Should_Handle_Various_Context_Scenarios()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        // Test different scenarios
        var scenarios = new[]
        {
            "billing issue",
            "scheduling conflict", 
            "urgent request",
            "general complaint",
            "unknown context"
        };
        
        foreach (var scenario in scenarios)
        {
            var result = await lyraCognition.ResolvePromptAsync(scenario);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }

    /// <summary>
    /// Test empathy responses for AnxiousCustomer persona
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("Persona", "AnxiousCustomer")]
    public async Task Should_Handle_AnxiousCustomer_Scenarios()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        // Test anxious customer scenarios
        var result = await lyraCognition.ResolvePromptAsync("service failure");
        Assert.NotNull(result);
        Assert.Contains("sorry", result.ToLower());
        
        var billingResult = await lyraCognition.ResolvePromptAsync("billing issue");
        Assert.NotNull(billingResult);
        Assert.Contains("understand", billingResult.ToLower());
    }

    /// <summary>
    /// Test empathy responses for FrustratedCustomer persona
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("Persona", "FrustratedCustomer")]
    public async Task Should_Handle_FrustratedCustomer_Scenarios()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        // Test frustrated customer scenarios
        var result = await lyraCognition.ResolvePromptAsync("general complaint");
        Assert.NotNull(result);
        Assert.Contains("attention", result.ToLower());
        
        var urgentResult = await lyraCognition.ResolvePromptAsync("urgent request");
        Assert.NotNull(urgentResult);
    }

    /// <summary>
    /// Test empathy responses for TechnicallySavvy persona
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    [Trait("Persona", "TechnicallySavvy")]
    public async Task Should_Handle_TechnicallySavvy_Scenarios()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
        
        // Test technically savvy customer scenarios
        var result = await lyraCognition.ResolvePromptAsync("service failure");
        Assert.NotNull(result);
        
        var billingResult = await lyraCognition.ResolvePromptAsync("billing issue");
        Assert.NotNull(billingResult);
        Assert.Contains("help", billingResult.ToLower());
    }
}