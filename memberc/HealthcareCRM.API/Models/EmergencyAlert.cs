using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    // Records an SOS alert triggered by a user, with the timestamp it was raised.
    public class EmergencyAlert
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "SOS"; // SOS | Resolved

        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        // How many emergency contacts were notified as part of this alert.
        public int ContactsNotified { get; set; }
    }
}
