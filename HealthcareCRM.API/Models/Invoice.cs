using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

<<<<<<< HEAD
        // Every invoice MUST be linked to a real, existing appointment.
        // No orphan invoices are allowed (enforced here + at the FK level in AppDbContext).
        [Required]
        public int AppointmentId { get; set; }
=======
        // Optional link to an appointment this invoice is for.
        public int? AppointmentId { get; set; }
>>>>>>> 5ad68447cafaeb1f8dd4a3710b689cbaf6afa0ca
        public Appointment? Appointment { get; set; }

        [Required, Range(0, 1000000)]
        public decimal Amount { get; set; }

        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Status { get; set; } = "Unpaid"; // Unpaid | Paid

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
<<<<<<< HEAD

        // Used to compute the "Overdue" filter (Unpaid + DueDate in the past).
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);

        // Set the moment the invoice is marked as paid.
        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
=======
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
>>>>>>> 5ad68447cafaeb1f8dd4a3710b689cbaf6afa0ca
    }
}
