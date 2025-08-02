namespace Interfaces;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Interface for synthesizing client behavioral patterns and recall data
/// Analyzes customer interaction patterns, service request history, and generates insights
/// Used by: ClientPatternRecallSynth service, Nova analytics engine
/// CLI Trigger: ServiceAtlantaCLI pattern-analysis commands
/// UI Trigger: Customer insight dashboard, technician pattern recommendations
/// Side Effects: Updates pattern cache, generates ML training data, triggers proactive alerts
/// </summary>
public interface IClientPatternRecallSynth
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Analyzes client interaction patterns
    /// </summary>
    /// <param name="clientId">Client identifier</param>
    /// <param name="timeframeInDays">Analysis timeframe in days</param>
    /// <returns>Synthesized pattern analysis with behavioral insights</returns>
    Task<PatternAnalysisResult> AnalyzeClientPatternsAsync(Guid clientId, int timeframeInDays);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Recalls historical service patterns for a client
    /// </summary>
    /// <param name="clientId">Client identifier</param>
    /// <param name="serviceType">Type of service to analyze</param>
    /// <returns>Historical pattern data with frequency analysis</returns>
    Task<List<ServicePatternRecall>> RecallServicePatternsAsync(Guid clientId, string serviceType);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Synthesizes predictive recommendations based on patterns
    /// </summary>
    /// <param name="clientId">Client identifier</param>
    /// <param name="currentContext">Current service context</param>
    /// <returns>Synthesized recommendations with confidence scores</returns>
    Task<List<PatternRecommendation>> SynthesizeRecommendationsAsync(Guid clientId, string currentContext);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Triggers pattern learning from completed service interactions
    /// </summary>
    /// <param name="serviceRequestId">Completed service request ID</param>
    /// <param name="outcome">Service outcome data</param>
    /// <returns>True if pattern was successfully learned</returns>
    Task<bool> LearnFromServiceOutcomeAsync(Guid serviceRequestId, ServiceOutcomeData outcome);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Validates pattern correlation accuracy
    /// </summary>
    /// <param name="clientId">Client identifier</param>
    /// <param name="predictedPattern">Previously predicted pattern</param>
    /// <param name="actualOutcome">Actual service outcome</param>
    /// <returns>Validation result with accuracy metrics</returns>
    Task<PatternValidationResult> ValidatePatternAccuracyAsync(Guid clientId, string predictedPattern, string actualOutcome);
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Pattern analysis result with behavioral insights
/// </summary>
public class PatternAnalysisResult
{
    public Guid ClientId { get; set; }
    public DateTime AnalysisDate { get; set; }
    public int TimeframeAnalyzed { get; set; }
    public List<BehavioralPattern> Patterns { get; set; } = new();
    public double ConfidenceScore { get; set; }
    public string PrimaryPattern { get; set; } = string.Empty;
    public List<string> Insights { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Individual behavioral pattern data
/// </summary>
public class BehavioralPattern
{
    public string PatternType { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public double Strength { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime FirstObserved { get; set; }
    public DateTime LastObserved { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Service pattern recall data
/// </summary>
public class ServicePatternRecall
{
    public string ServiceType { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
    public DateTime LastServiceDate { get; set; }
    public List<string> CommonIssues { get; set; } = new();
    public double AverageResolutionTime { get; set; }
    public string PreferredResolutionMethod { get; set; } = string.Empty;
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Pattern-based recommendation
/// </summary>
public class PatternRecommendation
{
    public string RecommendationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string BasedOnPattern { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Service outcome data for pattern learning
/// </summary>
public class ServiceOutcomeData
{
    public string ServiceType { get; set; } = string.Empty;
    public string ResolutionMethod { get; set; } = string.Empty;
    public TimeSpan ResolutionTime { get; set; }
    public int CustomerSatisfactionScore { get; set; }
    public bool RequiredFollowUp { get; set; }
    public List<string> IssuesEncountered { get; set; } = new();
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Pattern validation result
/// </summary>
public class PatternValidationResult
{
    public bool IsAccurate { get; set; }
    public double AccuracyPercentage { get; set; }
    public string ValidationNotes { get; set; } = string.Empty;
    public DateTime ValidatedAt { get; set; }
    public List<string> ImprovementSuggestions { get; set; } = new();
}