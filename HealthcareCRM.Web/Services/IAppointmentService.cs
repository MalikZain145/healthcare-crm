using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;

namespace HealthcareCRM.Web.Services
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAllAsync(string? search = null, string? status = null, int? doctorId = null, DateTime? date = null);
        Task<List<Appointment>> GetByPatientAsync(int patientId);
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> CreateAsync(AppointmentViewModel model);
        Task<Appointment?> UpdateAsync(int id, AppointmentViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<int> CountAsync();
    }
}
