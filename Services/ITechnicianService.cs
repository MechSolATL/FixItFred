using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Data.Models;
using System; // Sprint 84.7.2 — Live Filter + UI Overlay
using System.Security.Claims;

namespace MVP_Core.Services
{
    public interface ITechnicianService
    {
        Task<List<TechnicianViewModel>> GetAllAsync();
        Task<TechnicianViewModel?> GetByIdAsync(int id);
        Task AddAsync(Technician technician);
        Task UpdateAsync(Technician technician);
        Task DeleteAsync(int id);
        // Sprint 84.7.2 — Live Filter + UI Overlay
        int GetCurrentTechnicianId(ClaimsPrincipal user);
        Technician GetTechnicianById(int id);
    }
}
