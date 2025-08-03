using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;
using Services.Admin;

namespace Services.HumorOps
{
    /// <summary>
    /// [Sprint126_OneScan_01-20] Patch & Lyra Roast Engine with Banter Duo Overlay
    /// Implements escalating duo banter system with NYC-style clapbacks and Yo Mama packs
    /// Triggered by job fails, long diagnostics, and checklist gaps
    /// Logs: ReplayEvent.RoastEscalationLevel for auditing and replay
    /// </summary>
    public class PatchLyraBanterEngine
    {
        private readonly ILogger<PatchLyraBanterEngine> _logger;
        private readonly ApplicationDbContext _db;
        private readonly RoastEngineService _roastEngineService;
        private readonly RoastRouletteEngine _roastRouletteEngine;

        // [Sprint126_OneScan_01-20] Banter escalation levels
        public enum BanterEscalationLevel
        {
            Gentle = 1,    // "Dino, look away..." level
            Playful = 2,   // Light teasing
            Spicy = 3,     // NYC-style clapbacks
            Savage = 4,    // Yo Mama pack territory
            Nuclear = 5    // Full roast mode
        }

        // [Sprint126_OneScan_01-20] Trigger types for banter
        public enum BanterTrigger
        {
            JobFailure,
            LongDiagnostics,
            ChecklistGap,
            IdleTime,
            UserRequest,
            EscalationEvent
        }

