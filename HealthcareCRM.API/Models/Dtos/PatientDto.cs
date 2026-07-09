using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class PatientDto
    {
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        [Required] public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;
        public int? DoctorId { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
