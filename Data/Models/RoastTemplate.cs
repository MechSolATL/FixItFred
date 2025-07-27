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
    public string Message { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public RoastTier Tier { get; set; } // Soft, Medium, Savage, Brutal
    public string Category { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public int UseLimit { get; set; } // Max times this roast can be used before deactivation
    public int TimesUsed { get; set; } // Track usage
    // Sprint 73.9 fields
    public DateTime? LastUsedAt { get; set; }
    public double SuccessRate { get; set; }
    public bool Retired { get; set; }
    public bool AutoPromote { get; set; }
    public bool AIAuthored { get; set; }
    public bool LegacyStatus { get; set; }
}
