using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Services
{
    public interface IPrescriptionService
    {
        Task<List<Prescription>> GetByAppointmentAsync(int appointmentId);
        Task<Prescription?> GetByIdAsync(int id);
        Task<Prescription> CreateAsync(Prescription prescription);
        Task<Prescription?> UpdateAsync(int id, Prescription prescription);
        Task<bool> DeleteAsync(int id);
    }
}