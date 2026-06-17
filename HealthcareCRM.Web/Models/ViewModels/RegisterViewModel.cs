using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required, StringLength(120)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
