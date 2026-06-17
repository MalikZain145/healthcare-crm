using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.Web.Models.ViewModels
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a patient")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Display(Name = "Appointment (optional)")]
        public int? AppointmentId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, 1000000, ErrorMessage = "Enter a valid amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Unpaid";

        [Display(Name = "Issued Date")]
        [DataType(DataType.Date)]
        public DateTime IssuedDate { get; set; } = DateTime.Today;
    }
}
