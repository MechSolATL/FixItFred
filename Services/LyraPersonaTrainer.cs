using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Learns behavior per org/user cluster and adjusts overlays and tips based on:
    /// Workflow patterns, Task order, Frequency, Delay patterns
    /// </summary>
    public class LyraPersonaTrainer
    {
        private readonly UserPerformanceLevelEngine _performanceEngine;
        private readonly LyraTipRegistry _tipRegistry;
        private readonly Dictionary<string, OrganizationBehaviorProfile> _orgProfiles;
        private readonly Dictionary<int, UserBehaviorProfile> _userProfiles;

        public LyraPersonaTrainer(UserPerformanceLevelEngine performanceEngine, LyraTipRegistry tipRegistry)
        {
            _performanceEngine = performanceEngine;
            _tipRegistry = tipRegistry;
            _orgProfiles = new Dictionary<string, OrganizationBehaviorProfile>();
            _userProfiles = new Dictionary<int, UserBehaviorProfile>();
        }

        /// <summary>
        /// Records user workflow behavior for learning
        /// </summary>
        public async Task RecordWorkflowBehaviorAsync(int userId, string organizationId, WorkflowBehaviorEntry entry)
        {
            // Update user profile
            var userProfile = await GetOrCreateUserProfileAsync(userId, organizationId);
            userProfile.WorkflowEntries.Add(entry);
            userProfile.LastActivityDate = DateTime.UtcNow;
            
            // Update organization profile
            var orgProfile = await GetOrCreateOrgProfileAsync(organizationId);
            orgProfile.AggregateWorkflowEntries.Add(entry);
            orgProfile.LastUpdated = DateTime.UtcNow;

            // Analyze patterns if we have enough data
            if (userProfile.WorkflowEntries.Count > 20)
            {
                await AnalyzeUserPatternsAsync(userProfile);
            }

            if (orgProfile.AggregateWorkflowEntries.Count > 100)
            {
                await AnalyzeOrganizationPatternsAsync(orgProfile);
            }
        }

        /// <summary>
        /// Gets personalized recommendations for a user
        /// </summary>
        public async Task<PersonalizationRecommendations> GetPersonalizationRecommendationsAsync(int userId)
        {
            var userProfile = _userProfiles.GetValueOrDefault(userId);
            if (userProfile == null)
            {
                return GetDefaultRecommendations();
            }

            var orgProfile = _orgProfiles.GetValueOrDefault(userProfile.OrganizationId);
            var performanceLevel = await _performanceEngine.GetUserPerformanceLevelAsync(userId);

            var recommendations = new PersonalizationRecommendations
            {
                UserId = userId,
                PerformanceLevel = performanceLevel,
                PreferredWorkflowOrder = userProfile.PreferredTaskOrder,
                OptimalTipTiming = userProfile.OptimalTipTiming,
                SuggestedShortcuts = GetRecommendedShortcuts(userProfile, performanceLevel),
                CustomOverlaySettings = GetCustomOverlaySettings(userProfile, orgProfile),
                PredictedNextActions = PredictNextActions(userProfile),
                AdaptationStrength = CalculateAdaptationStrength(userProfile)
            };

            return recommendations;
        }

        /// <summary>
        /// Gets organization-wide behavior insights
        /// </summary>
        public async Task<OrganizationInsights> GetOrganizationInsightsAsync(string organizationId)
        {
            var orgProfile = _orgProfiles.GetValueOrDefault(organizationId);
            if (orgProfile == null)
            {
                return new OrganizationInsights { OrganizationId = organizationId };
            }

            var insights = new OrganizationInsights
            {
                OrganizationId = organizationId,
                CommonWorkflowPatterns = orgProfile.CommonWorkflowPatterns,
                PeakActivityTimes = orgProfile.PeakActivityTimes,
                PreferredTaskSequences = orgProfile.PreferredTaskSequences,
                AverageTaskCompletionTimes = orgProfile.AverageTaskCompletionTimes,
                ErrorPatterns = orgProfile.CommonErrorPatterns,
                EfficiencyMetrics = CalculateEfficiencyMetrics(orgProfile),
                RecommendedOptimizations = GenerateOptimizationRecommendations(orgProfile)
            };

            return await Task.FromResult(insights);
        }

        /// <summary>
        /// Adjusts system behavior based on learned patterns
        /// </summary>
        public async Task<AdaptationSettings> GetAdaptedSettingsAsync(int userId, string context)
        {
            var userProfile = _userProfiles.GetValueOrDefault(userId);
            var performanceLevel = await _performanceEngine.GetUserPerformanceLevelAsync(userId);

            if (userProfile == null)
            {
                return GetDefaultSettings(performanceLevel, context);
            }

            var settings = new AdaptationSettings
            {
                ShowTips = ShouldShowTips(userProfile, performanceLevel, context),
                TipDelay = CalculateOptimalTipDelay(userProfile, context),
                ShortcutsEnabled = ShouldEnableShortcuts(userProfile, performanceLevel),
                OverlayIntensity = CalculateOverlayIntensity(userProfile, performanceLevel),
                WorkflowSuggestions = GetWorkflowSuggestions(userProfile, context),
                PredictiveActions = GetPredictiveActions(userProfile, context)
            };

            return settings;
        }

        /// <summary>
        /// Forces relearning for a user or organization
        /// </summary>
        public async Task ResetLearningAsync(int? userId = null, string? organizationId = null)
        {
            if (userId.HasValue)
            {
                _userProfiles.Remove(userId.Value);
            }

            if (!string.IsNullOrEmpty(organizationId))
            {
                _orgProfiles.Remove(organizationId);
            }

            await Task.CompletedTask;
        }

        private async Task<UserBehaviorProfile> GetOrCreateUserProfileAsync(int userId, string organizationId)
        {
            if (!_userProfiles.ContainsKey(userId))
            {
                _userProfiles[userId] = new UserBehaviorProfile
                {
                    UserId = userId,
                    OrganizationId = organizationId,
                    FirstActivityDate = DateTime.UtcNow,
                    WorkflowEntries = new List<WorkflowBehaviorEntry>()
                };
            }

            return await Task.FromResult(_userProfiles[userId]);
        }

        private async Task<OrganizationBehaviorProfile> GetOrCreateOrgProfileAsync(string organizationId)
        {
            if (!_orgProfiles.ContainsKey(organizationId))
            {
                _orgProfiles[organizationId] = new OrganizationBehaviorProfile
                {
                    OrganizationId = organizationId,
                    AggregateWorkflowEntries = new List<WorkflowBehaviorEntry>(),
                    CommonWorkflowPatterns = new List<string>(),
                    PeakActivityTimes = new Dictionary<string, int>(),
                    PreferredTaskSequences = new List<string>(),
                    AverageTaskCompletionTimes = new Dictionary<string, double>(),
                    CommonErrorPatterns = new List<string>()
                };
            }

            return await Task.FromResult(_orgProfiles[organizationId]);
        }

        private async Task AnalyzeUserPatternsAsync(UserBehaviorProfile profile)
        {
            var recentEntries = profile.WorkflowEntries
                .Where(x => x.Timestamp > DateTime.UtcNow.AddDays(-30))
                .OrderBy(x => x.Timestamp)
                .ToList();

            // Analyze task order preferences
            profile.PreferredTaskOrder = AnalyzeTaskOrderPreferences(recentEntries);
            
            // Analyze timing patterns
            profile.OptimalTipTiming = AnalyzeOptimalTipTiming(recentEntries);
            
            // Analyze delay patterns
            profile.AverageDelayBetweenActions = CalculateAverageDelayBetweenActions(recentEntries);

            await Task.CompletedTask;
        }

        private async Task AnalyzeOrganizationPatternsAsync(OrganizationBehaviorProfile profile)
        {
            var recentEntries = profile.AggregateWorkflowEntries
                .Where(x => x.Timestamp > DateTime.UtcNow.AddDays(-30))
                .ToList();

            // Analyze common workflow patterns
            profile.CommonWorkflowPatterns = AnalyzeCommonWorkflowPatterns(recentEntries);
            
            // Analyze peak activity times
            profile.PeakActivityTimes = AnalyzePeakActivityTimes(recentEntries);
            
            // Analyze task completion times
            profile.AverageTaskCompletionTimes = CalculateAverageTaskCompletionTimes(recentEntries);

            await Task.CompletedTask;
        }

        // Helper methods for analysis
        private List<string> AnalyzeTaskOrderPreferences(List<WorkflowBehaviorEntry> entries)
        {
            return entries
                .GroupBy(x => x.TaskType)
                .OrderBy(g => g.Average(x => x.SequenceOrder))
                .Select(g => g.Key)
                .ToList();
        }

        private int AnalyzeOptimalTipTiming(List<WorkflowBehaviorEntry> entries)
        {
            var delaysWhenTipsShown = entries
                .Where(x => x.TipShown)
                .Select(x => x.DelayBeforeAction)
                .ToList();

            return delaysWhenTipsShown.Any() ? (int)delaysWhenTipsShown.Average() : 2000;
        }

        private double CalculateAverageDelayBetweenActions(List<WorkflowBehaviorEntry> entries)
        {
            return entries.Any() ? entries.Average(x => x.DelayBeforeAction) : 1000;
        }

        private List<string> AnalyzeCommonWorkflowPatterns(List<WorkflowBehaviorEntry> entries)
        {
            return entries
                .GroupBy(x => x.WorkflowPattern)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();
        }

        private Dictionary<string, int> AnalyzePeakActivityTimes(List<WorkflowBehaviorEntry> entries)
        {
            return entries
                .GroupBy(x => x.Timestamp.Hour.ToString())
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private Dictionary<string, double> CalculateAverageTaskCompletionTimes(List<WorkflowBehaviorEntry> entries)
        {
            return entries
                .Where(x => x.TaskCompletionTime > 0)
                .GroupBy(x => x.TaskType)
                .ToDictionary(g => g.Key, g => g.Average(x => x.TaskCompletionTime));
        }

        private PersonalizationRecommendations GetDefaultRecommendations()
        {
            return new PersonalizationRecommendations
            {
                PerformanceLevel = "Beginner",
                PreferredWorkflowOrder = new List<string>(),
                OptimalTipTiming = 3000,
                SuggestedShortcuts = new List<string>(),
                CustomOverlaySettings = new Dictionary<string, object>(),
                PredictedNextActions = new List<string>(),
                AdaptationStrength = 0.1
            };
        }

        private List<string> GetRecommendedShortcuts(UserBehaviorProfile profile, string performanceLevel)
        {
            var shortcuts = new List<string>();
            
            if (performanceLevel == "Intermediate" || performanceLevel == "Expert")
            {
                shortcuts.Add("keyboard_navigation");
                shortcuts.Add("quick_actions");
            }
            
            if (performanceLevel == "Expert")
            {
                shortcuts.Add("advanced_workflows");
                shortcuts.Add("bulk_operations");
            }

            return shortcuts;
        }

        private Dictionary<string, object> GetCustomOverlaySettings(UserBehaviorProfile? userProfile, OrganizationBehaviorProfile? orgProfile)
        {
            var settings = new Dictionary<string, object>
            {
                { "overlay_opacity", 0.8 },
                { "animation_speed", "normal" },
                { "highlight_color", "#007bff" }
            };

            if (userProfile != null)
            {
                settings["preferred_timing"] = userProfile.OptimalTipTiming;
            }

            return settings;
        }

        private List<string> PredictNextActions(UserBehaviorProfile profile)
        {
            var recentPatterns = profile.WorkflowEntries
                .TakeLast(10)
                .Select(x => x.TaskType)
                .ToList();

            // Simple prediction based on recent patterns
            return recentPatterns.Distinct().ToList();
        }

        private double CalculateAdaptationStrength(UserBehaviorProfile profile)
        {
            var entryCount = profile.WorkflowEntries.Count;
            return Math.Min(1.0, entryCount / 100.0); // Max adaptation at 100 entries
        }

        private AdaptationSettings GetDefaultSettings(string performanceLevel, string context)
        {
            return new AdaptationSettings
            {
                ShowTips = performanceLevel == "Beginner",
                TipDelay = performanceLevel == "Beginner" ? 1000 : 3000,
                ShortcutsEnabled = performanceLevel != "Beginner",
                OverlayIntensity = performanceLevel == "Beginner" ? 1.0 : 0.5,
                WorkflowSuggestions = new List<string>(),
                PredictiveActions = new List<string>()
            };
        }

        private bool ShouldShowTips(UserBehaviorProfile profile, string performanceLevel, string context)
        {
            if (performanceLevel == "Expert") return false;
            if (performanceLevel == "Beginner") return true;
            
            // For intermediate users, show tips in new contexts
            var hasExperienceInContext = profile.WorkflowEntries
                .Any(x => x.PageContext == context && x.TaskCompleted);
            
            return !hasExperienceInContext;
        }

        private int CalculateOptimalTipDelay(UserBehaviorProfile profile, string context)
        {
            var contextEntries = profile.WorkflowEntries
                .Where(x => x.PageContext == context)
                .ToList();

            if (!contextEntries.Any())
                return profile.OptimalTipTiming;

            return (int)contextEntries.Average(x => x.DelayBeforeAction);
        }

        private bool ShouldEnableShortcuts(UserBehaviorProfile profile, string performanceLevel)
        {
            return performanceLevel != "Beginner" && 
                   profile.WorkflowEntries.Count(x => x.UsedShortcut) > 5;
        }

        private double CalculateOverlayIntensity(UserBehaviorProfile profile, string performanceLevel)
        {
            return performanceLevel switch
            {
                "Beginner" => 1.0,
                "Intermediate" => 0.6,
                "Expert" => 0.3,
                _ => 0.8
            };
        }

        private List<string> GetWorkflowSuggestions(UserBehaviorProfile profile, string context)
        {
            return profile.PreferredTaskOrder
                .Where(task => !profile.WorkflowEntries
                    .Any(x => x.PageContext == context && x.TaskType == task && x.TaskCompleted))
                .ToList();
        }

        private List<string> GetPredictiveActions(UserBehaviorProfile profile, string context)
        {
            var contextPatterns = profile.WorkflowEntries
                .Where(x => x.PageContext == context)
                .GroupBy(x => x.TaskType)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(3)
                .ToList();

            return contextPatterns;
        }

        private Dictionary<string, double> CalculateEfficiencyMetrics(OrganizationBehaviorProfile profile)
        {
            return new Dictionary<string, double>
            {
                { "average_task_completion", profile.AverageTaskCompletionTimes.Values.Any() ? profile.AverageTaskCompletionTimes.Values.Average() : 0 },
                { "workflow_efficiency", CalculateWorkflowEfficiency(profile) },
                { "error_rate", CalculateErrorRate(profile) }
            };
        }

        private double CalculateWorkflowEfficiency(OrganizationBehaviorProfile profile)
        {
            var completedTasks = profile.AggregateWorkflowEntries.Count(x => x.TaskCompleted);
            var totalTasks = profile.AggregateWorkflowEntries.Count;
            return totalTasks > 0 ? (double)completedTasks / totalTasks : 0;
        }

        private double CalculateErrorRate(OrganizationBehaviorProfile profile)
        {
            var errorCount = profile.CommonErrorPatterns.Count;
            var totalEntries = profile.AggregateWorkflowEntries.Count;
            return totalEntries > 0 ? (double)errorCount / totalEntries : 0;
        }

        private List<string> GenerateOptimizationRecommendations(OrganizationBehaviorProfile profile)
        {
            var recommendations = new List<string>();

            if (profile.AverageTaskCompletionTimes.Values.Any(x => x > 30000)) // 30 seconds
            {
                recommendations.Add("Consider workflow simplification for slow tasks");
            }

            if (profile.CommonErrorPatterns.Count > 10)
            {
                recommendations.Add("Review common error patterns and add preventive guidance");
            }

            return recommendations;
        }
    }

    // Supporting classes for LyraPersonaTrainer
    public class WorkflowBehaviorEntry
    {
        public int UserId { get; set; }
        public string TaskType { get; set; } = string.Empty;
        public string WorkflowPattern { get; set; } = string.Empty;
        public string PageContext { get; set; } = string.Empty;
        public int SequenceOrder { get; set; }
        public double TaskCompletionTime { get; set; }
        public int DelayBeforeAction { get; set; }
        public bool TipShown { get; set; }
        public bool TaskCompleted { get; set; }
        public bool UsedShortcut { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class UserBehaviorProfile
    {
        public int UserId { get; set; }
        public string OrganizationId { get; set; } = string.Empty;
        public DateTime FirstActivityDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public List<WorkflowBehaviorEntry> WorkflowEntries { get; set; } = new();
        public List<string> PreferredTaskOrder { get; set; } = new();
        public int OptimalTipTiming { get; set; } = 2000;
        public double AverageDelayBetweenActions { get; set; }
    }

    public class OrganizationBehaviorProfile
    {
        public string OrganizationId { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public List<WorkflowBehaviorEntry> AggregateWorkflowEntries { get; set; } = new();
        public List<string> CommonWorkflowPatterns { get; set; } = new();
        public Dictionary<string, int> PeakActivityTimes { get; set; } = new();
        public List<string> PreferredTaskSequences { get; set; } = new();
        public Dictionary<string, double> AverageTaskCompletionTimes { get; set; } = new();
        public List<string> CommonErrorPatterns { get; set; } = new();
    }

    public class PersonalizationRecommendations
    {
        public int UserId { get; set; }
        public string PerformanceLevel { get; set; } = string.Empty;
        public List<string> PreferredWorkflowOrder { get; set; } = new();
        public int OptimalTipTiming { get; set; }
        public List<string> SuggestedShortcuts { get; set; } = new();
        public Dictionary<string, object> CustomOverlaySettings { get; set; } = new();
        public List<string> PredictedNextActions { get; set; } = new();
        public double AdaptationStrength { get; set; }
    }

    public class OrganizationInsights
    {
        public string OrganizationId { get; set; } = string.Empty;
        public List<string> CommonWorkflowPatterns { get; set; } = new();
        public Dictionary<string, int> PeakActivityTimes { get; set; } = new();
        public List<string> PreferredTaskSequences { get; set; } = new();
        public Dictionary<string, double> AverageTaskCompletionTimes { get; set; } = new();
        public List<string> ErrorPatterns { get; set; } = new();
        public Dictionary<string, double> EfficiencyMetrics { get; set; } = new();
        public List<string> RecommendedOptimizations { get; set; } = new();
    }

    public class AdaptationSettings
    {
        public bool ShowTips { get; set; }
        public int TipDelay { get; set; }
        public bool ShortcutsEnabled { get; set; }
        public double OverlayIntensity { get; set; }
        public List<string> WorkflowSuggestions { get; set; } = new();
        public List<string> PredictiveActions { get; set; } = new();
    }
}