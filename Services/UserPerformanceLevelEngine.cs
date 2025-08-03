using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// Flags user as Beginner → Intermediate → Expert based on behavior patterns
    /// Unlocks Mentor Mode toggle for Expert-level users
    /// </summary>
    public class UserPerformanceLevelEngine
    {
        private readonly LyraTipRegistry _tipRegistry;
        private readonly Dictionary<int, UserPerformanceProfile> _userProfiles;

        // Thresholds for performance level progression
        private const int INTERMEDIATE_DAYS_THRESHOLD = 14;
        private const int INTERMEDIATE_ACTIONS_THRESHOLD = 50;
        private const int EXPERT_DAYS_THRESHOLD = 45;
        private const int EXPERT_ACTIONS_THRESHOLD = 200;
        private const int EXPERT_MUSCLE_MEMORY_THRESHOLD = 10;
        private const double EXPERT_SUCCESS_RATE_THRESHOLD = 0.85;

        public UserPerformanceLevelEngine(LyraTipRegistry tipRegistry)
        {
            _tipRegistry = tipRegistry;
            _userProfiles = new Dictionary<int, UserPerformanceProfile>();
        }

        /// <summary>
        /// Evaluates and updates a user's performance level
        /// </summary>
        public async Task<string> EvaluateUserPerformanceLevelAsync(int userId)
        {
            var profile = await GetOrCreateUserProfileAsync(userId);
            var tipStats = await _tipRegistry.GetUserTipStatisticsAsync(userId);
            
            // Calculate current metrics
            var daysSinceFirstUse = (DateTime.UtcNow - profile.FirstLoginDate).TotalDays;
            var muscleMemoryIndicators = await _tipRegistry.GetMuscleMemoryIndicatorsAsync(userId);
            var muscleMemoryCount = muscleMemoryIndicators.Count(x => x.Value);
            
            // Determine performance level based on criteria
            var newLevel = DeterminePerformanceLevel(
                daysSinceFirstUse,
                tipStats.TotalTipInteractions,
                tipStats.SuccessfulActions,
                muscleMemoryCount,
                1.0 - tipStats.DismissalRate // Success rate approximation
            );

            // Update profile if level changed
            if (profile.CurrentLevel != newLevel)
            {
                profile.PreviousLevel = profile.CurrentLevel;
                profile.CurrentLevel = newLevel;
                profile.LevelChangedDate = DateTime.UtcNow;
                
                // Unlock Mentor Mode for Expert users
                if (newLevel == "Expert")
                {
                    profile.MentorModeUnlocked = true;
                    profile.MentorModeUnlockedDate = DateTime.UtcNow;
                }
            }

            profile.LastEvaluationDate = DateTime.UtcNow;
            _userProfiles[userId] = profile;

            return newLevel;
        }

        /// <summary>
        /// Gets the current performance level for a user
        /// </summary>
        public async Task<string> GetUserPerformanceLevelAsync(int userId)
        {
            var profile = await GetOrCreateUserProfileAsync(userId);
            
            // Re-evaluate if it's been more than a week since last evaluation
            if ((DateTime.UtcNow - profile.LastEvaluationDate).TotalDays > 7)
            {
                return await EvaluateUserPerformanceLevelAsync(userId);
            }

            return profile.CurrentLevel;
        }

        /// <summary>
        /// Checks if Mentor Mode is unlocked for a user
        /// </summary>
        public async Task<bool> IsMentorModeUnlockedAsync(int userId)
        {
            var profile = await GetOrCreateUserProfileAsync(userId);
            return profile.MentorModeUnlocked;
        }

        /// <summary>
        /// Gets detailed performance profile for a user
        /// </summary>
        public async Task<UserPerformanceProfile> GetUserPerformanceProfileAsync(int userId)
        {
            return await GetOrCreateUserProfileAsync(userId);
        }

        /// <summary>
        /// Forces a user to a specific performance level (admin override)
        /// </summary>
        public async Task SetUserPerformanceLevelAsync(int userId, string level, string reason = "admin_override")
        {
            var profile = await GetOrCreateUserProfileAsync(userId);
            profile.PreviousLevel = profile.CurrentLevel;
            profile.CurrentLevel = level;
            profile.LevelChangedDate = DateTime.UtcNow;
            profile.LastEvaluationDate = DateTime.UtcNow;
            profile.AdminOverride = true;
            profile.AdminOverrideReason = reason;

            if (level == "Expert")
            {
                profile.MentorModeUnlocked = true;
                if (profile.MentorModeUnlockedDate == DateTime.MinValue)
                {
                    profile.MentorModeUnlockedDate = DateTime.UtcNow;
                }
            }

            _userProfiles[userId] = profile;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets performance statistics across all users
        /// </summary>
        public async Task<PerformanceLevelStatistics> GetOverallPerformanceStatisticsAsync()
        {
            var stats = new PerformanceLevelStatistics();
            
            foreach (var profile in _userProfiles.Values)
            {
                switch (profile.CurrentLevel)
                {
                    case "Beginner":
                        stats.BeginnerCount++;
                        break;
                    case "Intermediate":
                        stats.IntermediateCount++;
                        break;
                    case "Expert":
                        stats.ExpertCount++;
                        if (profile.MentorModeEnabled)
                            stats.ActiveMentorCount++;
                        break;
                }
            }

            stats.TotalUsers = _userProfiles.Count;
            stats.MentorModeUnlockedCount = _userProfiles.Values.Count(x => x.MentorModeUnlocked);

            return await Task.FromResult(stats);
        }

        /// <summary>
        /// Enables or disables Mentor Mode for an Expert user
        /// </summary>
        public async Task SetMentorModeAsync(int userId, bool enabled)
        {
            var profile = await GetOrCreateUserProfileAsync(userId);
            
            if (!profile.MentorModeUnlocked)
            {
                throw new InvalidOperationException("Mentor Mode is not unlocked for this user");
            }

            profile.MentorModeEnabled = enabled;
            profile.MentorModeLastToggled = DateTime.UtcNow;
            _userProfiles[userId] = profile;
            
            await Task.CompletedTask;
        }

        private async Task<UserPerformanceProfile> GetOrCreateUserProfileAsync(int userId)
        {
            if (!_userProfiles.ContainsKey(userId))
            {
                _userProfiles[userId] = new UserPerformanceProfile
                {
                    UserId = userId,
                    CurrentLevel = "Beginner",
                    FirstLoginDate = DateTime.UtcNow,
                    LastEvaluationDate = DateTime.UtcNow
                };
            }

            return await Task.FromResult(_userProfiles[userId]);
        }

        private string DeterminePerformanceLevel(
            double daysSinceFirstUse,
            int totalInteractions,
            int successfulActions,
            int muscleMemoryCount,
            double successRate)
        {
            // Expert criteria
            if (daysSinceFirstUse >= EXPERT_DAYS_THRESHOLD &&
                successfulActions >= EXPERT_ACTIONS_THRESHOLD &&
                muscleMemoryCount >= EXPERT_MUSCLE_MEMORY_THRESHOLD &&
                successRate >= EXPERT_SUCCESS_RATE_THRESHOLD)
            {
                return "Expert";
            }

            // Intermediate criteria
            if (daysSinceFirstUse >= INTERMEDIATE_DAYS_THRESHOLD &&
                successfulActions >= INTERMEDIATE_ACTIONS_THRESHOLD &&
                muscleMemoryCount >= 3)
            {
                return "Intermediate";
            }

            // Default to Beginner
            return "Beginner";
        }
    }

    /// <summary>
    /// Detailed performance profile for a user
    /// </summary>
    public class UserPerformanceProfile
    {
        public int UserId { get; set; }
        public string CurrentLevel { get; set; } = "Beginner";
        public string? PreviousLevel { get; set; }
        public DateTime FirstLoginDate { get; set; }
        public DateTime LevelChangedDate { get; set; }
        public DateTime LastEvaluationDate { get; set; }
        public bool MentorModeUnlocked { get; set; }
        public bool MentorModeEnabled { get; set; }
        public DateTime MentorModeUnlockedDate { get; set; }
        public DateTime MentorModeLastToggled { get; set; }
        public bool AdminOverride { get; set; }
        public string AdminOverrideReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Overall performance statistics across all users
    /// </summary>
    public class PerformanceLevelStatistics
    {
        public int TotalUsers { get; set; }
        public int BeginnerCount { get; set; }
        public int IntermediateCount { get; set; }
        public int ExpertCount { get; set; }
        public int MentorModeUnlockedCount { get; set; }
        public int ActiveMentorCount { get; set; }
    }
}