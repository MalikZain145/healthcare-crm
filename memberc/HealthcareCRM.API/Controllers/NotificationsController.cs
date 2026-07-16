using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public NotificationsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Get unread notifications for a user (bell icon feed). Requires userId.
        /// </summary>
        /// <param name="userId">The user whose unread notifications should be returned</param>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int userId)
        {
            if (!await _db.Users.AnyAsync(u => u.Id == userId))
                return NotFound(new { message = "User not found" });

            var list = await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    n.Id,
                    n.UserId,
                    n.Title,
                    n.Message,
                    n.Type,
                    n.RelatedEntityId,
                    n.IsRead,
                    n.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Get a single notification by ID.
        /// </summary>
        /// <param name="id">Notification ID</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n == null) return NotFound(new { message = "Notification not found" });
            return Ok(n);
        }

        /// <summary>
        /// Create a new in-app notification for a user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _db.Users.AnyAsync(u => u.Id == dto.UserId))
                return BadRequest(new { message = "Invalid UserId" });

            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                Type = dto.Type,
                RelatedEntityId = dto.RelatedEntityId
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = notification.Id }, notification);
        }

        /// <summary>
        /// Mark a notification as read.
        /// </summary>
        /// <param name="id">Notification ID</param>
        [HttpPatch("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n == null) return NotFound(new { message = "Notification not found" });

            if (!n.IsRead)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return Ok(new { message = "Notification marked as read", n.Id, n.IsRead, n.ReadAt });
        }
    }
}
