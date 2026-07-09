using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class InvoiceDto
    {
        [Required] public int PatientId { get; set; }

        // Required — every invoice must be linked to a real appointment (no orphan invoices).
        [Required] public int AppointmentId { get; set; }

        [Required, Range(0, 1000000)] public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Unpaid";
        public DateTime IssuedDate { get; set; } = DateTime.Today;
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);
    }

    // Used by POST /api/billing/generate — creates an invoice directly from an AppointmentId.
    // PatientId is derived automatically from the appointment, so the link can never be orphaned.
    public class GenerateInvoiceDto
    {
        [Required] public int AppointmentId { get; set; }
        [Required, Range(0, 1000000)] public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }

    // Used by PATCH /api/billing/{id}/pay — optional body to record payment details.
    public class MarkPaidDto
    {
        public decimal? AmountPaid { get; set; }
        [StringLength(30)] public string PaymentMethod { get; set; } = "Cash";
    }
}
