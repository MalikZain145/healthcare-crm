using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Services
{
    public interface IPatientService
    {
        Task<List<Patient>> GetAllPatientsAsync(string? searchTerm = null);
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient> CreatePatientAsync(PatientViewModel model);
        Task<Patient?> UpdatePatientAsync(int id, PatientViewModel model);
        Task<bool> DeletePatientAsync(int id);
    }
}