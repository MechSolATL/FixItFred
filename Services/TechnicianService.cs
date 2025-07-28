using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Sprint 84.7.2 — Live Filter + UI Overlay
using System.Security.Claims;
using TechnicianModel = MVP_Core.Data.Models.Technician;

namespace MVP_Core.Services
{
    public class TechnicianService : ITechnicianService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<RequestHub> _hub;
        public TechnicianService(ApplicationDbContext db, IHubContext<RequestHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        public async Task<List<TechnicianViewModel>> GetAllAsync()
        {
            var technicians = await _db.Technicians.ToListAsync();
            var jobs = await _db.ServiceRequests.ToListAsync();
            return technicians.Select(t => new TechnicianViewModel
            {
                Id = t.Id,
                FullName = t.FullName,
                IsActive = t.IsActive,
                Email = t.Email,
                Phone = t.Phone,
                Specialty = t.Specialty,
                AssignedJobsCount = jobs.Count(r => r.AssignedTo == t.FullName)
            }).OrderBy(t => t.FullName).ToList();
        }

        public async Task<TechnicianViewModel?> GetByIdAsync(int id)
        {
            var t = await _db.Technicians.FindAsync(id);
            if (t == null) return null;
            var jobsCount = await _db.ServiceRequests.CountAsync(r => r.AssignedTo == t.FullName);
            return new TechnicianViewModel
            {
                Id = t.Id,
                FullName = t.FullName,
                IsActive = t.IsActive,
                Email = t.Email,
                Phone = t.Phone,
                Specialty = t.Specialty,
                AssignedJobsCount = jobsCount
            };
        }

        public async Task AddAsync(TechnicianModel technician)
        {
            _db.Technicians.Add(technician);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("TechnicianAdded", technician.Id, technician.FullName);
        }

        public async Task UpdateAsync(TechnicianModel technician)
        {
            _db.Technicians.Update(technician);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("TechnicianUpdated", technician.Id, technician.FullName);
        }

        public async Task DeleteAsync(int id)
        {
            var tech = await _db.Technicians.FindAsync(id);
            if (tech != null)
            {
                _db.Technicians.Remove(tech);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("TechnicianRemoved", tech.Id, tech.FullName);
            }
        }

        // Sprint 84.7.2 — Live Filter + UI Overlay
        public int GetCurrentTechnicianId(ClaimsPrincipal user)
        {
            // Assumes claim type "TechnicianId" is present
            var idClaim = user?.Claims?.FirstOrDefault(c => c.Type == "TechnicianId");
            if (idClaim != null && int.TryParse(idClaim.Value, out int id))
                return id;
            throw new Exception("TechnicianId claim not found");
        }

        public TechnicianModel GetTechnicianById(int id)
        {
            return _db.Technicians.FirstOrDefault(t => t.Id == id) ?? new TechnicianModel();
        }
    }
}
