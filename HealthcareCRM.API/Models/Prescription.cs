using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    // A prescription written against a specific appointment.
    public class Prescription
    {
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [Required, StringLength(200)]
        public string MedicineName { get; set; } = string.Empty;

        [StringLength(100)]
        public string Dosage { get; set; } = string.Empty; // e.g. "500mg"

        [StringLength(100)]
        public string Frequency { get; set; } = string.Empty; // e.g. "Twice a day"

        [StringLength(100)]
        public string Duration { get; set; } = string.Empty; // e.g. "7 days"

        [StringLength(500)]
        public string Instructions { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
