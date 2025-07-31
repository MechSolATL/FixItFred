using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using TechnicianModel = Data.Models.Technician;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class TechnicianSkillMatrixService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianSkillMatrixService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TechnicianSkillMatrix>> GetSkillsByTechnicianAsync(int techId)
        {
            return await _db.TechnicianSkillMatrices
                .Where(x => x.TechnicianId == techId)
                .OrderByDescending(x => x.ProficiencyLevel)
                .ToListAsync();
        }

        public async Task UpdateSkillMatrixAsync(TechnicianSkillMatrix entry)
        {
            entry.LastUpdated = DateTime.UtcNow;
            var existing = await _db.TechnicianSkillMatrices
                .FirstOrDefaultAsync(x => x.Id == entry.Id);
            if (existing != null)
            {
                existing.SkillTag = entry.SkillTag;
                existing.ProficiencyLevel = entry.ProficiencyLevel;
                existing.ExperienceYears = entry.ExperienceYears;
                existing.LastUpdated = entry.LastUpdated;
                _db.TechnicianSkillMatrices.Update(existing);
            }
            else
            {
                _db.TechnicianSkillMatrices.Add(entry);
            }
            await _db.SaveChangesAsync();
        }

        public async Task<List<TechnicianModel>> GetBestMatchTechniciansAsync(string requiredSkill, int minLevel)
        {
            var techIds = await _db.TechnicianSkillMatrices
                .Where(x => x.SkillTag == requiredSkill && x.ProficiencyLevel >= minLevel)
                .Select(x => x.TechnicianId)
                .Distinct()
                .ToListAsync();
            return await _db.Technicians
                .Where(t => techIds.Contains(t.Id) && t.IsActive)
                .ToListAsync();
        }
    }
}
