using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;

namespace HealthcareCRM.Web.Services
{
    public interface IPatientService
    {
        // doctorId == null  => all patients (admin).  doctorId set => only that doctor's patients.
        Task<List<Patient>> GetAllAsync(string? search = null, int? doctorId = null);
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByEmailAsync(string email);
        Task<Patient> CreateAsync(PatientViewModel model, int? doctorId = null);
        Task<Patient?> UpdateAsync(int id, PatientViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<int> CountAsync(int? doctorId = null);
    }
}
