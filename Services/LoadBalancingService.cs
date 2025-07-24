using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services
{
    public class LoadBalancingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IOptions<LoadBalancingConfig> _config;
        public LoadBalancingService(ApplicationDbContext db, IOptions<LoadBalancingConfig> config)
        {
            _db = db;
            _config = config;
        }

        public Technician? GetLeastLoadedTechnician(string serviceType)
        {
            return _db.Technicians
                .Where(t => t.IsActive && (string.IsNullOrEmpty(serviceType) || t.Specialty == null || t.Specialty == serviceType))
                .OrderBy(t => t.CurrentJobCount)
                .ThenBy(t => t.Id)
                .FirstOrDefault();
        }

        public Technician? GetBestTechnician(ServiceRequest request)
        {
            var techs = _db.Technicians
                .Where(t => t.IsActive && (string.IsNullOrEmpty(request.ServiceType) || t.Specialty == null || t.Specialty == request.ServiceType))
                .ToList();
            Technician? bestTech = null;
            double bestScore = double.MinValue;
            foreach (var tech in techs)
            {
                var score = CalculateTechScore(tech, request);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestTech = tech;
                }
            }
            return bestTech;
        }

        public (Technician? BestMatch, double ConfidenceScore) GetBestTechnicianWithScore(ServiceRequest request)
        {
            // Try to use navigation property if available, else fallback to join table
            var technicians = _db.Technicians
                .ToList();
            Technician? bestMatch = null;
            double bestScore = double.MinValue;
            foreach (var tech in technicians)
            {
                double score = CalculateTechScore(tech, request);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = tech;
                }
            }
            return (bestMatch, Math.Round(bestScore * 100, 2));
        }

        public double CalculateTechScore(Technician tech, ServiceRequest request)
        {
            // Skill match weight 0.7, load balance 0.3
            var reqSkills = (request.RequiredSkills ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var techSkillNames = _db.TechnicianSkillMaps
                .Where(m => m.TechnicianId == tech.Id)
                .Select(m => m.Skill != null ? m.Skill.Name : null)
                .Where(n => n != null)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            double skillScore = reqSkills.Length == 0 ? 1.0 : reqSkills.Count(s => techSkillNames.Contains(s)) / (double)Math.Max(1, reqSkills.Length);
            double loadScore = tech.MaxJobCapacity > 0 ? 1 - ((double)tech.CurrentJobCount / tech.MaxJobCapacity) : 1.0;
            return (skillScore * 0.7) + (loadScore * 0.3);
        }

        public async Task<Technician?> AutoAssignTechnicianAsync(ServiceRequest req)
        {
            var tech = GetBestTechnician(req);
            if (tech == null) return null;
            tech.CurrentJobCount++;
            _db.TechnicianLoadLogs.Add(new TechnicianLoadLog
            {
                TechnicianId = tech.Id,
                JobId = req.Id,
                AssignedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return tech;
        }
    }
}
