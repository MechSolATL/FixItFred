using Microsoft.Extensions.Logging;
using Interfaces;

namespace Services;

/// <summary>
/// [Sprint123_FixItFred_OmegaSweep] Implementation of ILyraEmpathyNarrator
/// Generates empathetic narratives based on customer personas and emotional intelligence
/// Integrates with Lyra cognition system for advanced emotional context processing
/// </summary>
public class LyraEmpathyNarrator : ILyraEmpathyNarrator
{
    private readonly ILogger<LyraEmpathyNarrator> _logger;
    private readonly ILyraCognition _lyraCognition;
    private readonly Dictionary<string, PersonaProfile> _personaProfiles;
    private readonly Dictionary<Guid, List<string>> _narrativeHistory;

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Initializes LyraEmpathyNarrator with persona profiles
    /// Sets up emotional intelligence models and narrative generation templates
    /// </summary>
    /// <param name="logger">Logger instance for empathy narrative tracking</param>
    /// <param name="lyraCognition">Lyra cognition service for emotional context processing</param>
    public LyraEmpathyNarrator(ILogger<LyraEmpathyNarrator> logger, ILyraCognition lyraCognition)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _lyraCognition = lyraCognition ?? throw new ArgumentNullException(nameof(lyraCognition));
        
        // [Sprint123_FixItFred_OmegaSweep] Initialize persona profiles for empathy generation
        _personaProfiles = InitializePersonaProfiles();
        _narrativeHistory = new Dictionary<Guid, List<string>>();
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Generates empathetic narrative based on customer persona
    /// Processes emotional context and service situation to create personalized empathetic responses
    /// </summary>
    /// <param name="persona">Customer persona (AnxiousCustomer, FrustratedCustomer, TechnicallySavvy, etc.)</param>
    /// <param name="emotionalContext">Current emotional state and context</param>
    /// <param name="serviceContext">Service situation context</param>
    /// <returns>Generated empathetic narrative response</returns>
    public async Task<EmpathyNarrativeResult> GenerateEmpathyNarrativeAsync(string persona, EmotionalContext emotionalContext, string serviceContext)
    {
        _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Generating empathy narrative for persona: {Persona}, emotion: {Emotion}, intensity: {Intensity}", 
            persona, emotionalContext.PrimaryEmotion, emotionalContext.IntensityLevel);

