namespace MVP_Core.Services.Logs.Empathy;

/// <summary>
/// Empathy Log record type for Sprint122_CertumDNSBypass
/// Captures empathetic interactions and emotional context
/// </summary>
public record EmpathyLog
{
    public string LogId { get; init; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string UserId { get; init; } = string.Empty;
    public string SessionId { get; init; } = string.Empty;
    public EmpathyLevel Level { get; init; } = EmpathyLevel.Neutral;
    public string Context { get; init; } = string.Empty;
    public string UserInput { get; init; } = string.Empty;
    public string SystemResponse { get; init; } = string.Empty;
    public EmotionalState UserState { get; init; } = EmotionalState.Unknown;
    public Dictionary<string, object> Metadata { get; init; } = new();
    
    /// <summary>
    /// Empathy levels for AI interactions
    /// </summary>
    public enum EmpathyLevel
    {
        Low = 1,
        Neutral = 2,
        Moderate = 3,
        High = 4,
        Exceptional = 5
    }
    
    /// <summary>
    /// Emotional states detected in user interactions
    /// </summary>
    public enum EmotionalState
    {
        Unknown,
        Happy,
        Frustrated,
        Confused,
        Satisfied,
        Angry,
        Grateful,
        Anxious,
        Calm
    }
    
    /// <summary>
    /// Creates an empathy log entry with basic information
    /// </summary>
    public static EmpathyLog Create(string userId, string context, EmpathyLevel level = EmpathyLevel.Neutral)
    {
        return new EmpathyLog
        {
            UserId = userId,
            Context = context,
            Level = level,
            SessionId = GenerateSessionId()
        };
    }
    
    /// <summary>
    /// Creates an empathy log entry for user interaction
    /// </summary>
    public static EmpathyLog CreateInteraction(
        string userId, 
        string userInput, 
        string systemResponse, 
        EmotionalState userState,
        EmpathyLevel empathyLevel = EmpathyLevel.Moderate)
    {
        return new EmpathyLog
        {
            UserId = userId,
            UserInput = userInput,
            SystemResponse = systemResponse,
            UserState = userState,
            Level = empathyLevel,
            Context = "User Interaction",
            SessionId = GenerateSessionId()
        };
    }
    
    /// <summary>
    /// Creates an empathy log for error handling
    /// </summary>
    public static EmpathyLog CreateErrorLog(string userId, string errorContext, string supportiveResponse)
    {
        return new EmpathyLog
        {
            UserId = userId,
            Context = $"Error Support: {errorContext}",
            SystemResponse = supportiveResponse,
            Level = EmpathyLevel.High,
            UserState = EmotionalState.Frustrated,
            SessionId = GenerateSessionId()
        };
    }
    
    /// <summary>
    /// Adds metadata to the empathy log
    /// </summary>
    public EmpathyLog WithMetadata(string key, object value)
    {
        var newMetadata = new Dictionary<string, object>(Metadata)
        {
            [key] = value
        };
        
        return this with { Metadata = newMetadata };
    }
    
    /// <summary>
    /// Adds emotional context metadata
    /// </summary>
    public EmpathyLog WithEmotionalContext(string sentiment, double confidence)
    {
        return WithMetadata("sentiment", sentiment)
               .WithMetadata("confidence", confidence)
               .WithMetadata("analysis_timestamp", DateTime.UtcNow);
    }
    
    /// <summary>
    /// Converts the log to a formatted string for output
    /// </summary>
    public override string ToString()
    {
        return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] EMPATHY_LOG: {LogId} | User: {UserId} | Level: {Level} | State: {UserState} | Context: {Context}";
    }
    
    /// <summary>
    /// Gets a detailed log entry for file output
    /// </summary>
    public string ToDetailedString()
    {
        var metadataStr = string.Join(", ", Metadata.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        
        return $"""
            LogId: {LogId}
            Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss} UTC
            UserId: {UserId}
            SessionId: {SessionId}
            EmpathyLevel: {Level}
            UserState: {UserState}
            Context: {Context}
            UserInput: {UserInput}
            SystemResponse: {SystemResponse}
            Metadata: {metadataStr}
            ---
            """;
    }
    
    /// <summary>
    /// Validates if the empathy log entry is complete
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(UserId) &&
               !string.IsNullOrWhiteSpace(Context) &&
               Timestamp != default;
    }
    
    /// <summary>
    /// Gets empathy score based on level and context
    /// </summary>
    public double GetEmpathyScore()
    {
        var baseScore = (int)Level * 20.0; // 20-100 scale
        
        // Adjust based on emotional state
        var adjustment = UserState switch
        {
            EmotionalState.Frustrated or EmotionalState.Angry => 10.0,
            EmotionalState.Confused or EmotionalState.Anxious => 5.0,
            EmotionalState.Happy or EmotionalState.Grateful => -5.0, // Less empathy needed
            EmotionalState.Satisfied or EmotionalState.Calm => -10.0,
            _ => 0.0
        };
        
        return Math.Min(100.0, Math.Max(0.0, baseScore + adjustment));
    }
    
    private static string GenerateSessionId()
    {
        return $"EMP_{DateTime.UtcNow:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}";
    }
}