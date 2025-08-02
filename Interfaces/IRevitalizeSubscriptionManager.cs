namespace Interfaces;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Interface for managing Revitalize SaaS subscriptions
/// Handles subscription lifecycle, billing, and tenant management operations
/// Used by: RevitalizeSubscriptionManager service
/// CLI Trigger: ServiceAtlantaCLI subscription commands
/// UI Trigger: Admin subscription management dashboard
/// Side Effects: Updates tenant billing status, subscription tiers, and access controls
/// </summary>
public interface IRevitalizeSubscriptionManager
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Creates a new subscription for a tenant
    /// </summary>
    /// <param name="tenantId">Unique identifier for the tenant</param>
    /// <param name="subscriptionTier">Subscription tier (Basic, Professional, Enterprise)</param>
    /// <param name="billingCycle">Billing cycle (Monthly, Yearly)</param>
    /// <returns>Subscription creation result with subscription ID</returns>
    Task<SubscriptionResult> CreateSubscriptionAsync(Guid tenantId, string subscriptionTier, string billingCycle);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Updates an existing subscription
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="newTier">New subscription tier</param>
    /// <returns>Update operation result</returns>
    Task<bool> UpdateSubscriptionTierAsync(Guid subscriptionId, string newTier);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Cancels a subscription
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <param name="reason">Cancellation reason</param>
    /// <returns>Cancellation operation result</returns>
    Task<bool> CancelSubscriptionAsync(Guid subscriptionId, string reason);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Gets subscription details
    /// </summary>
    /// <param name="subscriptionId">Subscription identifier</param>
    /// <returns>Subscription details or null if not found</returns>
    Task<SubscriptionViewModel?> GetSubscriptionAsync(Guid subscriptionId);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Validates subscription status and permissions
    /// </summary>
    /// <param name="tenantId">Tenant identifier</param>
    /// <param name="feature">Feature to validate access for</param>
    /// <returns>True if tenant has access to the feature</returns>
    Task<bool> ValidateFeatureAccessAsync(Guid tenantId, string feature);
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Subscription operation result
/// </summary>
public class SubscriptionResult
{
    public bool Success { get; set; }
    public Guid? SubscriptionId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Subscription view model for UI display
/// </summary>
public class SubscriptionViewModel
{
    public Guid SubscriptionId { get; set; }
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string SubscriptionTier { get; set; } = string.Empty;
    public string BillingCycle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public decimal MonthlyRate { get; set; }
    public List<string> Features { get; set; } = new();
}