using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;

namespace HealthcareCRM.Web.Services
{
    public interface IDoctorService
    {
        Task<List<Doctor>> GetAllAsync(string? search = null);
        Task<List<Doctor>> GetActiveAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor?> GetByEmailAsync(string email);
        Task<Doctor> CreateAsync(DoctorViewModel model);
        Task<Doctor?> UpdateAsync(int id, DoctorViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<int> CountAsync();
    }
}
