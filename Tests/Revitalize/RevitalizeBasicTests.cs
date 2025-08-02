using Microsoft.Extensions.DependencyInjection;
using Revitalize.Models;
using Revitalize.Services;
using Data;
using Xunit;
using RevitalizeServiceRequestService = Revitalize.Services.ServiceRequestService;

namespace Tests.Revitalize;

/// <summary>
/// Integration tests for Revitalize SaaS functionality using DI container - Sprint121
/// Following Option B from PR feedback: Manual DI setup for unit-style tests with DI
/// Enhanced with tactical add-ons: traits, test seeding, and mock services
/// </summary>
[Trait("Category", "Revitalize")]
[Trait("Layer", "Service")]
public class RevitalizeBasicTests : RevitalizeTestBase
{
    protected override void RegisterRevitalizeServices(IServiceCollection services)
    {
        // Register Revitalize services for testing
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IServiceRequestService, RevitalizeServiceRequestService>();
        services.AddScoped<IRevitalizeConfigService, RevitalizeConfigService>();
        services.AddScoped<IRevitalizeSeoService, RevitalizeSeoService>();
    }

    /// <summary>
    /// Test tenant creation and retrieval using DI-injected service
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    public async Task Should_Persist_Tenant_Using_TenantService()
    {
        using var serviceProvider = CreateTestServiceProvider();
        SeedTestData(serviceProvider); // Seed empathy data for testing
        using var scope = serviceProvider.CreateScope();
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        
        // Create a test tenant
        var newTenant = new RevitalizeTenant
        {
            CompanyName = "Test Plumbing Co",
            TenantCode = "TEST",
            Description = "Test tenant for Revitalize"
        };
        
        var created = await tenantService.CreateTenantAsync(newTenant);
        Assert.True(created.TenantId > 0);
        
        // Retrieve the tenant
        var retrieved = await tenantService.GetTenantAsync(created.TenantId);
        Assert.NotNull(retrieved);
        Assert.Equal("Test Plumbing Co", retrieved.CompanyName);
        
        // Get by code
        var byCode = await tenantService.GetTenantByCodeAsync("TEST");
        Assert.NotNull(byCode);
        Assert.Equal(created.TenantId, byCode.TenantId);
    }
    
    /// <summary>
    /// Test service request creation and management using DI-injected service
    /// </summary>
    [Fact]
    [Trait("TestType", "Integration")]
    public async Task Should_Persist_Request_Using_ServiceRequestService()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var serviceRequestService = scope.ServiceProvider.GetRequiredService<IServiceRequestService>();
        
        // Create a test service request
        var newRequest = new RevitalizeServiceRequest
        {
            TenantId = 1,
            Title = "Test Leak Repair",
            Description = "Test service request",
            ServiceType = RevitalizeServiceType.Plumbing,
            Priority = RevitalizePriority.Medium,
            CustomerName = "John Test",
            CustomerPhone = "555-TEST"
        };
        
        var created = await serviceRequestService.CreateServiceRequestAsync(newRequest);
        Assert.True(created.ServiceRequestId > 0);
        
        // Retrieve the request
        var retrieved = await serviceRequestService.GetServiceRequestAsync(created.ServiceRequestId);
        Assert.NotNull(retrieved);
        Assert.Equal("Test Leak Repair", retrieved.Title);
        
        // Test assignment
        var assigned = await serviceRequestService.AssignTechnicianAsync(created.ServiceRequestId, 1);
        Assert.True(assigned);
        
        // Verify assignment
        var afterAssignment = await serviceRequestService.GetServiceRequestAsync(created.ServiceRequestId);
        Assert.NotNull(afterAssignment);
        Assert.Equal(1, afterAssignment.AssignedTechnicianId);
        Assert.Equal(RevitalizeServiceRequestStatus.Assigned, afterAssignment.Status);
    }
    
    /// <summary>
    /// Test configuration service using DI-injected service
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public void Should_Access_RevitalizeConfigService_Via_DI()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var configService = scope.ServiceProvider.GetRequiredService<IRevitalizeConfigService>();
        
        // Verify the service can be resolved from DI container
        Assert.NotNull(configService);
        
        // Test basic configuration access
        var platformName = configService.GetPlatformName();
        Assert.False(string.IsNullOrEmpty(platformName));
        Assert.Equal("Test Revitalize Platform", platformName);
    }

    /// <summary>
    /// Test that all Revitalize services are properly registered in DI container
    /// </summary>
    [Fact]
    [Trait("TestType", "Unit")]
    public void Should_Resolve_All_Revitalize_Services_From_DI()
    {
        using var serviceProvider = CreateTestServiceProvider();
        using var scope = serviceProvider.CreateScope();
        
        // Verify all services can be resolved
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        var serviceRequestService = scope.ServiceProvider.GetRequiredService<IServiceRequestService>();
        var configService = scope.ServiceProvider.GetRequiredService<IRevitalizeConfigService>();
        var seoService = scope.ServiceProvider.GetRequiredService<IRevitalizeSeoService>();
        
        Assert.NotNull(tenantService);
        Assert.NotNull(serviceRequestService);
        Assert.NotNull(configService);
        Assert.NotNull(seoService);
    }
}