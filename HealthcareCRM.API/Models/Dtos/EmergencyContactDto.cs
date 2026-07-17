using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class EmergencyContactDto
    {
        [Required] public int UserId { get; set; }
        [Required, StringLength(120)] public string ContactName { get; set; } = string.Empty;
        [Required, Phone, StringLength(30)] public string PhoneNumber { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
    }
}
