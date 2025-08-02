namespace Interfaces;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Interface for replay transcript storage and empathy integration
/// Manages storage, retrieval, and analysis of service interaction transcripts with empathy logging
/// Used by: ReplayTranscriptStore service, ReplayEngineService, empathy analysis systems
/// CLI Trigger: ServiceAtlantaCLI transcript-store commands, RevitalizeCLI replay operations
/// UI Trigger: Admin replay dashboard, incident analysis interface, empathy review panels
/// Side Effects: Persists transcript data, indexes empathy events, triggers replay analysis workflows
/// </summary>
public interface IReplayTranscriptStore
{
    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Loads transcript data asynchronously with empathy annotations
    /// Required method for empathy + replay integration as specified in Sprint123 objectives
    /// </summary>
    /// <param name="transcriptId">Unique transcript identifier</param>
    /// <param name="includeEmpathyData">Whether to include empathy annotation data</param>
    /// <param name="timeRange">Optional time range filter for transcript segments</param>
    /// <returns>Complete transcript with optional empathy annotations</returns>
    Task<TranscriptData?> LoadTranscriptAsync(Guid transcriptId, bool includeEmpathyData = true, TimeRange? timeRange = null);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Saves empathy log entries linked to transcript events
    /// Required method for empathy + replay integration as specified in Sprint123 objectives
    /// </summary>
    /// <param name="transcriptId">Associated transcript identifier</param>
    /// <param name="empathyEvent">Empathy event data to log</param>
    /// <param name="timestamp">Event timestamp</param>
    /// <param name="metadata">Additional empathy metadata</param>
    /// <returns>True if empathy log was successfully saved</returns>
    Task<bool> SaveEmpathyLog(Guid transcriptId, EmpathyEventData empathyEvent, DateTime timestamp, Dictionary<string, object>? metadata = null);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Gets replay summary with empathy analysis integration
    /// Required method for empathy + replay integration as specified in Sprint123 objectives
    /// </summary>
    /// <param name="replaySessionId">Replay session identifier</param>
    /// <param name="includeEmpathyMetrics">Whether to include empathy effectiveness metrics</param>
    /// <returns>Comprehensive replay summary with empathy analysis</returns>
    Task<ReplaySummaryData?> GetReplaySummary(Guid replaySessionId, bool includeEmpathyMetrics = true);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Stores complete transcript data with indexing
    /// </summary>
    /// <param name="transcript">Complete transcript data to store</param>
    /// <param name="searchableFields">Fields to index for searching</param>
    /// <returns>Storage operation result with assigned transcript ID</returns>
    Task<TranscriptStorageResult> StoreTranscriptAsync(CompleteTranscriptData transcript, List<string>? searchableFields = null);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Searches transcripts by empathy patterns and criteria
    /// </summary>
    /// <param name="searchCriteria">Search criteria including empathy filters</param>
    /// <param name="pageSize">Number of results per page</param>
    /// <param name="pageIndex">Page index for pagination</param>
    /// <returns>Paginated search results with relevance scoring</returns>
    Task<TranscriptSearchResults> SearchTranscriptsAsync(EmpathyTranscriptSearchCriteria searchCriteria, int pageSize = 50, int pageIndex = 0);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Archives old transcripts with empathy data retention
    /// </summary>
    /// <param name="archiveOlderThan">Archive transcripts older than this date</param>
    /// <param name="retainEmpathyData">Whether to retain empathy data in archive</param>
    /// <returns>Archive operation result with statistics</returns>
    Task<ArchiveResult> ArchiveTranscriptsAsync(DateTime archiveOlderThan, bool retainEmpathyData = true);

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Gets empathy analytics from stored transcript data
    /// </summary>
    /// <param name="analysisTimeframe">Timeframe for analysis</param>
    /// <param name="groupBy">Grouping criteria (customer, technician, service type, etc.)</param>
    /// <returns>Empathy analytics with trend data</returns>
    Task<EmpathyAnalyticsResult> GetEmpathyAnalyticsAsync(TimeRange analysisTimeframe, string groupBy = "customer");
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Time range filter for transcript operations
/// </summary>
public class TimeRange
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Complete transcript data with empathy annotations
/// </summary>
public class TranscriptData
{
    public Guid TranscriptId { get; set; }
    public Guid ServiceRequestId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<TranscriptSegment> Segments { get; set; } = new();
    public List<EmpathyAnnotation> EmpathyAnnotations { get; set; } = new();
    public string InteractionType { get; set; } = string.Empty;
    public List<string> Participants { get; set; } = new();
    public TranscriptMetadata Metadata { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Individual transcript segment
/// </summary>
public class TranscriptSegment
{
    public Guid SegmentId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Speaker { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string SegmentType { get; set; } = string.Empty; // Speech, Action, System
    public Dictionary<string, object> SegmentMetadata { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Empathy event data for logging
/// </summary>
public class EmpathyEventData
{
    public string EventType { get; set; } = string.Empty;
    public string PersonaDetected { get; set; } = string.Empty;
    public string EmotionalState { get; set; } = string.Empty;
    public string EmpathyResponse { get; set; } = string.Empty;
    public double EffectivenessScore { get; set; }
    public string TriggerContext { get; set; } = string.Empty;
    public bool RequiredIntervention { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Empathy annotation linked to transcript segments
/// </summary>
public class EmpathyAnnotation
{
    public Guid AnnotationId { get; set; }
    public Guid SegmentId { get; set; }
    public EmpathyEventData EmpathyEvent { get; set; } = new();
    public DateTime AnnotatedAt { get; set; }
    public string AnnotatedBy { get; set; } = string.Empty; // System, Human, AI
    public double ConfidenceScore { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Replay summary with empathy analysis
/// </summary>
public class ReplaySummaryData
{
    public Guid ReplaySessionId { get; set; }
    public Guid OriginalTranscriptId { get; set; }
    public DateTime ReplayedAt { get; set; }
    public string ReplayTrigger { get; set; } = string.Empty;
    public TimeSpan OriginalDuration { get; set; }
    public TimeSpan ReplayDuration { get; set; }
    public List<ReplayEvent> Events { get; set; } = new();
    public EmpathyReplayMetrics? EmpathyMetrics { get; set; }
    public string ReplayOutcome { get; set; } = string.Empty;
    public List<string> LessonsLearned { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Individual replay event
/// </summary>
public class ReplayEvent
{
    public DateTime EventTime { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool WasEmpathyRelated { get; set; }
    public Dictionary<string, object> EventData { get; set; } = new();
}

/// <summary>
/// [Sprint124_FixItFred_EmpathyExpansion] Enhanced empathy metrics from replay analysis with persona-specific data
/// Provides detailed empathy analytics broken down by customer persona types
/// </summary>
public class EmpathyReplayMetrics
{
    public int TotalEmpathyEvents { get; set; }
    public double AverageEmpathyScore { get; set; }
    public int MissedEmpathyOpportunities { get; set; }
    public List<string> EmpathyImprovementAreas { get; set; } = new();
    public Dictionary<string, int> PersonaEncounters { get; set; } = new();
    public double CustomerSatisfactionImpact { get; set; }
    
    /// <summary>
    /// [Sprint124_FixItFred_EmpathyExpansion] Persona-specific empathy analytics for Sprint124 objectives
    /// </summary>
    public Dictionary<string, PersonaEmpathyMetrics> PersonaMetrics { get; set; } = new();
    
    /// <summary>
    /// [Sprint124_FixItFred_EmpathyExpansion] Graph-ready empathy data for dashboard visualization
    /// </summary>
    public List<EmpathyTrendPoint> EmpathyTrendData { get; set; } = new();
    
    /// <summary>
    /// [Sprint124_FixItFred_EmpathyExpansion] Real-time empathy effectiveness scores
    /// </summary>
    public Dictionary<string, double> PersonaEffectivenessScores { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Complete transcript data for storage
/// </summary>
public class CompleteTranscriptData
{
    public string RawTranscriptData { get; set; } = string.Empty;
    public TranscriptMetadata Metadata { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string SourceSystem { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Transcript metadata
/// </summary>
public class TranscriptMetadata
{
    public string ServiceType { get; set; } = string.Empty;
    public string CustomerTier { get; set; } = string.Empty;
    public string TechnicianLevel { get; set; } = string.Empty;
    public bool ContainsEmpathyEvents { get; set; }
    public string QualityRating { get; set; } = string.Empty;
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Transcript storage operation result
/// </summary>
public class TranscriptStorageResult
{
    public bool Success { get; set; }
    public Guid? TranscriptId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StoredAt { get; set; }
    public long StorageSize { get; set; }
    public bool IndexingCompleted { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Search criteria for empathy-enhanced transcript search
/// </summary>
public class EmpathyTranscriptSearchCriteria
{
    public string? CustomerPersona { get; set; }
    public string? EmotionalState { get; set; }
    public string? ServiceType { get; set; }
    public TimeRange? DateRange { get; set; }
    public double? MinEmpathyScore { get; set; }
    public bool? ContainsMissedOpportunities { get; set; }
    public List<string> Keywords { get; set; } = new();
    public string? TechnicianId { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Transcript search results with pagination
/// </summary>
public class TranscriptSearchResults
{
    public List<TranscriptSearchResult> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
    public TimeSpan SearchDuration { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Individual transcript search result
/// </summary>
public class TranscriptSearchResult
{
    public Guid TranscriptId { get; set; }
    public string Summary { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public DateTime RecordedAt { get; set; }
    public List<string> MatchedKeywords { get; set; } = new();
    public EmpathySearchHighlights? EmpathyHighlights { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Empathy-related search highlights
/// </summary>
public class EmpathySearchHighlights
{
    public List<string> EmpathyMoments { get; set; } = new();
    public string DominantPersona { get; set; } = string.Empty;
    public double EmpathyEffectiveness { get; set; }
    public int MissedOpportunityCount { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Archive operation result
/// </summary>
public class ArchiveResult
{
    public bool Success { get; set; }
    public int TranscriptsArchived { get; set; }
    public long SpaceFreed { get; set; }
    public int EmpathyRecordsRetained { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ArchivedAt { get; set; }
}

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Empathy analytics result
/// </summary>
public class EmpathyAnalyticsResult
{
    public TimeRange AnalysisTimeframe { get; set; } = new();
    public Dictionary<string, double> EmpathyTrends { get; set; } = new();
    public Dictionary<string, int> PersonaDistribution { get; set; } = new();
    public double OverallEmpathyImprovement { get; set; }
    public List<EmpathyInsight> KeyInsights { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// [Sprint124_FixItFred_EmpathyExpansion] Individual empathy insight
/// </summary>
public class EmpathyInsight
{
    public string InsightType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Impact { get; set; }
    public List<string> RecommendedActions { get; set; } = new();
}

/// <summary>
/// [Sprint124_FixItFred_EmpathyExpansion] Persona-specific empathy metrics for detailed analysis
/// </summary>
public class PersonaEmpathyMetrics
{
    public string PersonaType { get; set; } = string.Empty;
    public int TotalInteractions { get; set; }
    public double AverageEmpathyScore { get; set; }
    public int SuccessfulEmpathyResponses { get; set; }
    public int MissedOpportunities { get; set; }
    public double ResponseTime { get; set; } // Average time to empathy response
    public List<string> CommonTriggers { get; set; } = new();
    public List<string> EffectiveResponses { get; set; } = new();
    public double CustomerSatisfactionDelta { get; set; } // Change in satisfaction due to empathy
}

/// <summary>
/// [Sprint124_FixItFred_EmpathyExpansion] Time-series empathy trend point for graphing
/// </summary>
public class EmpathyTrendPoint
{
    public DateTime Timestamp { get; set; }
    public string PersonaType { get; set; } = string.Empty;
    public double EmpathyScore { get; set; }
    public int EventCount { get; set; }
    public string TrendDirection { get; set; } = string.Empty; // "improving", "declining", "stable"
}