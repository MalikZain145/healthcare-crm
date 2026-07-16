using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class PrescriptionDto
    {
        [Required] public int AppointmentId { get; set; }
        [Required, StringLength(200)] public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }
}
