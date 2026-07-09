using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [StringLength(5)]
        public string BloodType { get; set; } = string.Empty;

        // The doctor who created/owns this patient (null for self-registered patients).
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
