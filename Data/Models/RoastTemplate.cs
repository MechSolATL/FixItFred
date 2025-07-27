public enum RoastTier
{
    Soft,
    Medium,
    Savage,
    Brutal
}

public class RoastTemplate
{
    public int Id { get; set; }
    public string Message { get; set; }
    public RoastTier Tier { get; set; } // Soft, Medium, Savage, Brutal
    public string Category { get; set; } // e.g., looks, habits, lateness
    public int UseLimit { get; set; } // Max times this roast can be used before deactivation
    public int TimesUsed { get; set; } // Track usage
}
