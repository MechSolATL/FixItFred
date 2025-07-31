using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Admin;
using System.Collections.Generic;
using System.Linq;

namespace Pages.Pulse
{
    public class IntegrityFormModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IntegrityScoringService _scoringService;
        public List<string> Questions { get; set; } = new()
        {
            "I prefer clear rules over flexible guidelines.",
            "I feel comfortable challenging authority when needed.",
            "I often find ways to avoid direct responsibility.",
            "I am confident in my decisions, even under pressure.",
            "I reflect on my actions to improve self-awareness.",
            "I am willing to take calculated risks.",
            "I value hierarchy and clear reporting lines.",
            "I like to closely monitor team members' work.",
            "I believe integrity is the foundation of teamwork.",
            "I trust my colleagues to do their part.",
            "I empathize with others' perspectives.",
            "I enjoy collaborating on group projects.",
            "I get defensive when criticized.",
            "I welcome feedback to improve my work.",
            "I adapt quickly to new situations."
        };
        public List<List<string>> Options { get; set; } = new()
        {
            new() { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" },
            new() { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Very Willing", "Willing", "Neutral", "Unwilling", "Very Unwilling" },
            new() { "Very Important", "Important", "Neutral", "Unimportant", "Very Unimportant" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" },
            new() { "Always", "Often", "Sometimes", "Rarely", "Never" }
        };
        [BindProperty]
        public List<string> Answers { get; set; } = new();
        public bool Submitted { get; set; }

        public IntegrityFormModel(ApplicationDbContext db, IntegrityScoringService scoringService)
        {
            _db = db;
            _scoringService = scoringService;
        }

        public void OnGet()
        {
            Submitted = false;
        }

        public IActionResult OnPost()
        {
            if (Answers == null || Answers.Count != Questions.Count)
            {
                ModelState.AddModelError("", "Please answer all questions.");
                return Page();
            }
            // Get current employee ID (stub: replace with actual user context)
            int employeeId = 1; // TODO: Replace with actual employee lookup
            var profile = _scoringService.GenerateInitialRiskProfile(employeeId, Answers);
            _db.Set<EmployeeOnboardingProfile>().Add(profile);
            // Mark employee as having completed survey (stub)
            // TODO: Update Employees table HasCompletedIntegritySurvey
            _db.SaveChanges();
            Submitted = true;
            return Page();
        }
    }
}
