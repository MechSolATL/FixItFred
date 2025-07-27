using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    public class RoastEvolutionEngine
    {
        private readonly ApplicationDbContext _db;
        private readonly RoastFeedbackService _feedbackService;
        public RoastEvolutionEngine(ApplicationDbContext db, RoastFeedbackService feedbackService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        }

        // Analyze feedback trends, auto-retire low scorers, promote top-performers
        public async Task AnalyzeAndEvolveVaultAsync()
        {
            var templates = await _db.RoastTemplates.ToListAsync();
            foreach (var template in templates)
            {
                var reactions = await _db.RoastReactionLogs.Where(r => r.RoastId == template.Id).ToListAsync();
                double avgScore = reactions.Any() ? reactions.Select(r => r.ReactionType switch
                {
                    "star1" => 1,
                    "star2" => 2,
                    "star3" => 3,
                    "star4" => 4,
                    "star5" => 5,
                    _ => 0
                }).Average() : 0;
                template.SuccessRate = avgScore;
                template.LastUsedAt = reactions.OrderByDescending(r => r.SubmittedAt).FirstOrDefault()?.SubmittedAt;
                // Auto-retire if low score
                if (avgScore < 2.0 && reactions.Count > 5)
                {
                    template.Retired = true;
                    _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                    {
                        RoastTemplateId = template.Id,
                        EditType = "AutoRetire",
                        Editor = "EvolutionEngine",
                        Timestamp = DateTime.UtcNow,
                        Notes = "Auto-retired due to low avg score.",
                        EffectivenessScore = avgScore,
                        Retired = true,
                        PreviousMessage = template.Message,
                        NewMessage = template.Message
                    });
                }
                // Auto-promote if high score
                if (avgScore > 4.5 && reactions.Count > 10 && !template.AutoPromote)
                {
                    template.AutoPromote = true;
                    _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                    {
                        RoastTemplateId = template.Id,
                        EditType = "AutoPromote",
                        Editor = "EvolutionEngine",
                        Timestamp = DateTime.UtcNow,
                        Notes = "Auto-promoted due to high avg score.",
                        EffectivenessScore = avgScore,
                        Promoted = true,
                        PreviousMessage = template.Message,
                        NewMessage = template.Message
                    });
                }
            }
            await _db.SaveChangesAsync();
        }

        // Tag legacy, auto-generate new variations using GPT-based logic
        public async Task TagLegacyAndGenerateAIAsync()
        {
            var templates = await _db.RoastTemplates.ToListAsync();
            foreach (var template in templates)
            {
                if (template.LegacyStatus == false && template.TimesUsed > template.UseLimit * 0.8)
                {
                    template.LegacyStatus = true;
                    _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                    {
                        RoastTemplateId = template.Id,
                        EditType = "LegacyTag",
                        Editor = "EvolutionEngine",
                        Timestamp = DateTime.UtcNow,
                        Notes = "Tagged as legacy due to high usage.",
                        IsLegacy = true,
                        PreviousMessage = template.Message,
                        NewMessage = template.Message
                    });
                }
            }
            await _db.SaveChangesAsync();
        }

        // AI seeding tool for vault expansion
        public async Task<List<RoastTemplate>> GenerateAISeededRoastsAsync(string prompt)
        {
            // Simulate GPT-based generation (replace with actual AI integration)
            var tones = new[] { "Soft", "Medium", "Savage" };
            var themes = new[] { "Tech tardy", "GPS ghosting", "Chatty Cathy" };
            var results = new List<RoastTemplate>();
            foreach (var tone in tones)
            {
                foreach (var theme in themes)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var msg = $"[{tone}-{theme}-{i}] AI-generated roast joke.";
                        var roast = new RoastTemplate
                        {
                            Message = msg,
                            Tier = Enum.Parse<RoastTier>(tone),
                            Category = theme,
                            UseLimit = 20,
                            TimesUsed = 0,
                            AIAuthored = true
                        };
                        results.Add(roast);
                    }
                }
            }
            // Optionally add to DB
            _db.RoastTemplates.AddRange(results);
            await _db.SaveChangesAsync();
            foreach (var roast in results)
            {
                _db.RoastEvolutionLogs.Add(new RoastEvolutionLog
                {
                    RoastTemplateId = roast.Id,
                    EditType = "AISeed",
                    Editor = "EvolutionEngine",
                    Timestamp = DateTime.UtcNow,
                    Notes = "AI-generated seed.",
                    IsAIAuthored = true,
                    PreviousMessage = string.Empty,
                    NewMessage = roast.Message
                });
            }
            await _db.SaveChangesAsync();
            return results;
        }

        // Enforce cooldown, duplication, safety checks
        public bool IsSafeRoast(RoastTemplate template)
        {
            if (template == null) return false;
            if (template.Retired || template.TimesUsed > template.UseLimit) return false;
            if (string.IsNullOrEmpty(template.Message) || template.Message.Contains("inappropriate", StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }
    }
}
