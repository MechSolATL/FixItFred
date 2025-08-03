using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Manages tip suppression logic and tracks per-user tip interactions
    /// Identifies muscle memory patterns and adapts guidance accordingly
    /// </summary>
    public class LyraTipRegistry
    {
        private readonly List<UserTipInteractionLog> _interactionLogs;
        private readonly Dictionary<string, int> _tipSuppressionThresholds;
        private readonly int _muscleMemoryThreshold = 3; // 3+ uses indicates muscle memory

        public LyraTipRegistry()
        {
            _interactionLogs = new List<UserTipInteractionLog>();
            _tipSuppressionThresholds = new Dictionary<string, int>
            {
                { "basic_navigation", 2 },
                { "form_completion", 3 },
                { "workflow_guidance", 4 },
                { "advanced_features", 5 }
            };
        }

        /// <summary>
        /// Records a user's interaction with a tip or guidance element
        /// </summary>
        public async Task LogTipInteractionAsync(UserTipInteractionLog interaction)
        {
            // Update view count for this tip
            var existingInteractions = _interactionLogs
                .Where(x => x.UserId == interaction.UserId && x.TipId == interaction.TipId)
                .ToList();

            interaction.ViewCount = existingInteractions.Count + 1;

            // Check for muscle memory development
            if (interaction.ViewCount >= _muscleMemoryThreshold && 
                interaction.ActionCompleted && 
                interaction.InteractionDurationMs < 2000) // Quick interaction indicates familiarity
            {
                interaction.IndicatesMuscleMemory = true;
            }

            _interactionLogs.Add(interaction);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Determines if a tip should be suppressed for a specific user
        /// </summary>
        public async Task<bool> ShouldSuppressTipAsync(int userId, string tipId, string tipCategory = "default")
        {
            var userInteractions = _interactionLogs
                .Where(x => x.UserId == userId && x.TipId == tipId)
                .ToList();

            if (!userInteractions.Any())
                return false;

            var threshold = _tipSuppressionThresholds.GetValueOrDefault(tipCategory, 3);
            var successfulInteractions = userInteractions.Count(x => x.ActionCompleted);

            // Suppress if user has successfully used this tip enough times
            if (successfulInteractions >= threshold)
                return true;

            // Suppress if user has explicitly dismissed the tip multiple times
            var dismissalCount = userInteractions.Count(x => x.InteractionType == "dismissed");
            if (dismissalCount >= 2)
                return true;

            // Check for muscle memory patterns
            var recentInteractions = userInteractions
                .Where(x => x.Timestamp > DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(x => x.Timestamp)
                .Take(3)
                .ToList();

            if (recentInteractions.Count >= 3 && 
                recentInteractions.All(x => x.IndicatesMuscleMemory))
                return true;

            return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets muscle memory indicators for a user across all tips
        /// </summary>
        public async Task<Dictionary<string, bool>> GetMuscleMemoryIndicatorsAsync(int userId)
        {
            var result = new Dictionary<string, bool>();
            
            var userTips = _interactionLogs
                .Where(x => x.UserId == userId)
                .GroupBy(x => x.TipId)
                .ToList();

            foreach (var tipGroup in userTips)
            {
                var muscleMemoryCount = tipGroup.Count(x => x.IndicatesMuscleMemory);
                result[tipGroup.Key] = muscleMemoryCount >= _muscleMemoryThreshold;
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Gets user's interaction statistics for performance analysis
        /// </summary>
        public async Task<UserTipStatistics> GetUserTipStatisticsAsync(int userId)
        {
            var userInteractions = _interactionLogs.Where(x => x.UserId == userId).ToList();

            var stats = new UserTipStatistics
            {
                UserId = userId,
                TotalTipInteractions = userInteractions.Count,
                UniqueTipsViewed = userInteractions.Select(x => x.TipId).Distinct().Count(),
                SuccessfulActions = userInteractions.Count(x => x.ActionCompleted),
                DismissalRate = userInteractions.Any() ? 
                    (double)userInteractions.Count(x => x.InteractionType == "dismissed") / userInteractions.Count : 0,
                AverageInteractionTime = userInteractions.Any() ? 
                    userInteractions.Average(x => x.InteractionDurationMs) : 0,
                MuscleMemoryTips = userInteractions.Count(x => x.IndicatesMuscleMemory),
                LastInteraction = userInteractions.Any() ? 
                    userInteractions.Max(x => x.Timestamp) : DateTime.MinValue
            };

            return await Task.FromResult(stats);
        }

        /// <summary>
        /// Manually suppresses a tip for a user (admin override)
        /// </summary>
        public async Task SuppressTipAsync(int userId, string tipId, string reason = "manual_override")
        {
            var suppressionLog = new UserTipInteractionLog
            {
                UserId = userId,
                TipId = tipId,
                InteractionType = "suppressed",
                TaskContext = reason,
                PageContext = "admin_override",
                ActionCompleted = true,
                IndicatesMuscleMemory = true,
                Timestamp = DateTime.UtcNow,
                Metadata = $"Manually suppressed: {reason}"
            };

            await LogTipInteractionAsync(suppressionLog);
        }

        /// <summary>
        /// Resets tip interactions for a user (useful for retraining)
        /// </summary>
        public async Task ResetUserTipInteractionsAsync(int userId, string? specificTipId = null)
        {
            if (specificTipId != null)
            {
                _interactionLogs.RemoveAll(x => x.UserId == userId && x.TipId == specificTipId);
            }
            else
            {
                _interactionLogs.RemoveAll(x => x.UserId == userId);
            }

            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Statistics for user tip interactions
    /// </summary>
    public class UserTipStatistics
    {
        public int UserId { get; set; }
        public int TotalTipInteractions { get; set; }
        public int UniqueTipsViewed { get; set; }
        public int SuccessfulActions { get; set; }
        public double DismissalRate { get; set; }
        public double AverageInteractionTime { get; set; }
        public int MuscleMemoryTips { get; set; }
        public DateTime LastInteraction { get; set; }
    }
}