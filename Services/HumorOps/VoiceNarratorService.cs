using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Services.HumorOps
{
    /// <summary>
    /// [Sprint126_OneScan_21-40] Voice Narrator Mode Pack Service
    /// Manages multiple voice personalities with random rotation and fallback protection
    /// Personalities: Buckwheat, Forrest Gump, Rodney Dangerfield, Dice Clay, Austin Powers, Danny DeVito
    /// Includes share-to-IG image generator with Service-Atlanta watermark
    /// </summary>
    public class VoiceNarratorService
    {
        private readonly ILogger<VoiceNarratorService> _logger;
        private readonly IConfiguration _configuration;
        private readonly Random _random;

        // [Sprint126_OneScan_21-40] Voice personality definitions
        public enum VoicePersonality
        {
            Buckwheat,
            ForrestGump,
            RodneyDangerfield,
            DiceClay,
            AustinPowers,
            DannyDeVito,
            DefaultNarrator
        }

        // [Sprint126_OneScan_21-40] Voice personality configurations
        private readonly Dictionary<VoicePersonality, VoiceConfig> _voiceConfigs;
        private readonly List<VoicePersonality> _availableVoices;
        private VoicePersonality _lastUsedVoice;

        public VoiceNarratorService(ILogger<VoiceNarratorService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _random = new Random();
            _lastUsedVoice = VoicePersonality.DefaultNarrator;

            // [Sprint126_OneScan_21-40] Initialize voice personality configurations
            _voiceConfigs = InitializeVoiceConfigs();
            _availableVoices = Enum.GetValues<VoicePersonality>()
                .Where(v => v != VoicePersonality.DefaultNarrator)
                .ToList();
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Gets random voice personality with rotation protection
        /// Ensures same voice isn't used consecutively
        /// </summary>
        public VoicePersonality GetRandomVoicePersonality()
        {
            try
            {
                var availableChoices = _availableVoices.Where(v => v != _lastUsedVoice).ToList();
                
                if (!availableChoices.Any())
                {
                    availableChoices = _availableVoices.ToList();
                }

                var selectedVoice = availableChoices[_random.Next(availableChoices.Count)];
                _lastUsedVoice = selectedVoice;

                _logger.LogDebug("[Sprint126_OneScan_21-40] Selected voice personality: {Voice}", selectedVoice);
                return selectedVoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint126_OneScan_21-40] Error selecting random voice, using default");
                return VoicePersonality.DefaultNarrator;
            }
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Generates narration in specified voice personality
        /// </summary>
        /// <param name="text">Text to narrate</param>
        /// <param name="personality">Voice personality to use</param>
        /// <param name="context">Context for personality adaptation</param>
        /// <returns>Voice-adapted narration</returns>
        public async Task<VoiceNarrationResult> GenerateVoiceNarrationAsync(string text, VoicePersonality? personality = null, string context = "")
        {
            var selectedVoice = personality ?? GetRandomVoicePersonality();
            
            _logger.LogInformation("[Sprint126_OneScan_21-40] Generating narration for voice: {Voice}, text length: {Length}", 
                selectedVoice, text.Length);

            try
            {
                var voiceConfig = _voiceConfigs[selectedVoice];
                var adaptedText = await AdaptTextToVoiceAsync(text, voiceConfig, context);

                var result = new VoiceNarrationResult
                {
                    Id = Guid.NewGuid(),
                    OriginalText = text,
                    NarratedText = adaptedText,
                    VoicePersonality = selectedVoice,
                    VoiceName = voiceConfig.Name,
                    Context = context,
                    Timestamp = DateTime.UtcNow,
                    AudioUrl = await GenerateAudioUrlAsync(adaptedText, selectedVoice),
                    ShareImageUrl = await GenerateShareImageAsync(adaptedText, selectedVoice)
                };

                _logger.LogInformation("[Sprint126_OneScan_21-40] Generated narration {NarrationId} for voice {Voice}", 
                    result.Id, selectedVoice);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint126_OneScan_21-40] Error generating narration for voice {Voice}", selectedVoice);
                
                // [Sprint126_OneScan_21-40] Fallback to default narrator
                return new VoiceNarrationResult
                {
                    Id = Guid.NewGuid(),
                    OriginalText = text,
                    NarratedText = text,
                    VoicePersonality = VoicePersonality.DefaultNarrator,
                    VoiceName = "Default Narrator",
                    Context = "fallback",
                    Timestamp = DateTime.UtcNow,
                    AudioUrl = null,
                    ShareImageUrl = null
                };
            }
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Adapts text to specific voice personality
        /// </summary>
        private async Task<string> AdaptTextToVoiceAsync(string text, VoiceConfig config, string context)
        {
            return config.Personality switch
            {
                VoicePersonality.Buckwheat => await AdaptToBuckwheatAsync(text, context),
                VoicePersonality.ForrestGump => await AdaptToForrestGumpAsync(text, context),
                VoicePersonality.RodneyDangerfield => await AdaptToRodneyDangerfieldAsync(text, context),
                VoicePersonality.DiceClay => await AdaptToDiceClayAsync(text, context),
                VoicePersonality.AustinPowers => await AdaptToAustinPowersAsync(text, context),
                VoicePersonality.DannyDeVito => await AdaptToDannyDeVitoAsync(text, context),
                _ => text
            };
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Buckwheat voice adaptation
        /// </summary>
        private async Task<string> AdaptToBuckwheatAsync(string text, string context)
        {
            var adaptations = new Dictionary<string, string>
            {
                { "okay", "otay" },
                { "yes", "yep yep" },
                { "hello", "hewwo" },
                { "really", "weally" },
                { "right", "wight" },
                { "ready", "weady" },
                { "problem", "pwoblem" },
                { "quick", "qwick" },
                { "very", "vewy" }
            };

            var adapted = text;
            foreach (var kvp in adaptations)
            {
                adapted = adapted.Replace(kvp.Key, kvp.Value, StringComparison.OrdinalIgnoreCase);
            }

            return $"*in Buckwheat voice* {adapted}!";
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Forrest Gump voice adaptation
        /// </summary>
        private async Task<string> AdaptToForrestGumpAsync(string text, string context)
        {
            var gumpisms = new[]
            {
                "Well, I'll tell you what",
                "Mama always said",
                "That's all I got to say about that",
                "Life is like a box of chocolates",
                "Stupid is as stupid does"
            };

            var intro = gumpisms[_random.Next(gumpisms.Length)];
            return $"{intro}... {text}. That's all I got to say about that.";
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Rodney Dangerfield voice adaptation
        /// </summary>
        private async Task<string> AdaptToRodneyDangerfieldAsync(string text, string context)
        {
            var rodneyIntros = new[]
            {
                "I tell ya, I get no respect at all",
                "Take my system, please",
                "I went to my developer and said",
                "You know what I mean?"
            };

            var intro = rodneyIntros[_random.Next(rodneyIntros.Length)];
            return $"{intro}... {text}. *tugs at collar* No respect, I tell ya!";
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Dice Clay voice adaptation  
        /// </summary>
        private async Task<string> AdaptToDiceClayAsync(string text, string context)
        {
            return $"Ohhhhhhh! {text}! You know what I'm sayin'? Unbelievable!";
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Austin Powers voice adaptation
        /// </summary>
        private async Task<string> AdaptToAustinPowersAsync(string text, string context)
        {
            var powersisms = new[]
            {
                "Yeah baby!",
                "Groovy!",
                "Shagadelic!",
                "Oh behave!",
                "Smashing!"
            };

            var outro = powersisms[_random.Next(powersisms.Length)];
            return $"Right then... {text}. {outro}";
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Danny DeVito voice adaptation
        /// </summary>
        private async Task<string> AdaptToDannyDeVitoAsync(string text, string context)
        {
            return $"*gruff DeVito voice* Listen here, {text}. Capisce?";
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Generates audio URL for narration (mock implementation)
        /// </summary>
        private async Task<string?> GenerateAudioUrlAsync(string text, VoicePersonality personality)
        {
            // Mock audio generation - in production would integrate with TTS service
            await Task.Delay(100); // Simulate processing
            return $"/api/audio/narration/{Guid.NewGuid()}.mp3";
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Generates share-to-IG image with Service-Atlanta watermark
        /// </summary>
        private async Task<string?> GenerateShareImageAsync(string text, VoicePersonality personality)
        {
            try
            {
                await Task.Delay(200); // Simulate image generation

                // Mock image generation with Service-Atlanta watermark
                var imageConfig = new ShareImageConfig
                {
                    Text = text,
                    VoicePersonality = personality,
                    Watermark = "Service-Atlanta",
                    BackgroundColor = GetPersonalityColor(personality),
                    FontFamily = "Arial, sans-serif",
                    Width = 1080,
                    Height = 1080
                };

                // In production, would generate actual image
                var imageId = Guid.NewGuid();
                _logger.LogDebug("[Sprint126_OneScan_21-40] Generated share image {ImageId} for voice {Voice}", 
                    imageId, personality);

                return $"/api/images/share/{imageId}.jpg";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint126_OneScan_21-40] Error generating share image for voice {Voice}", personality);
                return null;
            }
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Gets personality-specific color for image generation
        /// </summary>
        private string GetPersonalityColor(VoicePersonality personality)
        {
            return personality switch
            {
                VoicePersonality.Buckwheat => "#FFD700",
                VoicePersonality.ForrestGump => "#228B22",
                VoicePersonality.RodneyDangerfield => "#DC143C",
                VoicePersonality.DiceClay => "#000000",
                VoicePersonality.AustinPowers => "#4169E1",
                VoicePersonality.DannyDeVito => "#8B4513",
                _ => "#808080"
            };
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Gets all available voice personalities with metadata
        /// </summary>
        public async Task<List<VoicePersonalityInfo>> GetAvailableVoicesAsync()
        {
            return _voiceConfigs.Select(kvp => new VoicePersonalityInfo
            {
                Personality = kvp.Key,
                Name = kvp.Value.Name,
                Description = kvp.Value.Description,
                SamplePhrase = kvp.Value.SamplePhrase,
                Color = GetPersonalityColor(kvp.Key),
                IsAvailable = _availableVoices.Contains(kvp.Key)
            }).ToList();
        }

        /// <summary>
        /// [Sprint126_OneScan_21-40] Initializes voice personality configurations
        /// </summary>
        private Dictionary<VoicePersonality, VoiceConfig> InitializeVoiceConfigs()
        {
            return new Dictionary<VoicePersonality, VoiceConfig>
            {
                [VoicePersonality.Buckwheat] = new VoiceConfig
                {
                    Personality = VoicePersonality.Buckwheat,
                    Name = "Buckwheat",
                    Description = "Innocent and endearing childhood character voice",
                    SamplePhrase = "Otay! Dis is gonna be gweaat!",
                    AccentStrength = 0.8f,
                    SpeedModifier = 0.9f
                },
                [VoicePersonality.ForrestGump] = new VoiceConfig
                {
                    Personality = VoicePersonality.ForrestGump,
                    Name = "Forrest Gump",
                    Description = "Southern drawl with folksy wisdom",
                    SamplePhrase = "Mama always said, life is like a box of chocolates",
                    AccentStrength = 0.7f,
                    SpeedModifier = 0.8f
                },
                [VoicePersonality.RodneyDangerfield] = new VoiceConfig
                {
                    Personality = VoicePersonality.RodneyDangerfield,
                    Name = "Rodney Dangerfield",
                    Description = "Self-deprecating comedian with nervous energy",
                    SamplePhrase = "I tell ya, I get no respect at all!",
                    AccentStrength = 0.6f,
                    SpeedModifier = 1.1f
                },
                [VoicePersonality.DiceClay] = new VoiceConfig
                {
                    Personality = VoicePersonality.DiceClay,
                    Name = "Andrew Dice Clay",
                    Description = "Brooklyn tough guy attitude",
                    SamplePhrase = "Ohhhhhhh! Unbelievable!",
                    AccentStrength = 0.9f,
                    SpeedModifier = 1.0f
                },
                [VoicePersonality.AustinPowers] = new VoiceConfig
                {
                    Personality = VoicePersonality.AustinPowers,
                    Name = "Austin Powers",
                    Description = "British spy with 60s flair",
                    SamplePhrase = "Yeah baby! Groovy!",
                    AccentStrength = 0.8f,
                    SpeedModifier = 1.0f
                },
                [VoicePersonality.DannyDeVito] = new VoiceConfig
                {
                    Personality = VoicePersonality.DannyDeVito,
                    Name = "Danny DeVito",
                    Description = "Gruff, no-nonsense Brooklyn attitude",
                    SamplePhrase = "Listen here, capisce?",
                    AccentStrength = 0.7f,
                    SpeedModifier = 0.9f
                },
                [VoicePersonality.DefaultNarrator] = new VoiceConfig
                {
                    Personality = VoicePersonality.DefaultNarrator,
                    Name = "Default Narrator",
                    Description = "Professional system narrator",
                    SamplePhrase = "System notification ready.",
                    AccentStrength = 0.0f,
                    SpeedModifier = 1.0f
                }
            };
        }
    }

    /// <summary>
    /// [Sprint126_OneScan_21-40] Voice narration result
    /// </summary>
    public class VoiceNarrationResult
    {
        public Guid Id { get; set; }
        public string OriginalText { get; set; } = string.Empty;
        public string NarratedText { get; set; } = string.Empty;
        public VoiceNarratorService.VoicePersonality VoicePersonality { get; set; }
        public string VoiceName { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? AudioUrl { get; set; }
        public string? ShareImageUrl { get; set; }
    }

    /// <summary>
    /// [Sprint126_OneScan_21-40] Voice personality information
    /// </summary>
    public class VoicePersonalityInfo
    {
        public VoiceNarratorService.VoicePersonality Personality { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SamplePhrase { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    /// <summary>
    /// [Sprint126_OneScan_21-40] Voice configuration
    /// </summary>
    public class VoiceConfig
    {
        public VoiceNarratorService.VoicePersonality Personality { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SamplePhrase { get; set; } = string.Empty;
        public float AccentStrength { get; set; }
        public float SpeedModifier { get; set; }
    }

    /// <summary>
    /// [Sprint126_OneScan_21-40] Share image configuration
    /// </summary>
    public class ShareImageConfig
    {
        public string Text { get; set; } = string.Empty;
        public VoiceNarratorService.VoicePersonality VoicePersonality { get; set; }
        public string Watermark { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public string FontFamily { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
    }
}