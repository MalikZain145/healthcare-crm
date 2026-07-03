using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.Web.Models.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a patient")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Date & time is required")]
        [Display(Name = "Date & Time")]
        public DateTime AppointmentDate { get; set; } = DateTime.Now.AddDays(1);

        [Display(Name = "Reason")]
        public string Reason { get; set; } = string.Empty;

      
        public string Status { get; set; } = "Scheduled";

        public string? Notes { get; set; }
    }
}
