using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;

        public NotificationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Notification>> GetUnreadAsync(int userId)
        {
            return await _db.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var notification = await _db.Set<Notification>().FindAsync(id);

            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }
    }
}