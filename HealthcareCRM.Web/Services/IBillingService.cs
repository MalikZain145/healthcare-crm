using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;

namespace HealthcareCRM.Web.Services
{
    public interface IBillingService
    {
        Task<List<Invoice>> GetAllAsync(string? search = null, string? status = null);
        Task<List<Invoice>> GetByPatientAsync(int patientId);
        Task<Invoice?> GetByIdAsync(int id);
        Task<Invoice> CreateAsync(InvoiceViewModel model);
        Task<Invoice?> UpdateAsync(int id, InvoiceViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> MarkPaidAsync(int id);
    }
}
