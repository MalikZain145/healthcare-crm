using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EmergencyController(AppDbContext db) => _db = db;

        // Mock location data for demonstration purposes
        private static readonly Dictionary<int, object> _mockLocations = new()
        {
            [1] = new
            {
                EmergencyId = 1,
                PatientName = "Ahmed Raza",
                Status = "Active",
                Location = new
                {
                    Latitude = 31.5204,
                    Longitude = 74.3587,
                    Address = "Lahore General Hospital, Lahore, Pakistan",
                    Floor = "2nd Floor",
                    Ward = "Emergency Ward A"
                },
                RespondingUnit = "Ambulance Unit 3",
                DispatchedAt = DateTime.UtcNow.AddMinutes(-12),
                EstimatedArrivalMinutes = 5
            },
            [2] = new
            {
                EmergencyId = 2,
                PatientName = "Sara Khan",
                Status = "Resolved",
                Location = new
                {
                    Latitude = 33.7294,
                    Longitude = 73.0931,
                    Address = "PIMS Hospital, Islamabad, Pakistan",
                    Floor = "Ground Floor",
                    Ward = "Trauma Center"
                },
                RespondingUnit = "Ambulance Unit 1",
                DispatchedAt = DateTime.UtcNow.AddHours(-2),
                EstimatedArrivalMinutes = 0
            }
        };

        /// <summary>
        /// Get the real-time mock location of an emergency case by its ID.
        /// Returns location coordinates, address, ward, and responding unit details.
        /// </summary>
        /// <param name="id">Emergency case ID</param>
        /// <returns>Emergency location details</returns>
        /// <response code="200">Location found and returned</response>
        /// <response code="404">Emergency case not found</response>
        [HttpGet("{id:int}/location")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetLocation(int id)
        {
            if (_mockLocations.TryGetValue(id, out var location))
                return Ok(location);

            // Return a generated mock for any ID not in the dictionary
            if (id > 0)
            {
                var mockData = new
                {
                    EmergencyId = id,
                    PatientName = $"Patient #{id}",
                    Status = "Active",
                    Location = new
                    {
                        Latitude = 30.3753 + (id * 0.01),
                        Longitude = 69.3451 + (id * 0.01),
                        Address = $"Mock Hospital, Room {id * 10}, Pakistan",
                        Floor = "1st Floor",
                        Ward = "Emergency Ward"
                    },
                    RespondingUnit = $"Ambulance Unit {id % 5 + 1}",
                    DispatchedAt = DateTime.UtcNow.AddMinutes(-id * 3),
                    EstimatedArrivalMinutes = Math.Max(0, 15 - id)
                };
                return Ok(mockData);
            }

            return NotFound(new { message = $"Emergency case with ID {id} not found." });
        }

        /// <summary>
        /// Trigger an SOS alert for a user. Marks the request as SOS, records the alert timestamp,
        /// and reports how many of the user's emergency contacts would be notified.
        /// </summary>
        /// <param name="id">User ID of the person triggering the SOS alert</param>
        /// <response code="200">Alert created and recorded</response>
        /// <response code="404">User not found</response>
        [HttpPost("{id:int}/notify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Notify(int id)
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
                return NotFound(new { message = $"User with ID {id} not found." });

            var contactCount = await _db.EmergencyContacts.CountAsync(c => c.UserId == id);

            var alert = new EmergencyAlert
            {
                UserId = id,
                Status = "SOS",
                TriggeredAt = DateTime.UtcNow,
                ContactsNotified = contactCount
            };

            _db.EmergencyAlerts.Add(alert);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = contactCount > 0
                    ? $"SOS alert sent. {contactCount} emergency contact(s) notified."
                    : "SOS alert recorded, but this user has no emergency contacts on file.",
                alert.Id,
                alert.UserId,
                alert.Status,
                alert.TriggeredAt,
                alert.ContactsNotified
            });
        }

        /// <summary>
        /// Track B — Push Notification Trigger. Updates the status of an existing emergency
        /// alert (e.g. SOS -> Dispatched -> Resolved). Whenever the status actually changes,
        /// an in-app push notification (bell icon) is automatically created for the affected
        /// user so they see the update in real time.
        /// </summary>
        /// <param name="alertId">ID of the emergency alert to update</param>
        /// <param name="dto">The new status: SOS, Dispatched, or Resolved</param>
        /// <response code="200">Status updated (and notification triggered if it changed)</response>
        /// <response code="400">Invalid or missing status value</response>
        /// <response code="404">Emergency alert not found</response>
        [HttpPatch("alerts/{alertId:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAlertStatus(int alertId, [FromBody] EmergencyStatusUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var allowedStatuses = new[] { "SOS", "Dispatched", "Resolved" };
            if (!allowedStatuses.Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = $"Status must be one of: {string.Join(", ", allowedStatuses)}"
                });
            }

            var alert = await _db.EmergencyAlerts.FindAsync(alertId);
            if (alert == null)
                return NotFound(new { message = $"Emergency alert with ID {alertId} not found." });

            var previousStatus = alert.Status;
            var statusChanged = !string.Equals(previousStatus, dto.Status, StringComparison.OrdinalIgnoreCase);

            alert.Status = dto.Status;
            if (string.Equals(dto.Status, "Resolved", StringComparison.OrdinalIgnoreCase) && alert.ResolvedAt == null)
                alert.ResolvedAt = DateTime.UtcNow;

            Notification? notification = null;

            // ---- Push Notification Trigger ----
            // Only fire when the status genuinely changed, so repeated calls with the
            // same status don't spam the user with duplicate notifications.
            if (statusChanged)
            {
                notification = new Notification
                {
                    UserId = alert.UserId,
                    Title = "Emergency Status Update",
                    Message = $"Your emergency alert status changed from '{previousStatus}' to '{dto.Status}'.",
                    Type = "Emergency",
                    RelatedEntityId = alert.Id
                };
                _db.Notifications.Add(notification);
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = statusChanged
                    ? $"Status updated to '{alert.Status}'. Push notification triggered for the user."
                    : $"Status is already '{alert.Status}'. No notification triggered.",
                alert.Id,
                alert.UserId,
                previousStatus,
                currentStatus = alert.Status,
                alert.ResolvedAt,
                notificationTriggered = statusChanged,
                notificationId = notification?.Id
            });
        }
    }
}
