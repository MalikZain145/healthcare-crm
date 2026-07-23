using System.ComponentModel.DataAnnotations;

namespace HealthcareCRM.API.Models.Dtos
{
    // Request body for PATCH /api/emergency/alerts/{alertId}/status
    public class EmergencyStatusUpdateDto
    {
        [Required, StringLength(20)]
        public string Status { get; set; } = string.Empty; // SOS | Dispatched | Resolved
    }
}
