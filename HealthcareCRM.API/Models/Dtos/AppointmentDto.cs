using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class AppointmentDto
    {
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId { get; set; }
        [Required] public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
        public string Notes { get; set; } = string.Empty;
    }
}
