using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // Used by the notification bell dropdown (topbar) to poll unread items.
        [HttpGet]
        public async Task<IActionResult> GetUnreadJson()
        {
            // Temporary user id
            int userId = 1;

            try
            {
                var notifications = await _notificationService.GetUnreadAsync(userId);
                var items = notifications.Select(n => new
                {
                    id = n.Id,
                    title = n.Title,
                    message = n.Message,
                    type = n.Type,
                    relatedEntityId = n.RelatedEntityId,
                    createdAt = n.CreatedAt
                });
                return Json(new { count = notifications.Count, items });
            }
            catch (Exception)
            {
                return Json(new { count = 0, items = Array.Empty<object>() });
            }
        }

        // Used by the notification bell dropdown to mark a single item as read
        // without reloading the page (unlike MarkAsRead above, used by the full list page).
        [HttpPost]
        public async Task<IActionResult> MarkAsReadAjax(int id)
        {
            var success = await _notificationService.MarkAsReadAsync(id);
            return Json(new { success });
        }
    }
}