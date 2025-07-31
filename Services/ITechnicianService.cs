using System.Collections.Generic;
using System.Threading.Tasks;
using System; // Sprint 84.7.2 — Live Filter + UI Overlay
using System.Security.Claims;
using TechnicianModel = Data.Models.Technician;
using Data.Models;

namespace Services
{
    public interface ITechnicianService
    {
        Task<List<TechnicianViewModel>> GetAllAsync();
        Task<TechnicianViewModel?> GetByIdAsync(int id);
        Task AddAsync(TechnicianModel technician);
        Task UpdateAsync(TechnicianModel technician);
        Task DeleteAsync(int id);
        // Sprint 84.7.2 — Live Filter + UI Overlay
        int GetCurrentTechnicianId(ClaimsPrincipal user);
        TechnicianModel GetTechnicianById(int id);
    }
}
