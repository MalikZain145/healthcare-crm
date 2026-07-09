using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    public class Payment
    {
        public int Id { get; set; }

        // Every payment MUST belong to a real invoice.
        [Required]
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        [Required, Range(0, 1000000)]
        public decimal AmountPaid { get; set; }

        [Required, StringLength(30)]
        public string PaymentMethod { get; set; } = "Cash"; // Cash | Card | BankTransfer | Online

        [Required, StringLength(20)]
        public string Status { get; set; } = "Completed"; // Completed | Failed | Refunded

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
