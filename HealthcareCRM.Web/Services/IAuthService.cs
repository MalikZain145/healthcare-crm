using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;

namespace HealthcareCRM.Web.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? Error)> RegisterAsync(RegisterViewModel model);
        Task<User?> ValidateAsync(string email, string password);
    }
}
