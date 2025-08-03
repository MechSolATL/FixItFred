using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Data;
using Data.Models;
using Services;
using System.Threading.Tasks;

namespace Tests.Services
{
    /// <summary>
    /// Unit tests for HeroFX Studio Engine
    /// Sprint127_HeroFX_StudioDivision - Focused tests for effect management
    /// </summary>
    public class HeroFXEngineTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly HeroFXEngine _heroFxEngine;
        private readonly ILogger<HeroFXEngine> _logger;

        public HeroFXEngineTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            
            // Create mock logger
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<HeroFXEngine>();
            
            _heroFxEngine = new HeroFXEngine(_context, _logger);
        }

        [Fact]
        public async Task SeedDefaultEffects_ShouldCreateStandardEffects()
        {
            // Act
            await _heroFxEngine.SeedDefaultEffectsAsync("test-user");

            // Assert
            var effects = await _context.HeroImpactEffects.ToListAsync();
            Assert.NotEmpty(effects);
            Assert.Contains(effects, e => e.Name == "slam");
            Assert.Contains(effects, e => e.Name == "pop");
            Assert.Contains(effects, e => e.Name == "yeet");
            Assert.Contains(effects, e => e.Name == "glitch");
            Assert.Contains(effects, e => e.Name == "stretch");
        }

        [Fact]
        public async Task TriggerEffect_ShouldIncrementUsageCount()
        {
            // Arrange
            await _heroFxEngine.SeedDefaultEffectsAsync("test-user");
            const string effectName = "slam";

            // Act
            var result = await _heroFxEngine.TriggerEffectAsync(effectName, "test", "user123", "Tech", "desktop");

            // Assert
            Assert.True(result);
            
            var effect = await _context.HeroImpactEffects.FirstAsync(e => e.Name == effectName);
            Assert.Equal(1, effect.UsageCount);
            
            var log = await _context.HeroFxAnalyticsLogs.FirstAsync();
            Assert.Equal(effectName, log.EffectName);
            Assert.Equal("test", log.TriggerEvent);
            Assert.Equal("user123", log.UserId);
            Assert.True(log.WasSuccessful);
        }

        [Fact]
        public async Task GetEffectsByTrigger_ShouldFilterCorrectly()
        {
            // Arrange
            await _heroFxEngine.SeedDefaultEffectsAsync("test-user");

            // Act
            var dispatchEffects = await _heroFxEngine.GetEffectsByTriggerAsync("dispatch");
            var loginEffects = await _heroFxEngine.GetEffectsByTriggerAsync("login");

            // Assert
            Assert.NotEmpty(dispatchEffects);
            Assert.NotEmpty(loginEffects);
            Assert.Contains(dispatchEffects, e => e.Name == "slam");
            Assert.Contains(loginEffects, e => e.Name == "pop");
        }

        [Fact]
        public async Task SaveEffect_ShouldCreateNewEffect()
        {
            // Arrange
            var newEffect = new HeroImpactEffect
            {
                Name = "bounce",
                DisplayName = "BOUNCE!",
                CssClass = "hero-fx-bounce",
                DurationMs = 700,
                TriggerEvents = "test",
                IsActive = true
            };

            // Act
            var savedEffect = await _heroFxEngine.SaveEffectAsync(newEffect, "test-user");

            // Assert
            Assert.NotEqual(0, savedEffect.Id);
            Assert.Equal("bounce", savedEffect.Name);
            Assert.Equal("test-user", savedEffect.CreatedBy);
            
            var dbEffect = await _context.HeroImpactEffects.FindAsync(savedEffect.Id);
            Assert.NotNull(dbEffect);
            Assert.Equal("BOUNCE!", dbEffect.DisplayName);
        }

        [Fact]
        public async Task GetRandomEffect_ShouldReturnEffectMatchingCriteria()
        {
            // Arrange
            await _heroFxEngine.SeedDefaultEffectsAsync("test-user");

            // Act
            var techEffect = await _heroFxEngine.GetRandomEffectAsync(null, "Tech", null);
            var csrEffect = await _heroFxEngine.GetRandomEffectAsync(null, "CSR", null);

            // Assert
            Assert.NotNull(techEffect);
            Assert.NotNull(csrEffect);
            
            // Tech role should get slam, yeet, or glitch
            Assert.Contains(techEffect.Name, new[] { "slam", "yeet", "glitch" });
            
            // CSR role should get pop or stretch  
            Assert.Contains(csrEffect.Name, new[] { "pop", "stretch" });
        }

        [Fact]
        public async Task LogReaction_ShouldIncrementReactionCount()
        {
            // Arrange
            await _heroFxEngine.SeedDefaultEffectsAsync("test-user");
            const string effectName = "slam";
            
            // First trigger the effect
            await _heroFxEngine.TriggerEffectAsync(effectName, "test", "user123", "Tech", "desktop");

            // Act
            await _heroFxEngine.LogReactionAsync(effectName, "user123");

            // Assert
            var effect = await _context.HeroImpactEffects.FirstAsync(e => e.Name == effectName);
            Assert.Equal(1, effect.ReactionCount);
            Assert.Equal(100m, effect.KapowToClickRatio); // 1 reaction / 1 usage * 100
            
            var log = await _context.HeroFxAnalyticsLogs.FirstAsync();
            Assert.True(log.GotReaction);
        }

        [Fact]
        public async Task GetAnalytics_ShouldReturnCorrectMetrics()
        {
            // Arrange
            await _heroFxEngine.SeedDefaultEffectsAsync("test-user");
            
            // Trigger some effects
            await _heroFxEngine.TriggerEffectAsync("slam", "dispatch", "user1", "Tech", "desktop");
            await _heroFxEngine.TriggerEffectAsync("pop", "login", "user2", "CSR", "mobile");
            await _heroFxEngine.LogReactionAsync("slam", "user1");

            // Act
            var analytics = await _heroFxEngine.GetAnalyticsAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            // Assert
            Assert.NotNull(analytics);
            var summary = ((dynamic)analytics).Summary;
            Assert.Equal(2, summary.TotalTriggers);
            Assert.Equal(2, summary.SuccessfulTriggers);
            Assert.Equal(100.0, summary.SuccessRate);
            Assert.Equal(1, summary.ReactionsCount);
            Assert.Equal(50.0, summary.KapowToClickRatio);
        }

        [Fact]
        public async Task DeleteEffect_ShouldRemoveFromDatabase()
        {
            // Arrange
            await _heroFxEngine.SeedDefaultEffectsAsync("test-user");
            var effect = await _context.HeroImpactEffects.FirstAsync();

            // Act
            var result = await _heroFxEngine.DeleteEffectAsync(effect.Id, "test-user");

            // Assert
            Assert.True(result);
            var deletedEffect = await _context.HeroImpactEffects.FindAsync(effect.Id);
            Assert.Null(deletedEffect);
        }

        [Fact]
        public async Task TriggerEffect_WithInactiveEffect_ShouldReturnFalse()
        {
            // Arrange
            var inactiveEffect = new HeroImpactEffect
            {
                Name = "inactive",
                DisplayName = "Inactive Effect",
                CssClass = "hero-fx-inactive",
                IsActive = false
            };
            await _heroFxEngine.SaveEffectAsync(inactiveEffect, "test-user");

            // Act
            var result = await _heroFxEngine.TriggerEffectAsync("inactive", "test", "user123", "Tech", "desktop");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task TriggerEffect_WithMobileIncompatibleEffect_ShouldReturnFalse()
        {
            // Arrange
            var desktopOnlyEffect = new HeroImpactEffect
            {
                Name = "desktop-only",
                DisplayName = "Desktop Only",
                CssClass = "hero-fx-desktop",
                IsActive = true,
                IsMobileCompatible = false,
                IsDesktopCompatible = true
            };
            await _heroFxEngine.SaveEffectAsync(desktopOnlyEffect, "test-user");

            // Act
            var result = await _heroFxEngine.TriggerEffectAsync("desktop-only", "test", "user123", "Tech", "mobile");

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}