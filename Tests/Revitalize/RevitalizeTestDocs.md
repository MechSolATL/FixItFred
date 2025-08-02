# Revitalize Test Framework Documentation

**Sprint121 - DI-Enabled Test Framework with Tactical Add-ons**

## Overview

This document describes the enhanced DI-enabled test framework for the Revitalize SaaS platform, including tactical add-ons for empathy testing, mock services, and configuration management.

## Architecture

### Test Base Class: `RevitalizeTestBase`

The `RevitalizeTestBase` abstract class provides a foundation for all Revitalize tests with:

- **Dependency Injection Container** - Properly configured ServiceCollection with all dependencies
- **In-Memory Database** - Entity Framework in-memory database for isolated testing
- **Configuration Management** - Feature flags, Lyra tuning options, and platform settings
- **Test Data Seeding** - Automatic empathy prompt seeding and Revitalize test data
- **Mock Service Registration** - LyraCognition and FixItFred mock implementations

### Key Components

#### 1. TestSeeds Folder Structure
```
Tests/
└── TestSeeds/
    └── TestDataSeeder.cs
```

**TestDataSeeder** provides:
- `SeedTestData(ApplicationDbContext context)` - Seeds empathy prompts for Lyra testing
- `SeedRevitalizeTestData(ApplicationDbContext context)` - Seeds Revitalize platform data

#### 2. Mock Services
```
Tests/
└── Mocks/
    ├── LyraCognitionMock.cs
    └── FixItFredCLIMock.cs
```

**LyraCognitionMock** features:
- Context-aware empathy prompt resolution
- Configurable mock responses for specific scenarios
- Support for custom response injection during tests

**FixItFredCLIMock** features:
- Diagnostic execution simulation
- Health status mocking
- Configurable results for testing edge cases

## Configuration System

### Feature Flags & Settings

The test framework supports comprehensive configuration through `IConfiguration`:

```csharp
var configurationData = new Dictionary<string, string?>
{
    // Revitalize platform
    ["Revitalize:PlatformName"] = "Test Revitalize Platform",
    ["Revitalize:EnableDebugReplay"] = "true",
    
    // Lyra cognition
    ["Lyra:PromptMode"] = "Expanded",
    ["Lyra:EnableEmpathy"] = "true",
    
    // FixItFred diagnostics
    ["FixItFred:DiagnosticsEnabled"] = "true",
    
    // Feature toggles
    ["Features:EmpathyMode"] = "true"
};
```

## Writing Tests

### Basic Test Structure

```csharp
[Trait("Category", "Revitalize")]
[Trait("Layer", "Service")]
public class MyTests : RevitalizeTestBase
{
    protected override void RegisterRevitalizeServices(IServiceCollection services)
    {
        // Register your specific services
        services.AddScoped<IMyService, MyService>();
    }

    [Fact]
    [Trait("TestType", "Integration")]
    public async Task Should_Test_My_Feature()
    {
        using var serviceProvider = CreateTestServiceProvider();
        SeedTestData(serviceProvider); // Seeds empathy data automatically
        using var scope = serviceProvider.CreateScope();
        
        var service = scope.ServiceProvider.GetRequiredService<IMyService>();
        // ... test implementation
    }
}
```

### Trait Categories

The framework uses structured traits for test organization:

- **Category**: `Revitalize`, `Empathy`, `FixItFred`
- **Layer**: `Service`, `Diagnostic`, `Integration`
- **TestType**: `Unit`, `Integration`, `Performance`

### Using Mock Services

#### Lyra Cognition Testing
```csharp
[Fact]
public async Task Should_Resolve_Empathy_Prompt()
{
    using var serviceProvider = CreateTestServiceProvider();
    using var scope = serviceProvider.CreateScope();
    
    var lyraCognition = scope.ServiceProvider.GetRequiredService<ILyraCognition>();
    var result = await lyraCognition.ResolvePromptAsync("service failure");
    
    Assert.Contains("sorry", result.ToLower());
}
```

#### FixItFred Diagnostic Testing
```csharp
[Fact]
public async Task Should_Run_Diagnostics()
{
    using var serviceProvider = CreateTestServiceProvider();
    using var scope = serviceProvider.CreateScope();
    
    var fred = scope.ServiceProvider.GetRequiredService<IFixItFredCLI>();
    var result = await fred.RunDiagnosticsAsync();
    
    Assert.Contains("diagnostics", result.ToLower());
}
```

## Database Testing

### Empathy Prompt Seeding

The framework automatically seeds empathy prompts for testing:

```csharp
// Automatically seeded categories:
- Apology: "I'm sorry to hear that."
- Understanding: "I understand how frustrating that must be."
- Support: "Let me help you resolve this issue."
- Gratitude: "Thank you for your patience."
```

### Database Verification

```csharp
[Fact]
public void Should_Verify_Seeded_Data()
{
    using var serviceProvider = CreateTestServiceProvider();
    SeedTestData(serviceProvider);
    using var scope = serviceProvider.CreateScope();
    
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var prompts = context.EmpathyPrompts.ToList();
    
    Assert.NotEmpty(prompts);
    Assert.True(prompts.Count >= 4);
}
```

## Test Execution

### Running Specific Categories
```bash
# Run all Revitalize tests
dotnet test --filter "Category=Revitalize"

# Run empathy tests only
dotnet test --filter "Category=Empathy"

# Run diagnostic tests
dotnet test --filter "Category=FixItFred"

# Run by layer
dotnet test --filter "Layer=Service"

# Run by test type
dotnet test --filter "TestType=Integration"
```

### CI Integration

The trait system enables organized test execution in CI pipelines:

```yaml
# Example CI configuration
- name: Run Unit Tests
  run: dotnet test --filter "TestType=Unit"
  
- name: Run Integration Tests  
  run: dotnet test --filter "TestType=Integration"
  
- name: Run Diagnostic Tests
  run: dotnet test --filter "Category=FixItFred"
```

## Advanced Features

### Custom Mock Responses

```csharp
// In test setup
var mockService = scope.ServiceProvider.GetRequiredService<LyraCognitionMock>();
mockService.AddMockResponse("custom_scenario", "Custom empathy response");
```

### Configuration Override

```csharp
// Override specific configuration for a test
protected override void RegisterRevitalizeServices(IServiceCollection services)
{
    base.RegisterRevitalizeServices(services);
    
    // Add test-specific configuration
    services.Configure<MyOptions>(options => 
    {
        options.TestMode = true;
    });
}
```

## Best Practices

### 1. Use Proper Scoping
Always use `using var scope = serviceProvider.CreateScope()` to ensure proper disposal of services.

### 2. Seed Data Consistently
Call `SeedTestData(serviceProvider)` in tests that require empathy data or database verification.

### 3. Apply Meaningful Traits
Use consistent trait categories to enable effective test filtering and CI organization.

### 4. Mock External Dependencies
Use the provided mock services for external dependencies like Lyra cognition and FixItFred diagnostics.

### 5. Test DI Registration
Always include tests that verify services can be resolved from the DI container.

## Future Enhancements

- **Performance Testing Traits** - Add performance-specific test categories
- **Client Pattern Recall** - Integration with client pattern testing
- **Replay Engine Integration** - Connection to Revitalize replay CLI functionality
- **Enhanced Seeding** - More comprehensive test data scenarios

---

**Last Updated:** Sprint121 Implementation  
**Framework Version:** 1.0.0-Sprint121