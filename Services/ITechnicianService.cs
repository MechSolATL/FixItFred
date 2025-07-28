using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Data.Models;
using System; // Sprint 84.7.2 — Live Filter + UI Overlay
using System.Security.Claims;
using TechnicianModel = MVP_Core.Data.Models.Technician;

namespace MVP_Core.Services
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
