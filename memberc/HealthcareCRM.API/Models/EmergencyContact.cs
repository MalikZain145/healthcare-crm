using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models
{
    // A contact (family member, friend, etc.) that gets notified when a user triggers an SOS alert.
    public class EmergencyContact
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required, StringLength(120)]
        public string ContactName { get; set; } = string.Empty;

        [Required, Phone, StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string Relationship { get; set; } = string.Empty; // e.g. Spouse, Parent, Sibling, Friend

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
