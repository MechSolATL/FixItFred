using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Data;
using Interfaces;
using System.Text.RegularExpressions;

namespace Services
{
    /// <summary>
    /// [Sprint126_OneScan_51-70] Smart Prefill + Empathy Bindings Service
    /// Extends LyraEmpathyNarrator with smart form prefill and role-based logic
    /// Supports voice input: "Lyra, find Jenkins job from 2022"
    /// Role-based filtering: Tech, Dispatcher, Manager, Admin with LyraPublicContext
    /// </summary>
    public class LyraSmartPrefillService
    {
        private readonly ILogger<LyraSmartPrefillService> _logger;
        private readonly ApplicationDbContext _db;
        private readonly ILyraEmpathyNarrator _empathyNarrator;
        private readonly ILyraCognition _lyraCognition;

        // [Sprint126_OneScan_51-70] Role-based access levels
        public enum UserRole
        {
            Tech,
            Dispatcher,
            Manager,
            Admin,
            Public
        }

        // [Sprint126_OneScan_51-70] Smart prefill contexts
        public enum PrefillContext
        {
            ServiceRequest,
            TechnicianAssignment,
            CustomerInteraction,
            DispatcherQueue,
            ReportGeneration,
            EmpathyResponse,
            VoiceSearch,
            GeneralQuery
        }

        public LyraSmartPrefillService(
            ILogger<LyraSmartPrefillService> logger,
            ApplicationDbContext db,
            ILyraEmpathyNarrator empathyNarrator,
            ILyraCognition lyraCognition)
        {
            _logger = logger;
            _db = db;
            _empathyNarrator = empathyNarrator;
            _lyraCognition = lyraCognition;
        }

        /// <summary>
        /// [Sprint126_OneScan_51-70] Processes voice input for smart search and prefill
        /// Handles queries like: "Lyra, find Jenkins job from 2022"
        /// </summary>
        /// <param name="voiceInput">Raw voice input text</param>
        /// <param name="userRole">User's role for context filtering</param>
        /// <param name="currentContext">Current application context</param>
        /// <returns>Smart prefill result with empathy integration</returns>
        public async Task<SmartPrefillResult> ProcessVoiceInputAsync(string voiceInput, UserRole userRole, string currentContext = "")
        {
            _logger.LogInformation("[Sprint126_OneScan_51-70] Processing voice input: {Input} for role: {Role}", 
                voiceInput, userRole);

            try
            {
                // [Sprint126_OneScan_51-70] Parse voice input for intent and entities
                var parsedInput = await ParseVoiceInputAsync(voiceInput);
                
                // [Sprint126_OneScan_51-70] Apply role-based filtering
                var filteredResults = await ApplyRoleBasedFilteringAsync(parsedInput, userRole);
                
                // [Sprint126_OneScan_51-70] Generate smart prefill suggestions
                var prefillSuggestions = await GeneratePrefillSuggestionsAsync(filteredResults, currentContext);
                
                // [Sprint126_OneScan_51-70] Create empathy binding if appropriate
                var empathyBinding = await CreateEmpathyBindingAsync(parsedInput, userRole, currentContext);

                var result = new SmartPrefillResult
                {
                    Id = Guid.NewGuid(),
                    OriginalVoiceInput = voiceInput,
                    ParsedIntent = parsedInput.Intent,
                    ExtractedEntities = parsedInput.Entities,
                    UserRole = userRole,
                    PrefillSuggestions = prefillSuggestions,
                    EmpathyBinding = empathyBinding,
                    ConfidenceScore = parsedInput.ConfidenceScore,
                    Context = currentContext,
                    Timestamp = DateTime.UtcNow,
                    IsPublicContext = IsPublicContext(userRole, currentContext)
                };

                _logger.LogInformation("[Sprint126_OneScan_51-70] Generated smart prefill result {ResultId} with {Count} suggestions", 
                    result.Id, prefillSuggestions.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint126_OneScan_51-70] Error processing voice input: {Input}", voiceInput);
                
                // [Sprint126_OneScan_51-70] Return fallback result
                return new SmartPrefillResult
                {
                    Id = Guid.NewGuid(),
                    OriginalVoiceInput = voiceInput,
                    ParsedIntent = "unknown",
                    UserRole = userRole,
                    PrefillSuggestions = new List<PrefillSuggestion>(),
                    EmpathyBinding = null,
                    ConfidenceScore = 0.0,
                    Context = "error",
                    Timestamp = DateTime.UtcNow,
                    IsPublicContext = true
                };
            }
        }

