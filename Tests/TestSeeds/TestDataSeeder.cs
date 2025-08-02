using Data;
using Data.Models;

namespace Tests.TestSeeds;

/// <summary>
/// Test data seeder for injecting empathy data into AppDbContext during test setup
/// Sprint121: Tactical add-on for DI-enabled test framework
/// </summary>
public static class TestDataSeeder
{
    /// <summary>
    /// Seeds test empathy data into the database context
    /// </summary>
    /// <param name="context">Application database context</param>
    public static void SeedTestData(ApplicationDbContext context)
    {
        // Clear existing data for clean tests
        context.EmpathyPrompts.RemoveRange(context.EmpathyPrompts);
        
        // Add test empathy prompts
        var empathyPrompts = new List<EmpathyPrompt>
        {
            new EmpathyPrompt 
            { 
                Id = Guid.NewGuid(), 
                Text = "I'm sorry to hear that.", 
                Category = "Apology",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new EmpathyPrompt 
            { 
                Id = Guid.NewGuid(), 
                Text = "I understand how frustrating that must be.", 
                Category = "Understanding",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new EmpathyPrompt 
            { 
                Id = Guid.NewGuid(), 
                Text = "Let me help you resolve this issue.", 
                Category = "Support",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new EmpathyPrompt 
            { 
                Id = Guid.NewGuid(), 
                Text = "Thank you for your patience.", 
                Category = "Gratitude",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };
        
        context.EmpathyPrompts.AddRange(empathyPrompts);
        context.SaveChanges();
    }
    
    /// <summary>
    /// Seeds test data for Revitalize platform
    /// </summary>
    /// <param name="context">Application database context</param>
    public static void SeedRevitalizeTestData(ApplicationDbContext context)
    {
        // This can be expanded when Revitalize entities are added to DbContext
        // For now, placeholder for future expansion
    }
}