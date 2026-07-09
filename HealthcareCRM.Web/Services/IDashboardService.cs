using HealthcareCRM.Web.Models.ViewModels;

namespace HealthcareCRM.Web.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetStatsAsync();
        Task<DashboardViewModel> GetStatsForDoctorAsync(int doctorId);
    }
}
