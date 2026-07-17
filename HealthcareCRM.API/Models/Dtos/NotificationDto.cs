using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    public class NotificationDto
    {
        [Required] public int UserId { get; set; }
        [Required, StringLength(150)] public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "General";
        public int? RelatedEntityId { get; set; }
    }
}