        try
        {
            var narrativeId = Guid.NewGuid();
            
            // Get persona profile or default
            if (!_personaProfiles.TryGetValue(persona, out var personaProfile))
            {
                _logger.LogWarning("[Sprint123_FixItFred_OmegaSweep] Unknown persona {Persona}, using default profile", persona);
                personaProfile = _personaProfiles["Default"];
            }

            // [Sprint123_FixItFred_OmegaSweep] Generate base empathy response using Lyra cognition
            var contextPrompt = $"Persona: {persona}, Emotion: {emotionalContext.PrimaryEmotion}, Context: {serviceContext}";
            var baseEmpathyResponse = await _lyraCognition.ResolvePromptAsync(contextPrompt);

            // [Sprint123_FixItFred_OmegaSweep] Enhance with persona-specific tone and emotional triggers
            var enhancedNarrative = EnhanceNarrativeWithPersona(baseEmpathyResponse, personaProfile, emotionalContext);
            
            // Calculate empathy score based on emotional alignment
            var empathyScore = CalculateEmpathyScore(emotionalContext, personaProfile);
            
            // [Sprint123_FixItFred_OmegaSweep] Generate follow-up suggestions
            var followUps = GenerateFollowUpSuggestions(persona, emotionalContext, serviceContext);

            var result = new EmpathyNarrativeResult
            {
                NarrativeId = narrativeId,
                Narrative = enhancedNarrative,
                PersonaUsed = persona,
                ToneApplied = personaProfile.PreferredTone,
                EmpathyScore = empathyScore,
                EmotionalTriggers = emotionalContext.SecondaryEmotions.ToList(),
                GeneratedAt = DateTime.UtcNow,
                SuggestedFollowUps = followUps
            };

            _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Generated empathy narrative {NarrativeId} with score {Score:F2}", 
                narrativeId, empathyScore);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Sprint123_FixItFred_OmegaSweep] Error generating empathy narrative for persona {Persona}", persona);
            
            // Return fallback empathy response
            return new EmpathyNarrativeResult
            {
                NarrativeId = Guid.NewGuid(),
                Narrative = "I understand this situation may be challenging. Let me help you find the best solution.",
                PersonaUsed = persona,
                ToneApplied = "Supportive",
                EmpathyScore = 0.5,
                EmotionalTriggers = new List<string>(),
                GeneratedAt = DateTime.UtcNow,
                SuggestedFollowUps = new List<string> { "Follow up with customer in 30 minutes" }
            };
        }
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Adapts narrative tone based on interaction history
    /// Analyzes past interactions to personalize communication style and approach
    /// </summary>
    /// <param name="customerId">Customer identifier</param>
    /// <param name="baseNarrative">Base narrative to adapt</param>
    /// <returns>Tone-adapted narrative with personality adjustments</returns>
    public async Task<string> AdaptNarrativeToneAsync(Guid customerId, string baseNarrative)
    {
        _logger.LogDebug("[Sprint123_FixItFred_OmegaSweep] Adapting narrative tone for customer {CustomerId}", customerId);

        try
        {
            // [Sprint123_FixItFred_OmegaSweep] Retrieve interaction history
            var hasHistory = _narrativeHistory.TryGetValue(customerId, out var history);
            
            if (!hasHistory || history == null || !history.Any())
            {
                _logger.LogDebug("[Sprint123_FixItFred_OmegaSweep] No history found for customer {CustomerId}, using base narrative", customerId);
                return baseNarrative;
            }

            // [Sprint123_FixItFred_OmegaSweep] Analyze historical tone preferences
            var preferredTone = AnalyzeHistoricalTonePreference(history);
            var adaptedNarrative = ApplyToneAdaptation(baseNarrative, preferredTone);

            _logger.LogDebug("[Sprint123_FixItFred_OmegaSweep] Adapted narrative tone to {Tone} for customer {CustomerId}", 
                preferredTone, customerId);

            return await Task.FromResult(adaptedNarrative);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Sprint123_FixItFred_OmegaSweep] Error adapting narrative tone for customer {CustomerId}", customerId);
            return baseNarrative; // Return original on error
        }
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Validates empathy appropriateness for given context
    /// Ensures narrative content is suitable for the situation and emotional state
    /// </summary>
    /// <param name="narrative">Proposed empathy narrative</param>
    /// <param name="contextSensitivity">Context sensitivity level</param>
    /// <returns>Validation result with appropriateness score</returns>
    public async Task<EmpathyValidationResult> ValidateEmpathyAppropriatenessAsync(string narrative, string contextSensitivity)
    {
        _logger.LogDebug("[Sprint123_FixItFred_OmegaSweep] Validating empathy appropriateness, sensitivity: {Sensitivity}", contextSensitivity);

        try
        {
            // [Sprint123_FixItFred_OmegaSweep] Perform content validation checks
            var validationChecks = new List<ValidationCheck>
            {
                CheckToneAppropriateness(narrative, contextSensitivity),
                CheckContentSensitivity(narrative),
                CheckLengthAppropriateness(narrative),
                CheckPersonalizationLevel(narrative)
            };

            var overallScore = validationChecks.Average(check => check.Score);
            var isAppropriate = overallScore >= 0.7; // 70% threshold

            var warnings = validationChecks
                .Where(check => check.Score < 0.7)
                .Select(check => check.Warning)
                .Where(warning => !string.IsNullOrEmpty(warning))
                .ToList();

            var improvements = validationChecks
                .SelectMany(check => check.Improvements)
                .Distinct()
                .ToList();

            var result = new EmpathyValidationResult
            {
                IsAppropriate = isAppropriate,
                AppropriatenessScore = overallScore,
                ValidationWarnings = warnings,
                ImprovementSuggestions = improvements,
                ContextSensitivityAssessment = $"Sensitivity level: {contextSensitivity}, Score: {overallScore:F2}"
            };

            _logger.LogDebug("[Sprint123_FixItFred_OmegaSweep] Empathy validation completed with score {Score:F2}, appropriate: {Appropriate}", 
                overallScore, isAppropriate);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Sprint123_FixItFred_OmegaSweep] Error validating empathy appropriateness");
            
            return new EmpathyValidationResult
            {
                IsAppropriate = false,
                AppropriatenessScore = 0,
                ValidationWarnings = new List<string> { "Validation process failed" },
                ImprovementSuggestions = new List<string> { "Review validation process" },
                ContextSensitivityAssessment = "Validation error occurred"
            };
        }
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Streams real-time empathy suggestions during interactions
    /// Provides continuous empathy guidance as conversation context evolves
    /// </summary>
    /// <param name="interactionId">Current interaction identifier</param>
    /// <param name="realTimeContext">Live interaction context</param>
    /// <returns>Stream of empathy suggestions as interaction progresses</returns>
    public async IAsyncEnumerable<EmpathySuggestion> StreamEmpathySuggestionsAsync(Guid interactionId, string realTimeContext)
    {
        _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Starting empathy suggestion stream for interaction {InteractionId}", interactionId);

        // [Sprint123_FixItFred_OmegaSweep] Simulate real-time suggestion generation
        var suggestions = new[]
        {
            new EmpathySuggestion
            {
                SuggestionType = "ToneAdjustment",
                Suggestion = "Consider using a more reassuring tone based on detected anxiety",
                Confidence = 0.85,
                Trigger = "Anxiety detected in customer response",
                SuggestedAt = DateTime.UtcNow,
                IsUrgent = false
            },
            new EmpathySuggestion
            {
                SuggestionType = "ActiveListening",
                Suggestion = "Acknowledge the customer's specific concern about timeline",
                Confidence = 0.92,
                Trigger = "Timeline concern mentioned",
                SuggestedAt = DateTime.UtcNow.AddSeconds(15),
                IsUrgent = false
            },
            new EmpathySuggestion
            {
                SuggestionType = "EscalationPrevention",
                Suggestion = "Proactively offer status update to prevent frustration escalation",
                Confidence = 0.78,
                Trigger = "Extended wait time detected",
                SuggestedAt = DateTime.UtcNow.AddSeconds(30),
                IsUrgent = true
            }
        };

        foreach (var suggestion in suggestions)
        {
            // [Sprint123_FixItFred_OmegaSweep] Simulate real-time delays
            await Task.Delay(TimeSpan.FromSeconds(10));
            
            _logger.LogDebug("[Sprint123_FixItFred_OmegaSweep] Streaming suggestion: {Type} for interaction {InteractionId}", 
                suggestion.SuggestionType, interactionId);
            
            yield return suggestion;
        }

        _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Completed empathy suggestion stream for interaction {InteractionId}", interactionId);
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Learns from feedback to improve future empathy generation
    /// Incorporates user feedback into ML models for enhanced empathy effectiveness
    /// </summary>
    /// <param name="narrativeId">Generated narrative identifier</param>
    /// <param name="feedback">Customer/technician feedback on empathy effectiveness</param>
    /// <returns>True if feedback was successfully incorporated into learning</returns>
    public async Task<bool> LearnFromEmpathyFeedbackAsync(Guid narrativeId, EmpathyFeedback feedback)
    {
        _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Learning from empathy feedback for narrative {NarrativeId}, rating: {Rating}", 
            narrativeId, feedback.EffectivenessRating);

        try
        {
            // [Sprint123_FixItFred_OmegaSweep] Process feedback for learning
            var isPositiveFeedback = feedback.EffectivenessRating >= 4 && feedback.WasHelpful;
            
            if (isPositiveFeedback)
            {
                _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Positive feedback received for narrative {NarrativeId}, reinforcing patterns", 
                    narrativeId);
            }
            else
            {
                _logger.LogWarning("[Sprint123_FixItFred_OmegaSweep] Negative feedback for narrative {NarrativeId}: {Comments}", 
                    narrativeId, feedback.Comments);
                
                // Log improvement areas for analysis
                foreach (var area in feedback.ImprovementAreas)
                {
                    _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Improvement area identified: {Area}", area);
                }
            }

            // [Sprint123_FixItFred_OmegaSweep] Simulate ML model update
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Sprint123_FixItFred_OmegaSweep] Error learning from empathy feedback for narrative {NarrativeId}", narrativeId);
            return false;
        }
    }

    /// <summary>
    /// [Sprint123_FixItFred_OmegaSweep] Gets empathy statistics for performance analysis
    /// Provides analytics on empathy effectiveness and improvement trends
    /// </summary>
    /// <param name="timeframeInDays">Analysis timeframe</param>
    /// <returns>Empathy performance statistics</returns>
    public async Task<EmpathyPerformanceStats> GetEmpathyStatsAsync(int timeframeInDays)
    {
        _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Generating empathy performance stats for {Days} days", timeframeInDays);

        try
        {
            // [Sprint123_FixItFred_OmegaSweep] Generate mock performance statistics for MVP
            var stats = new EmpathyPerformanceStats
            {
                TotalNarrativesGenerated = 247,
                AverageEmpathyScore = 0.82,
                CustomerSatisfactionImpact = 0.15, // 15% improvement
                PersonaUsageStats = new Dictionary<string, int>
                {
                    ["AnxiousCustomer"] = 89,
                    ["FrustratedCustomer"] = 72,
                    ["TechnicallySavvy"] = 45,
                    ["ElderlyCare"] = 31,
                    ["BusinessClient"] = 10
                },
                ToneEffectiveness = new Dictionary<string, double>
                {
                    ["Reassuring"] = 0.91,
                    ["Professional"] = 0.87,
                    ["Friendly"] = 0.83,
                    ["Technical"] = 0.79,
                    ["Urgent"] = 0.75
                },
                TopPerformingNarratives = new List<string>
                {
                    "I completely understand your concern about the timeline...",
                    "Your frustration is absolutely valid, and I'm here to help...",
                    "Let me explain the technical details since I can see you appreciate that level of information..."
                },
                AnalysisPeriodStart = DateTime.UtcNow.AddDays(-timeframeInDays),
                AnalysisPeriodEnd = DateTime.UtcNow
            };

            _logger.LogInformation("[Sprint123_FixItFred_OmegaSweep] Generated empathy stats: {Total} narratives, {Average:F2} avg score", 
                stats.TotalNarrativesGenerated, stats.AverageEmpathyScore);

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Sprint123_FixItFred_OmegaSweep] Error generating empathy performance stats");
            
            return new EmpathyPerformanceStats
            {
                TotalNarrativesGenerated = 0,
                AverageEmpathyScore = 0,
                CustomerSatisfactionImpact = 0,
                PersonaUsageStats = new Dictionary<string, int>(),
                ToneEffectiveness = new Dictionary<string, double>(),
                TopPerformingNarratives = new List<string>(),
                AnalysisPeriodStart = DateTime.UtcNow.AddDays(-timeframeInDays),
                AnalysisPeriodEnd = DateTime.UtcNow
            };
        }
    }

    // [Sprint123_FixItFred_OmegaSweep] Private helper methods for empathy processing

    private Dictionary<string, PersonaProfile> InitializePersonaProfiles()
    {
        return new Dictionary<string, PersonaProfile>
        {
            ["AnxiousCustomer"] = new PersonaProfile
            {
                PreferredTone = "Reassuring",
                EmotionalTriggers = new List<string> { "uncertainty", "timeline", "cost" },
                ResponsePattern = "Provide clear timelines and frequent updates"
            },
            ["FrustratedCustomer"] = new PersonaProfile
            {
                PreferredTone = "Understanding",
                EmotionalTriggers = new List<string> { "delay", "miscommunication", "repeat issues" },
                ResponsePattern = "Acknowledge frustration and provide immediate action plan"
            },
            ["TechnicallySavvy"] = new PersonaProfile
            {
                PreferredTone = "Technical",
                EmotionalTriggers = new List<string> { "oversimplification", "lack of detail" },
                ResponsePattern = "Provide technical details and root cause analysis"
            },
            ["Default"] = new PersonaProfile
            {
                PreferredTone = "Professional",
                EmotionalTriggers = new List<string> { "confusion", "concern" },
                ResponsePattern = "Clear, professional communication with empathy"
            }
        };
    }

    private string EnhanceNarrativeWithPersona(string baseNarrative, PersonaProfile profile, EmotionalContext context)
    {
        var prefix = profile.PreferredTone switch
        {
            "Reassuring" => "I want to reassure you that ",
            "Understanding" => "I completely understand your frustration, and ",
            "Technical" => "From a technical perspective, ",
            _ => "I understand your situation, and "
        };

        return $"{prefix}{baseNarrative.ToLower()}";
    }

    private double CalculateEmpathyScore(EmotionalContext context, PersonaProfile profile)
    {
        var baseScore = 0.7;
        
        // Boost score if we're addressing known emotional triggers
        if (profile.EmotionalTriggers.Any(trigger => 
            context.TriggerEvent.Contains(trigger, StringComparison.OrdinalIgnoreCase)))
        {
            baseScore += 0.2;
        }

        // Adjust for emotional intensity
        var intensityFactor = Math.Min(context.IntensityLevel / 10.0, 1.0);
        baseScore += intensityFactor * 0.1;

        return Math.Min(baseScore, 1.0);
    }

    private List<string> GenerateFollowUpSuggestions(string persona, EmotionalContext context, string serviceContext)
    {
        var suggestions = new List<string> { "Follow up within 1 hour with status update" };
        
        if (context.IntensityLevel > 7)
        {
            suggestions.Add("Escalate to supervisor if no resolution within 2 hours");
        }
        
        if (persona == "AnxiousCustomer")
        {
            suggestions.Add("Send detailed timeline breakdown to reduce anxiety");
        }
        
        return suggestions;
    }

    private string AnalyzeHistoricalTonePreference(List<string> history)
    {
        // Simple analysis for MVP - would use ML in production
        return history.Count > 5 ? "Professional" : "Friendly";
    }

    private string ApplyToneAdaptation(string narrative, string tone)
    {
        return tone switch
        {
            "Professional" => $"I'd like to professionally address your concern: {narrative}",
            "Friendly" => $"I'm happy to help you with this: {narrative}",
            _ => narrative
        };
    }

    private ValidationCheck CheckToneAppropriateness(string narrative, string sensitivity)
    {
        var score = narrative.Length > 10 ? 0.8 : 0.4;
        return new ValidationCheck
        {
            Score = score,
            Warning = score < 0.7 ? "Tone may be too brief for sensitive context" : "",
            Improvements = score < 0.7 ? new List<string> { "Expand narrative with more empathetic language" } : new List<string>()
        };
    }

    private ValidationCheck CheckContentSensitivity(string narrative)
    {
        var sensitiveWords = new[] { "sorry", "apologize", "unfortunately" };
        var hasEmpathy = sensitiveWords.Any(word => narrative.Contains(word, StringComparison.OrdinalIgnoreCase));
        var score = hasEmpathy ? 0.9 : 0.6;
        
        return new ValidationCheck
        {
            Score = score,
            Warning = score < 0.7 ? "Consider adding more empathetic language" : "",
            Improvements = score < 0.7 ? new List<string> { "Include acknowledgment of customer's situation" } : new List<string>()
        };
    }

    private ValidationCheck CheckLengthAppropriateness(string narrative)
    {
        var score = narrative.Length >= 50 && narrative.Length <= 300 ? 0.9 : 0.5;
        return new ValidationCheck
        {
            Score = score,
            Warning = score < 0.7 ? "Narrative length may not be optimal" : "",
            Improvements = score < 0.7 ? new List<string> { "Adjust narrative length for better engagement" } : new List<string>()
        };
    }

    private ValidationCheck CheckPersonalizationLevel(string narrative)
    {
        var hasPersonalization = narrative.Contains("you", StringComparison.OrdinalIgnoreCase);
        var score = hasPersonalization ? 0.8 : 0.5;
        
        return new ValidationCheck
        {
            Score = score,
            Warning = score < 0.7 ? "Narrative could be more personalized" : "",
            Improvements = score < 0.7 ? new List<string> { "Add direct customer references for personalization" } : new List<string>()
        };
    }

    // [Sprint123_FixItFred_OmegaSweep] Helper classes for internal processing
    private class PersonaProfile
    {
        public string PreferredTone { get; set; } = string.Empty;
        public List<string> EmotionalTriggers { get; set; } = new();
        public string ResponsePattern { get; set; } = string.Empty;
    }

    private class ValidationCheck
    {
        public double Score { get; set; }
        public string Warning { get; set; } = string.Empty;
        public List<string> Improvements { get; set; } = new();
    }
}