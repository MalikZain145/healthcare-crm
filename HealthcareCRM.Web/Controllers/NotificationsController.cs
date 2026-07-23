using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            // Temporary user id
            int userId = 1;

            try
            {
                var notifications = await _notificationService.GetUnreadAsync(userId);
                return View(notifications);
            }
            catch (Exception)
            {
                ViewBag.LoadError = true;
                return View(new List<Notification>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}