        /// <summary>
        /// [Sprint126_OneScan_51-70] Parses voice input to extract intent and entities
        /// </summary>
        private async Task<VoiceInputParseResult> ParseVoiceInputAsync(string voiceInput)
        {
            var input = voiceInput.ToLower().Trim();
            
            // [Sprint126_OneScan_51-70] Intent classification patterns
            var intentPatterns = new Dictionary<string, string[]>
            {
                ["search"] = new[] { "find", "search", "look for", "show me", "get", "retrieve" },
                ["create"] = new[] { "create", "make", "new", "add", "start" },
                ["update"] = new[] { "update", "change", "modify", "edit", "fix" },
                ["delete"] = new[] { "delete", "remove", "cancel", "clear" },
                ["assign"] = new[] { "assign", "send", "dispatch", "route" },
                ["status"] = new[] { "status", "check", "monitor", "track" },
                ["report"] = new[] { "report", "summary", "analytics", "stats" },
                ["help"] = new[] { "help", "assist", "guide", "explain" }
            };

            var intent = "unknown";
            var confidenceScore = 0.0;

            // Find best matching intent
            foreach (var pattern in intentPatterns)
            {
                var matchCount = pattern.Value.Count(keyword => input.Contains(keyword));
                if (matchCount > 0)
                {
                    var score = (double)matchCount / pattern.Value.Length;
                    if (score > confidenceScore)
                    {
                        intent = pattern.Key;
                        confidenceScore = score;
                    }
                }
            }

            // [Sprint126_OneScan_51-70] Entity extraction
            var entities = await ExtractEntitiesAsync(input);

            return new VoiceInputParseResult
            {
                Intent = intent,
                Entities = entities,
                ConfidenceScore = Math.Min(confidenceScore, 1.0),
                ProcessedText = input
            };
        }

        /// <summary>
        /// [Sprint126_OneScan_51-70] Extracts entities from voice input
        /// </summary>
        private async Task<Dictionary<string, string>> ExtractEntitiesAsync(string input)
        {
            var entities = new Dictionary<string, string>();

            // [Sprint126_OneScan_51-70] Date/time extraction
            var datePatterns = new Dictionary<string, string>
            {
                ["today"] = DateTime.Today.ToString("yyyy-MM-dd"),
                ["yesterday"] = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
                ["this week"] = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).ToString("yyyy-MM-dd"),
                ["last week"] = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek - 7).ToString("yyyy-MM-dd"),
                ["this month"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd"),
                ["last month"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1).ToString("yyyy-MM-dd")
            };

            foreach (var pattern in datePatterns)
            {
                if (input.Contains(pattern.Key))
                {
                    entities["date"] = pattern.Value;
                    break;
                }
            }

            // Year extraction
            var yearMatch = Regex.Match(input, @"\b(20\d{2})\b");
            if (yearMatch.Success)
            {
                entities["year"] = yearMatch.Value;
            }

            // [Sprint126_OneScan_51-70] Entity type extraction
            var entityTypes = new Dictionary<string, string[]>
            {
                ["technician"] = new[] { "technician", "tech", "worker", "employee" },
                ["customer"] = new[] { "customer", "client", "user" },
                ["job"] = new[] { "job", "request", "service", "work order", "ticket" },
                ["dispatcher"] = new[] { "dispatcher", "dispatch", "coordinator" },
                ["manager"] = new[] { "manager", "supervisor", "admin" },
                ["report"] = new[] { "report", "analytics", "summary", "stats" },
                ["jenkins"] = new[] { "jenkins", "build", "deployment", "ci/cd" },
                ["schedule"] = new[] { "schedule", "calendar", "appointment", "booking" }
            };

            foreach (var entityType in entityTypes)
            {
                if (entityType.Value.Any(keyword => input.Contains(keyword)))
                {
                    entities["type"] = entityType.Key;
                    break;
                }
            }

