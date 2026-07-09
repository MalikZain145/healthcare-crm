using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.Web.Models
{
    // Application user for authentication (staff / admin who log into the CRM).
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        // PBKDF2 hash (salt is embedded). Never store plain passwords.
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Role { get; set; } = "Patient"; // Admin | Doctor | Patient

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
