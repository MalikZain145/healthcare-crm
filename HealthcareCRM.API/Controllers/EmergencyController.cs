using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyController : ControllerBase
    {
        // Mock location data for demonstration purposes
        private static readonly Dictionary<int, object> _mockLocations = new()
        {
            [1] = new
            {
                EmergencyId = 1,
                PatientName = "Ahmed Raza",
                Status = "Active",
                Location = new
                {
                    Latitude = 31.5204,
                    Longitude = 74.3587,
                    Address = "Lahore General Hospital, Lahore, Pakistan",
                    Floor = "2nd Floor",
                    Ward = "Emergency Ward A"
                },
                RespondingUnit = "Ambulance Unit 3",
                DispatchedAt = DateTime.UtcNow.AddMinutes(-12),
                EstimatedArrivalMinutes = 5
            },
            [2] = new
            {
                EmergencyId = 2,
                PatientName = "Sara Khan",
                Status = "Resolved",
                Location = new
                {
                    Latitude = 33.7294,
                    Longitude = 73.0931,
                    Address = "PIMS Hospital, Islamabad, Pakistan",
                    Floor = "Ground Floor",
                    Ward = "Trauma Center"
                },
                RespondingUnit = "Ambulance Unit 1",
                DispatchedAt = DateTime.UtcNow.AddHours(-2),
                EstimatedArrivalMinutes = 0
            }
        };

        /// <summary>
        /// Get the real-time mock location of an emergency case by its ID.
        /// Returns location coordinates, address, ward, and responding unit details.
        /// </summary>
        /// <param name="id">Emergency case ID</param>
        /// <returns>Emergency location details</returns>
        /// <response code="200">Location found and returned</response>
        /// <response code="404">Emergency case not found</response>
        [HttpGet("{id:int}/location")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetLocation(int id)
        {
            if (_mockLocations.TryGetValue(id, out var location))
                return Ok(location);

            // Return a generated mock for any ID not in the dictionary
            if (id > 0)
            {
                var mockData = new
                {
                    EmergencyId = id,
                    PatientName = $"Patient #{id}",
                    Status = "Active",
                    Location = new
                    {
                        Latitude = 30.3753 + (id * 0.01),
                        Longitude = 69.3451 + (id * 0.01),
                        Address = $"Mock Hospital, Room {id * 10}, Pakistan",
                        Floor = "1st Floor",
                        Ward = "Emergency Ward"
                    },
                    RespondingUnit = $"Ambulance Unit {id % 5 + 1}",
                    DispatchedAt = DateTime.UtcNow.AddMinutes(-id * 3),
                    EstimatedArrivalMinutes = Math.Max(0, 15 - id)
                };
                return Ok(mockData);
            }

            return NotFound(new { message = $"Emergency case with ID {id} not found." });
        }
    }
}
