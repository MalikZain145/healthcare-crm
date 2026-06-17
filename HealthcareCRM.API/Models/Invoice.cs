using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        // Optional link to an appointment this invoice is for.
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [Required, Range(0, 1000000)]
        public decimal Amount { get; set; }

        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = "Unpaid"; // Unpaid | Paid

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
