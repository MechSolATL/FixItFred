using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Data.Models;

namespace MVP_Core.Services
{
    public interface ITechnicianService
    {
        Task<List<TechnicianViewModel>> GetAllAsync();
        Task<TechnicianViewModel?> GetByIdAsync(int id);
        Task AddAsync(Technician technician);
        Task UpdateAsync(Technician technician);
        Task DeleteAsync(int id);
    }
}
