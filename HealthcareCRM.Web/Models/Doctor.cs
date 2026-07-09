using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.Web.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Specialization { get; set; } = string.Empty;

        [EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
