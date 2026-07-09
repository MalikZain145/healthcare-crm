using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [StringLength(250)]
        public string Reason { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled | Completed | Cancelled

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
