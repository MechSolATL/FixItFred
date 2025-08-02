namespace Interfaces;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Interface for Lyra empathy narrative generation and emotional intelligence
/// Generates empathetic responses based on customer personas and emotional context
/// Used by: LyraEmpathyNarrator service, RevitalizeReplayCLI for persona-based interactions
/// CLI Trigger: ServiceAtlantaCLI empathy-gen commands, RevitalizeCLI persona testing
/// UI Trigger: Customer service chat interface, technician guidance system
/// Side Effects: Updates empathy model training data, logs emotional context patterns
/// </summary>
public interface ILyraEmpathyNarrator
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Generates empathetic narrative based on customer persona
    /// </summary>
    /// <param name="persona">Customer persona (AnxiousCustomer, FrustratedCustomer, TechnicallySavvy, etc.)</param>
    /// <param name="emotionalContext">Current emotional state and context</param>
    /// <param name="serviceContext">Service situation context</param>
    /// <returns>Generated empathetic narrative response</returns>
    Task<EmpathyNarrativeResult> GenerateEmpathyNarrativeAsync(string persona, EmotionalContext emotionalContext, string serviceContext);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Adapts narrative tone based on interaction history
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="baseNarrative">Base narrative to adapt</param>
    /// <returns>Tone-adapted narrative with personality adjustments</returns>
    Task<string> AdaptNarrativeToneAsync(Guid customerId, string baseNarrative);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Validates empathy appropriateness for given context
    /// </summary>
    /// <param name="narrative">Proposed empathy narrative</param>
    /// <param name="contextSensitivity">Context sensitivity level</param>
    /// <returns>Validation result with appropriateness score</returns>
    Task<EmpathyValidationResult> ValidateEmpathyAppropriatenessAsync(string narrative, string contextSensitivity);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Streams real-time empathy suggestions during interactions
    /// </summary>
    /// <param name="interactionId">Current interaction identifier</param>
    /// <param name="realTimeContext">Live interaction context</param>
    /// <returns>Stream of empathy suggestions as interaction progresses</returns>
    IAsyncEnumerable<EmpathySuggestion> StreamEmpathySuggestionsAsync(Guid interactionId, string realTimeContext);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Learns from feedback to improve future empathy generation
    /// </summary>
    /// <param name="narrativeId">Generated narrative identifier</param>
    /// <param name="feedback">Customer/technician feedback on empathy effectiveness</param>
    /// <returns>True if feedback was successfully incorporated into learning</returns>
    Task<bool> LearnFromEmpathyFeedbackAsync(Guid narrativeId, EmpathyFeedback feedback);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Gets empathy statistics for performance analysis
    /// </summary>
    /// <param name="timeframeInDays">Analysis timeframe</param>
    /// <returns>Empathy performance statistics</returns>
    Task<EmpathyPerformanceStats> GetEmpathyStatsAsync(int timeframeInDays);
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Emotional context data for empathy generation
/// </summary>
public class EmotionalContext
{
    public string PrimaryEmotion { get; set; } = string.Empty;
    public List<string> SecondaryEmotions { get; set; } = new();
    public int IntensityLevel { get; set; } // 1-10 scale
    public string TriggerEvent { get; set; } = string.Empty;
    public TimeSpan EmotionalDuration { get; set; }
    public bool RequiresImmediateAttention { get; set; }
    public Dictionary<string, object> ContextualFactors { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Generated empathy narrative result
/// </summary>
public class EmpathyNarrativeResult
{
    public Guid NarrativeId { get; set; }
    public string Narrative { get; set; } = string.Empty;
    public string PersonaUsed { get; set; } = string.Empty;
    public string ToneApplied { get; set; } = string.Empty;
    public double EmpathyScore { get; set; }
    public List<string> EmotionalTriggers { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public List<string> SuggestedFollowUps { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Empathy validation result
/// </summary>
public class EmpathyValidationResult
{
    public bool IsAppropriate { get; set; }
    public double AppropriatenessScore { get; set; }
    public List<string> ValidationWarnings { get; set; } = new();
    public List<string> ImprovementSuggestions { get; set; } = new();
    public string ContextSensitivityAssessment { get; set; } = string.Empty;
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Real-time empathy suggestion
/// </summary>
public class EmpathySuggestion
{
    public string SuggestionType { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Trigger { get; set; } = string.Empty;
    public DateTime SuggestedAt { get; set; }
    public bool IsUrgent { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Empathy feedback for learning
/// </summary>
public class EmpathyFeedback
{
    public int EffectivenessRating { get; set; } // 1-5 scale
    public string FeedbackType { get; set; } = string.Empty; // Customer, Technician, Supervisor
    public string Comments { get; set; } = string.Empty;
    public bool WasHelpful { get; set; }
    public List<string> ImprovementAreas { get; set; } = new();
    public DateTime ProvidedAt { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Empathy performance statistics
/// </summary>
public class EmpathyPerformanceStats
{
    public int TotalNarrativesGenerated { get; set; }
    public double AverageEmpathyScore { get; set; }
    public double CustomerSatisfactionImpact { get; set; }
    public Dictionary<string, int> PersonaUsageStats { get; set; } = new();
    public Dictionary<string, double> ToneEffectiveness { get; set; } = new();
    public List<string> TopPerformingNarratives { get; set; } = new();
    public DateTime AnalysisPeriodStart { get; set; }
    public DateTime AnalysisPeriodEnd { get; set; }
}