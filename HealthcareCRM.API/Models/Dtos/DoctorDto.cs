using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class DoctorDto
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public string Specialization { get; set; } = string.Empty;
        [EmailAddress] public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
