using Xunit;
using Services;
using Models;
using System.Threading.Tasks;

namespace Tests.Services
{
    public class LyraAdaptiveLearningTests
    {
        [Fact]
        public async Task LyraTipRegistry_ShouldSuppressTipAfterThreshold()
        {
            // Arrange
            var tipRegistry = new LyraTipRegistry();
            const int userId = 1;
            const string tipId = "basic_navigation_tip";

            // Act - Log multiple successful interactions
            for (int i = 0; i < 3; i++)
            {
                await tipRegistry.LogTipInteractionAsync(new UserTipInteractionLog
                {
                    UserId = userId,
                    TipId = tipId,
                    InteractionType = "viewed",
                    ActionCompleted = true,
                    TaskContext = "service_request_creation",
                    PageContext = "/revitalize",
                    InteractionDurationMs = 1500
                });
            }

            // Assert
            var shouldSuppress = await tipRegistry.ShouldSuppressTipAsync(userId, tipId, "basic_navigation");
            Assert.True(shouldSuppress);
        }

        [Fact]
        public async Task UserPerformanceLevelEngine_ShouldProgressToExpert()
        {
            // Arrange
            var tipRegistry = new LyraTipRegistry();
            var performanceEngine = new UserPerformanceLevelEngine(tipRegistry);
            const int userId = 2;

            // Act - Simulate expert-level usage
            for (int i = 0; i < 15; i++)
            {
                await tipRegistry.LogTipInteractionAsync(new UserTipInteractionLog
                {
                    UserId = userId,
                    TipId = $"advanced_tip_{i}",
                    InteractionType = "viewed",
                    ActionCompleted = true,
                    IndicatesMuscleMemory = i > 10,
                    TaskContext = "workflow_optimization",
                    PageContext = "/revitalize/advanced"
                });
            }

            // Force user to expert level for testing
            await performanceEngine.SetUserPerformanceLevelAsync(userId, "Expert", "test_scenario");

            // Assert
            var level = await performanceEngine.GetUserPerformanceLevelAsync(userId);
            var mentorUnlocked = await performanceEngine.IsMentorModeUnlockedAsync(userId);

            Assert.Equal("Expert", level);
            Assert.True(mentorUnlocked);
        }

        [Fact]
        public async Task LyraPersonaTrainer_ShouldProvidePersonalization()
        {
            // Arrange
            var tipRegistry = new LyraTipRegistry();
            var performanceEngine = new UserPerformanceLevelEngine(tipRegistry);
            var personaTrainer = new LyraPersonaTrainer(performanceEngine, tipRegistry);
            const int userId = 3;
            const string orgId = "test_org";

            // Act - Record workflow behavior
            for (int i = 0; i < 25; i++)
            {
                await personaTrainer.RecordWorkflowBehaviorAsync(userId, orgId, new WorkflowBehaviorEntry
                {
                    UserId = userId,
                    TaskType = i % 2 == 0 ? "service_request" : "tenant_management",
                    WorkflowPattern = "standard_workflow",
                    PageContext = "/revitalize/dashboard",
                    SequenceOrder = i % 5,
                    TaskCompletionTime = 2000 + (i * 100),
                    DelayBeforeAction = 1500 - (i * 20),
                    TaskCompleted = true,
                    UsedShortcut = i > 15
                });
            }

            // Assert
            var recommendations = await personaTrainer.GetPersonalizationRecommendationsAsync(userId);
            Assert.NotNull(recommendations);
            Assert.True(recommendations.AdaptationStrength > 0);
            Assert.NotEmpty(recommendations.PreferredWorkflowOrder);
        }

        [Fact]
        public async Task MentorMode_ShouldOnlyBeAvailableToExperts()
        {
            // Arrange
            var tipRegistry = new LyraTipRegistry();
            var performanceEngine = new UserPerformanceLevelEngine(tipRegistry);
            const int beginnerUserId = 4;
            const int expertUserId = 5;

            // Set different performance levels
            await performanceEngine.SetUserPerformanceLevelAsync(expertUserId, "Expert", "test_setup");

            // Act & Assert
            var beginnerCanUseMentor = await performanceEngine.IsMentorModeUnlockedAsync(beginnerUserId);
            var expertCanUseMentor = await performanceEngine.IsMentorModeUnlockedAsync(expertUserId);

            Assert.False(beginnerCanUseMentor);
            Assert.True(expertCanUseMentor);

            // Test mentor mode toggling for expert
            await performanceEngine.SetMentorModeAsync(expertUserId, true);
            var profile = await performanceEngine.GetUserPerformanceProfileAsync(expertUserId);
            Assert.True(profile.MentorModeEnabled);

            // Test that beginner cannot enable mentor mode
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => performanceEngine.SetMentorModeAsync(beginnerUserId, true)
            );
        }

        [Fact]
        public async Task TipStatistics_ShouldTrackUserProgress()
        {
            // Arrange
            var tipRegistry = new LyraTipRegistry();
            const int userId = 6;

            // Act - Log various interactions
            await tipRegistry.LogTipInteractionAsync(new UserTipInteractionLog
            {
                UserId = userId,
                TipId = "tip1",
                InteractionType = "viewed",
                ActionCompleted = true
            });

            await tipRegistry.LogTipInteractionAsync(new UserTipInteractionLog
            {
                UserId = userId,
                TipId = "tip2",
                InteractionType = "dismissed",
                ActionCompleted = false
            });

            await tipRegistry.LogTipInteractionAsync(new UserTipInteractionLog
            {
                UserId = userId,
                TipId = "tip3",
                InteractionType = "viewed",
                ActionCompleted = true,
                IndicatesMuscleMemory = true
            });

            // Assert
            var stats = await tipRegistry.GetUserTipStatisticsAsync(userId);
            Assert.Equal(3, stats.TotalTipInteractions);
            Assert.Equal(3, stats.UniqueTipsViewed);
            Assert.Equal(2, stats.SuccessfulActions);
            Assert.Equal(1, stats.MuscleMemoryTips);
            Assert.True(stats.DismissalRate > 0);
        }
    }
}