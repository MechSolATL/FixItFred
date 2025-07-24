using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task AddAsync(Technician technician)
        {
            _db.Technicians.Add(technician);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("TechnicianAdded", technician.Id, technician.FullName);
        }

        public async Task UpdateAsync(Technician technician)
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
    }
}
