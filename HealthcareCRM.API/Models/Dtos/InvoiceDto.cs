using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class InvoiceDto
    {
        [Required] public int PatientId { get; set; }
        public int? AppointmentId { get; set; }
        [Required, Range(0, 1000000)] public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Unpaid";
        public DateTime IssuedDate { get; set; } = DateTime.Today;
    }
}
