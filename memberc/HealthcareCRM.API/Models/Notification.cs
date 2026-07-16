using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    // In-app notification record shown to a user (bell icon / notification panel).
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Type { get; set; } = "General"; // e.g. Reminder | Prescription | Emergency | General

        // Optional pointer to whatever record this notification relates to
        // (e.g. an AppointmentId or PrescriptionId), so the frontend can navigate to it.
        public int? RelatedEntityId { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
