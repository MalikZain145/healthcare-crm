using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUnreadAsync(int userId);

        Task<bool> MarkAsReadAsync(int id);
    }
}