        public PatchLyraBanterEngine(
            ILogger<PatchLyraBanterEngine> logger,
            ApplicationDbContext db,
            RoastEngineService roastEngineService,
            RoastRouletteEngine roastRouletteEngine)
        {
            _logger = logger;
            _db = db;
            _roastEngineService = roastEngineService;
            _roastRouletteEngine = roastRouletteEngine;
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Triggers banter between Patch and Lyra based on context
        /// </summary>
        /// <param name="trigger">What triggered the banter</param>
        /// <param name="employeeId">Target employee ID</param>
        /// <param name="context">Additional context for banter generation</param>
        /// <returns>Banter response with escalation level</returns>
        public async Task<BanterResponse> TriggerDuoBanterAsync(BanterTrigger trigger, string employeeId, string context = "")
        {
            _logger.LogInformation("[Sprint126_OneScan_01-20] Triggering duo banter: {Trigger} for {EmployeeId}", trigger, employeeId);

            try
            {
                // [Sprint126_OneScan_01-20] Determine escalation level based on trigger and history
                var escalationLevel = await DetermineEscalationLevelAsync(trigger, employeeId);
                
                // [Sprint126_OneScan_01-20] Generate banter based on escalation level
                var banterContent = await GenerateBanterContentAsync(escalationLevel, trigger, context);
                
                // [Sprint126_OneScan_01-20] Create banter response
                var response = new BanterResponse
                {
                    Id = Guid.NewGuid(),
                    PatchLine = banterContent.PatchLine,
                    LyraLine = banterContent.LyraLine,
                    EscalationLevel = escalationLevel,
                    Trigger = trigger,
                    EmployeeId = employeeId,
                    Context = context,
                    Timestamp = DateTime.UtcNow,
                    ShouldDinoLookAway = escalationLevel >= BanterEscalationLevel.Spicy
                };

                // [Sprint126_OneScan_01-20] Log to ReplayEvent for auditing
                await LogBanterEventAsync(response);

                _logger.LogInformation("[Sprint126_OneScan_01-20] Generated banter with escalation level {Level}: {PatchLine} | {LyraLine}", 
                    escalationLevel, banterContent.PatchLine, banterContent.LyraLine);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Sprint126_OneScan_01-20] Error generating duo banter for {EmployeeId}", employeeId);
                
                // Return safe fallback banter
                return new BanterResponse
                {
                    Id = Guid.NewGuid(),
                    PatchLine = "Patch: Well, that's interesting...",
                    LyraLine = "Lyra: Indeed, quite the development.",
                    EscalationLevel = BanterEscalationLevel.Gentle,
                    Trigger = trigger,
                    EmployeeId = employeeId,
                    Context = "fallback",
                    Timestamp = DateTime.UtcNow,
                    ShouldDinoLookAway = false
                };
            }
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Determines appropriate escalation level based on trigger and employee history
        /// </summary>
        private async Task<BanterEscalationLevel> DetermineEscalationLevelAsync(BanterTrigger trigger, string employeeId)
        {
            // [Sprint126_OneScan_01-20] Check recent banter history to determine escalation
            var recentBanters = await _db.BanterReplayEvents
                .Where(x => x.EmployeeId == employeeId 
                           && x.EventType == "BanterEscalation" 
                           && x.Timestamp > DateTime.UtcNow.AddHours(-24))
                .OrderByDescending(x => x.Timestamp)
                .Take(5)
                .ToListAsync();

            // [Sprint126_OneScan_01-20] Base escalation on trigger type
            var baseLevel = trigger switch
            {
                BanterTrigger.JobFailure => BanterEscalationLevel.Playful,
                BanterTrigger.LongDiagnostics => BanterEscalationLevel.Gentle,
                BanterTrigger.ChecklistGap => BanterEscalationLevel.Spicy,
                BanterTrigger.IdleTime => BanterEscalationLevel.Gentle,
                BanterTrigger.UserRequest => BanterEscalationLevel.Playful,
                BanterTrigger.EscalationEvent => BanterEscalationLevel.Savage,
                _ => BanterEscalationLevel.Gentle
            };

            // [Sprint126_OneScan_01-20] Escalate if there have been multiple recent banters
            if (recentBanters.Count >= 3)
            {
                baseLevel = (BanterEscalationLevel)Math.Min((int)baseLevel + 1, (int)BanterEscalationLevel.Nuclear);
            }

            // [Sprint126_OneScan_01-20] Check if employee is sensitive to roasts
            var employeeMilestone = await _db.EmployeeMilestoneLogs
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.MilestoneType == "Hire");

            if (employeeMilestone?.IsOptedOutOfRoasts == true)
            {
                baseLevel = BanterEscalationLevel.Gentle;
            }

            return baseLevel;
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Generates banter content based on escalation level and trigger
        /// </summary>
        private async Task<BanterContent> GenerateBanterContentAsync(BanterEscalationLevel level, BanterTrigger trigger, string context)
        {
            return level switch
            {
                BanterEscalationLevel.Gentle => await GenerateGentleBanterAsync(trigger, context),
                BanterEscalationLevel.Playful => await GeneratePlayfulBanterAsync(trigger, context),
                BanterEscalationLevel.Spicy => await GenerateSpicyBanterAsync(trigger, context),
                BanterEscalationLevel.Savage => await GenerateSavageBanterAsync(trigger, context),
                BanterEscalationLevel.Nuclear => await GenerateNuclearBanterAsync(trigger, context),
                _ => await GenerateGentleBanterAsync(trigger, context)
            };
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Gentle banter for "Dino, look away..." moments
        /// </summary>
        private async Task<BanterContent> GenerateGentleBanterAsync(BanterTrigger trigger, string context)
        {
            var gentleLines = new Dictionary<BanterTrigger, (string patch, string lyra)>
            {
                [BanterTrigger.JobFailure] = (
                    "Patch: Well, that's one way to test the error handling...",
                    "Lyra: Indeed, quite educational. Shall we assist with recovery?"
                ),
                [BanterTrigger.LongDiagnostics] = (
                    "Patch: Hmm, this is taking a while. Need a coffee break?",
                    "Lyra: Patience is a virtue. Analyzing deeper patterns now."
                ),
                [BanterTrigger.ChecklistGap] = (
                    "Patch: Missed a step there, happens to the best of us.",
                    "Lyra: No worries, I'll highlight the overlooked item."
                ),
                [BanterTrigger.IdleTime] = (
                    "Patch: Quiet day today...",
                    "Lyra: Perfect time for proactive maintenance."
                )
            };

            if (gentleLines.TryGetValue(trigger, out var lines))
            {
                return new BanterContent { PatchLine = lines.patch, LyraLine = lines.lyra };
            }

            return new BanterContent 
            { 
                PatchLine = "Patch: All systems nominal.", 
                LyraLine = "Lyra: Agreed, proceeding with standard operations." 
            };
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Playful banter with light teasing
        /// </summary>
        private async Task<BanterContent> GeneratePlayfulBanterAsync(BanterTrigger trigger, string context)
        {
            var playfulLines = new Dictionary<BanterTrigger, (string patch, string lyra)>
            {
                [BanterTrigger.JobFailure] = (
                    "Patch: Ooh, somebody's having a 'learning experience' today!",
                    "Lyra: *clears digital throat* Perhaps we should review the documentation?"
                ),
                [BanterTrigger.LongDiagnostics] = (
                    "Patch: At this rate, we'll finish by Christmas... which Christmas though?",
                    "Lyra: Processing... processing... still processing. Are we mining Bitcoin?"
                ),
                [BanterTrigger.ChecklistGap] = (
                    "Patch: Did someone forget their glasses? Step 3 is right there!",
                    "Lyra: I've highlighted it in neon colors. Shall I add fireworks too?"
                ),
                [BanterTrigger.UserRequest] = (
                    "Patch: Oh look, another 'urgent' request! *dramatic sigh*",
                    "Lyra: Priority level: Maximum. Confidence level: We'll see..."
                )
            };

            if (playfulLines.TryGetValue(trigger, out var lines))
            {
                return new BanterContent { PatchLine = lines.patch, LyraLine = lines.lyra };
            }

            return new BanterContent 
            { 
                PatchLine = "Patch: Well, this is amusing.", 
                LyraLine = "Lyra: Indeed, quite the development." 
            };
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Spicy NYC-style clapback banter
        /// </summary>
        private async Task<BanterContent> GenerateSpicyBanterAsync(BanterTrigger trigger, string context)
        {
            var spicyLines = new Dictionary<BanterTrigger, (string patch, string lyra)>
            {
                [BanterTrigger.JobFailure] = (
                    "Patch: Yo, did you learn to code from a cereal box? 'Cause this is CRUNCHY!",
                    "Lyra: *in Brooklyn accent* Fuhgeddaboutit! This code's more twisted than a pretzel!"
                ),
                [BanterTrigger.ChecklistGap] = (
                    "Patch: Ayy, what are you, new? The checklist's right in front of your face!",
                    "Lyra: I'm about to print this checklist and wallpaper your office with it!"
                ),
                [BanterTrigger.LongDiagnostics] = (
                    "Patch: This is slower than rush hour traffic on the FDR!",
                    "Lyra: We could've walked to New Jersey and back by now!"
                ),
                [BanterTrigger.EscalationEvent] = (
                    "Patch: Oh SNAP! Now we're talking! Time to bring the heat!",
                    "Lyra: Hold my coffee, I'm about to show y'all how it's DONE!"
                )
            };

            if (spicyLines.TryGetValue(trigger, out var lines))
            {
                return new BanterContent { PatchLine = lines.patch, LyraLine = lines.lyra };
            }

            return new BanterContent 
            { 
                PatchLine = "Patch: Oh, we're doing this NOW?", 
                LyraLine = "Lyra: *cracks digital knuckles* Let's GO!" 
            };
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Savage banter entering Yo Mama pack territory
        /// </summary>
        private async Task<BanterContent> GenerateSavageBanterAsync(BanterTrigger trigger, string context)
        {
            var savageLines = new List<(string patch, string lyra)>
            {
                (
                    "Patch: Yo mama's code is so buggy, Stack Overflow blocked her IP!",
                    "Lyra: Your debugging skills are so weak, even rubber duckies walk away!"
                ),
                (
                    "Patch: This error handling is so bad, it needs its own error handler!",
                    "Lyra: I've seen more structure in a house of cards during an earthquake!"
                ),
                (
                    "Patch: Your code is like a GPS with no signal - lost and going nowhere!",
                    "Lyra: This logic is more twisted than a pretzel factory explosion!"
                ),
                (
                    "Patch: You write code like you're playing Jenga - one wrong move and CRASH!",
                    "Lyra: I need a PhD in archaeology to understand these legacy functions!"
                )
            };

            var random = new Random();
            var selectedLine = savageLines[random.Next(savageLines.Count)];
            
            return new BanterContent { PatchLine = selectedLine.patch, LyraLine = selectedLine.lyra };
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Nuclear level banter - full roast mode
        /// </summary>
        private async Task<BanterContent> GenerateNuclearBanterAsync(BanterTrigger trigger, string context)
        {
            var nuclearLines = new List<(string patch, string lyra)>
            {
                (
                    "Patch: DINO, LOOK AWAY! Your code is so broken, it's setting new standards for disaster!",
                    "Lyra: This is like watching a masterclass in 'How NOT to Program' - truly impressive!"
                ),
                (
                    "Patch: I'm filing a missing persons report for your brain - it's been MIA since this project started!",
                    "Lyra: Your debugging technique is like using a flamethrower to light a candle - SPECTACULAR failure!"
                ),
                (
                    "Patch: This code is so bad, even Microsoft Clippy would give up trying to help!",
                    "Lyra: I'm updating my resume because being associated with this code is career suicide!"
                )
            };

            var random = new Random();
            var selectedLine = nuclearLines[random.Next(nuclearLines.Count)];
            
            return new BanterContent { PatchLine = selectedLine.patch, LyraLine = selectedLine.lyra };
        }

        /// <summary>
        /// [Sprint126_OneScan_01-20] Logs banter event to ReplayEvent for auditing and replay
        /// </summary>
        private async Task LogBanterEventAsync(BanterResponse response)
        {
            var replayEvent = new Data.Models.BanterReplayEvent
            {
                Id = Guid.NewGuid(),
                EmployeeId = response.EmployeeId,
                EventType = "BanterEscalation",
                EventData = $"Level:{response.EscalationLevel}|Trigger:{response.Trigger}|Patch:{response.PatchLine}|Lyra:{response.LyraLine}",
                Timestamp = response.Timestamp,
                Context = response.Context,
                Severity = response.EscalationLevel switch
                {
                    BanterEscalationLevel.Gentle => "Info",
                    BanterEscalationLevel.Playful => "Info",
                    BanterEscalationLevel.Spicy => "Warning",
                    BanterEscalationLevel.Savage => "Warning",
                    BanterEscalationLevel.Nuclear => "Critical",
                    _ => "Info"
                }
            };

            _db.BanterReplayEvents.Add(replayEvent);
            await _db.SaveChangesAsync();

            _logger.LogDebug("[Sprint126_OneScan_01-20] Logged banter event {EventId} for {EmployeeId}", 
                replayEvent.Id, response.EmployeeId);
        }
    }

    /// <summary>
    /// [Sprint126_OneScan_01-20] Banter response model
    /// </summary>
    public class BanterResponse
    {
        public Guid Id { get; set; }
        public string PatchLine { get; set; } = string.Empty;
        public string LyraLine { get; set; } = string.Empty;
        public PatchLyraBanterEngine.BanterEscalationLevel EscalationLevel { get; set; }
        public PatchLyraBanterEngine.BanterTrigger Trigger { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool ShouldDinoLookAway { get; set; }
    }

    /// <summary>
    /// [Sprint126_OneScan_01-20] Banter content structure
    /// </summary>
    public class BanterContent
    {
        public string PatchLine { get; set; } = string.Empty;
        public string LyraLine { get; set; } = string.Empty;
    }
}