            // [Sprint126_OneScan_51-70] Status extraction
            var statusTypes = new[] { "pending", "completed", "active", "cancelled", "failed", "success" };
            var foundStatus = statusTypes.FirstOrDefault(status => input.Contains(status));
            if (foundStatus != null)
            {
                entities["status"] = foundStatus;
            }

            return entities;
        }

        /// <summary>
        /// [Sprint126_OneScan_51-70] Applies role-based filtering to search results
        /// </summary>
        private async Task<FilteredSearchResults> ApplyRoleBasedFilteringAsync(VoiceInputParseResult parsedInput, UserRole userRole)
        {
            var results = new FilteredSearchResults
            {
                AllowedContexts = GetAllowedContextsForRole(userRole),
                FilteredEntities = parsedInput.Entities.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                AccessLevel = GetAccessLevelForRole(userRole)
            };

            // [Sprint126_OneScan_51-70] Filter sensitive data based on role
            switch (userRole)
            {
                case UserRole.Public:
                    // Remove sensitive entities for public access
                    results.FilteredEntities.Remove("technician");
                    results.FilteredEntities.Remove("jenkins");
                    results.FilteredEntities.Remove("dispatcher");
                    break;

                case UserRole.Tech:
                    // Tech can see own data and assigned jobs
                    results.FilteredEntities["scope"] = "assigned_only";
                    break;

                case UserRole.Dispatcher:
                    // Dispatcher can see regional data
                    results.FilteredEntities["scope"] = "regional";
                    break;

                case UserRole.Manager:
                case UserRole.Admin:
                    // Full access
                    results.FilteredEntities["scope"] = "full";
                    break;
            }

            _logger.LogDebug("[Sprint126_OneScan_51-70] Applied role-based filtering for {Role}, access level: {Level}", 
                userRole, results.AccessLevel);

            return results;
        }

