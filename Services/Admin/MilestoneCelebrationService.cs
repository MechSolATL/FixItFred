using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    public class MilestoneCelebrationService
    {
        public readonly ApplicationDbContext _db;
        public MilestoneCelebrationService(ApplicationDbContext db)
        {
            _db = db;
        }
        // Checks for upcoming milestones and broadcasts funny messages
        public async Task CheckAndBroadcastMilestonesAsync()
        {
            var today = DateTime.UtcNow.Date;
            var milestones = await _db.EmployeeMilestoneLogs.Where(m => m.DateRecognized.Date == today && !m.Broadcasted).ToListAsync();
            foreach (var milestone in milestones)
            {
                // Simulate broadcast (set Broadcasted true)
                milestone.Broadcasted = true;
                milestone.CustomMessage = GenerateFunnyMessages(milestone.MilestoneType, milestone.EmployeeId).FirstOrDefault() ?? "Happy Milestone!";
            }
            await _db.SaveChangesAsync();
        }
        // Generate funny messages for milestone type
        public List<string> GenerateFunnyMessages(string type, string name, int? years = null)
        {
            var messages = new List<string>();
            if (type == "Birthday")
            {
                messages.Add($"?? Happy Birthday, {name}! May your coffee be strong and your meetings be short.");
                messages.Add($"?? {name}, another year wiser (or just older?)!");
                messages.Add($"?? {name}, cake calories don’t count today!");
            }
            else if (type == "Anniversary")
            {
                messages.Add($"?? Congrats {name} on {years ?? 1} year(s) of surviving us!");
                messages.Add($"?? {name}, {years ?? 1} years of legendary service!");
                messages.Add($"?? {name}, thanks for {years ?? 1} years of putting up with our jokes!");
            }
            else
            {
                messages.Add($"?? {name}, milestone unlocked!");
            }
            return messages;
        }
    }
}