        /// <summary>
        /// [Sprint126_OneScan_51-70] Generates smart prefill suggestions based on context
        /// </summary>
        private async Task<List<PrefillSuggestion>> GeneratePrefillSuggestionsAsync(FilteredSearchResults filteredResults, string currentContext)
        {
            var suggestions = new List<PrefillSuggestion>();

            try
            {
                // [Sprint126_OneScan_51-70] Generate suggestions based on entities and context
                if (filteredResults.FilteredEntities.ContainsKey("type"))
                {
                    var entityType = filteredResults.FilteredEntities["type"];
                    suggestions.AddRange(await GetEntitySpecificSuggestionsAsync(entityType, filteredResults));
                }

                // [Sprint126_OneScan_51-70] Add context-specific suggestions
                suggestions.AddRange(await GetContextSpecificSuggestionsAsync(currentContext, filteredResults));

                // [Sprint126_OneScan_51-70] Add recent items for quick access
                suggestions.AddRange(await GetRecentItemSuggestionsAsync(filteredResults.AccessLevel));

                // [Sprint126_OneScan_51-70] Sort by relevance and confidence
                suggestions = suggestions
                    .OrderByDescending(s => s.ConfidenceScore)
                    .ThenByDescending(s => s.RelevanceScore)
                    .Take(10)
                    .ToList();

                return suggestions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint126_OneScan_51-70] Error generating prefill suggestions");
                return new List<PrefillSuggestion>();
            }
        }

        /// <summary>
        /// [Sprint126_OneScan_51-70] Creates empathy binding for emotional context
        /// </summary>
        private async Task<EmpathyBinding?> CreateEmpathyBindingAsync(VoiceInputParseResult parsedInput, UserRole userRole, string currentContext)
        {
            try
            {
                // [Sprint126_OneScan_51-70] Detect emotional indicators in voice input
                var emotionalIndicators = new[]
                {
                    "urgent", "critical", "emergency", "frustrated", "confused", 
                    "help", "problem", "issue", "trouble", "stuck"
                };

                var hasEmotionalContext = emotionalIndicators.Any(indicator => 
                    parsedInput.ProcessedText.Contains(indicator));

                if (!hasEmotionalContext)
                {
                    return null;
                }

                // [Sprint126_OneScan_51-70] Create emotional context
                var emotionalContext = new EmotionalContext
                {
                    PrimaryEmotion = DetectPrimaryEmotion(parsedInput.ProcessedText),
                    IntensityLevel = CalculateEmotionalIntensity(parsedInput.ProcessedText),
                    TriggerEvent = parsedInput.ProcessedText,
                    EmotionalDuration = TimeSpan.FromMinutes(5),
                    RequiresImmediateAttention = userRole == UserRole.Public && 
                        parsedInput.ProcessedText.Contains("emergency")
                };

                // [Sprint126_OneScan_51-70] Generate empathy response
                var empathyResult = await _empathyNarrator.GenerateEmpathyNarrativeAsync(
                    DetectPersona(parsedInput.ProcessedText),
                    emotionalContext,
                    currentContext
                );

                return new EmpathyBinding
                {
                    Id = Guid.NewGuid(),
                    EmotionalContext = emotionalContext,
                    EmpathyNarrative = empathyResult,
                    SuggestedActions = GenerateEmpathySuggestedActions(emotionalContext),
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint126_OneScan_51-70] Error creating empathy binding");
                return null;
            }
        }

        // [Sprint126_OneScan_51-70] Helper methods

        private List<PrefillContext> GetAllowedContextsForRole(UserRole role)
        {
            return role switch
            {
                UserRole.Public => new List<PrefillContext> { PrefillContext.ServiceRequest, PrefillContext.GeneralQuery },
                UserRole.Tech => new List<PrefillContext> { PrefillContext.ServiceRequest, PrefillContext.CustomerInteraction, PrefillContext.ReportGeneration },
                UserRole.Dispatcher => new List<PrefillContext> { PrefillContext.TechnicianAssignment, PrefillContext.DispatcherQueue, PrefillContext.ReportGeneration },
                UserRole.Manager => Enum.GetValues<PrefillContext>().ToList(),
                UserRole.Admin => Enum.GetValues<PrefillContext>().ToList(),
                _ => new List<PrefillContext> { PrefillContext.GeneralQuery }
            };
        }

        private string GetAccessLevelForRole(UserRole role)
        {
            return role switch
            {
                UserRole.Public => "restricted",
                UserRole.Tech => "limited",
                UserRole.Dispatcher => "regional",
                UserRole.Manager => "full",
                UserRole.Admin => "full",
                _ => "restricted"
            };
        }

        private bool IsPublicContext(UserRole role, string context)
        {
            return role == UserRole.Public || context.Contains("public", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<List<PrefillSuggestion>> GetEntitySpecificSuggestionsAsync(string entityType, FilteredSearchResults filteredResults)
        {
            var suggestions = new List<PrefillSuggestion>();

            switch (entityType)
            {
                case "technician":
                    suggestions.Add(new PrefillSuggestion
                    {
                        FieldName = "TechnicianId",
                        SuggestedValue = "Recent technicians",
                        DisplayText = "Recent technician assignments",
                        ConfidenceScore = 0.8,
                        RelevanceScore = 0.9,
                        Source = "recent_activity"
                    });
                    break;

                case "customer":
                    suggestions.Add(new PrefillSuggestion
                    {
                        FieldName = "CustomerId",
                        SuggestedValue = "Recent customers",
                        DisplayText = "Recent customer interactions",
                        ConfidenceScore = 0.7,
                        RelevanceScore = 0.8,
                        Source = "recent_activity"
                    });
                    break;

                case "job":
                    suggestions.Add(new PrefillSuggestion
                    {
                        FieldName = "ServiceRequestId",
                        SuggestedValue = "Recent jobs",
                        DisplayText = "Recent service requests",
                        ConfidenceScore = 0.9,
                        RelevanceScore = 0.95,
                        Source = "job_history"
                    });
                    break;
            }

            return suggestions;
        }

        private async Task<List<PrefillSuggestion>> GetContextSpecificSuggestionsAsync(string context, FilteredSearchResults filteredResults)
        {
            var suggestions = new List<PrefillSuggestion>();

            if (context.Contains("dashboard", StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add(new PrefillSuggestion
                {
                    FieldName = "SearchQuery",
                    SuggestedValue = "dashboard metrics",
                    DisplayText = "Show dashboard metrics",
                    ConfidenceScore = 0.85,
                    RelevanceScore = 0.8,
                    Source = "context_aware"
                });
            }

            return suggestions;
        }

        private async Task<List<PrefillSuggestion>> GetRecentItemSuggestionsAsync(string accessLevel)
        {
            var suggestions = new List<PrefillSuggestion>();

            // Mock recent items based on access level
            if (accessLevel != "restricted")
            {
                suggestions.Add(new PrefillSuggestion
                {
                    FieldName = "QuickAction",
                    SuggestedValue = "recent_items",
                    DisplayText = "Show recent activity",
                    ConfidenceScore = 0.6,
                    RelevanceScore = 0.7,
                    Source = "recent_activity"
                });
            }

            return suggestions;
        }

        private string DetectPrimaryEmotion(string text)
        {
            if (text.Contains("urgent") || text.Contains("emergency")) return "Anxiety";
            if (text.Contains("frustrated") || text.Contains("trouble")) return "Frustration";
            if (text.Contains("confused") || text.Contains("help")) return "Confusion";
            return "Neutral";
        }

        private int CalculateEmotionalIntensity(string text)
        {
            var intensityWords = new[] { "very", "extremely", "really", "super", "urgent", "critical", "emergency" };
            var count = intensityWords.Count(word => text.Contains(word));
            return Math.Min(count * 2 + 3, 10); // Scale 1-10
        }

        private string DetectPersona(string text)
        {
            if (text.Contains("technical") || text.Contains("details")) return "TechnicallySavvy";
            if (text.Contains("urgent") || text.Contains("emergency")) return "AnxiousCustomer";
            if (text.Contains("frustrated") || text.Contains("problem")) return "FrustratedCustomer";
            return "DefaultCustomer";
        }

        private List<string> GenerateEmpathySuggestedActions(EmotionalContext context)
        {
            var actions = new List<string>();

            if (context.IntensityLevel > 7)
            {
                actions.Add("Escalate to supervisor immediately");
                actions.Add("Provide direct contact information");
            }

            if (context.PrimaryEmotion == "Anxiety")
            {
                actions.Add("Provide detailed timeline and updates");
                actions.Add("Offer frequent status checks");
            }

            if (context.PrimaryEmotion == "Frustration")
            {
                actions.Add("Acknowledge frustration and apologize");
                actions.Add("Provide immediate action plan");
            }

            actions.Add("Follow up within 1 hour");

            return actions;
        }
    }

    // [Sprint126_OneScan_51-70] Supporting models

    public class SmartPrefillResult
    {
        public Guid Id { get; set; }
        public string OriginalVoiceInput { get; set; } = string.Empty;
        public string ParsedIntent { get; set; } = string.Empty;
        public Dictionary<string, string> ExtractedEntities { get; set; } = new();
        public LyraSmartPrefillService.UserRole UserRole { get; set; }
        public List<PrefillSuggestion> PrefillSuggestions { get; set; } = new();
        public EmpathyBinding? EmpathyBinding { get; set; }
        public double ConfidenceScore { get; set; }
        public string Context { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsPublicContext { get; set; }
    }

    public class VoiceInputParseResult
    {
        public string Intent { get; set; } = string.Empty;
        public Dictionary<string, string> Entities { get; set; } = new();
        public double ConfidenceScore { get; set; }
        public string ProcessedText { get; set; } = string.Empty;
    }

    public class FilteredSearchResults
    {
        public List<LyraSmartPrefillService.PrefillContext> AllowedContexts { get; set; } = new();
        public Dictionary<string, string> FilteredEntities { get; set; } = new();
        public string AccessLevel { get; set; } = string.Empty;
    }

    public class PrefillSuggestion
    {
        public string FieldName { get; set; } = string.Empty;
        public string SuggestedValue { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public double RelevanceScore { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class EmpathyBinding
    {
        public Guid Id { get; set; }
        public EmotionalContext EmotionalContext { get; set; } = new();
        public EmpathyNarrativeResult EmpathyNarrative { get; set; } = new();
        public List<string> SuggestedActions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}