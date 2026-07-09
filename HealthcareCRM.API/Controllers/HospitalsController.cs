using HealthcareCRM.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public HospitalsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Get all hospitals (unsorted).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Hospitals
                .OrderBy(h => h.Name)
                .Select(h => new { h.Id, h.Name, h.Address, h.City, h.PhoneNumber, h.Latitude, h.Longitude })
                .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get all hospitals sorted by distance from the caller's real, current location
        /// (nearest first). Latitude and longitude MUST be supplied by the client from the
        /// device's actual GPS/geolocation (e.g. the browser's navigator.geolocation API) —
        /// this endpoint intentionally does not fall back to any hardcoded/default location,
        /// so both parameters are required.
        /// </summary>
        /// <param name="latitude">Caller's current latitude (-90 to 90), from the device's real geolocation</param>
        /// <param name="longitude">Caller's current longitude (-180 to 180), from the device's real geolocation</param>
        /// <response code="200">Hospitals returned, nearest first, each with its distance in km</response>
        /// <response code="400">Latitude/longitude missing or out of valid range</response>
        [HttpGet("nearby")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNearby([FromQuery] double? latitude, [FromQuery] double? longitude)
        {
            if (latitude is null || longitude is null)
            {
                return BadRequest(new
                {
                    message = "latitude and longitude are required query parameters. " +
                               "Send the device's real current coordinates (e.g. from navigator.geolocation in the browser) — " +
                               "a hardcoded/default location is not accepted."
                });
            }

            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                return BadRequest(new { message = "latitude must be between -90 and 90, longitude between -180 and 180." });

            var hospitals = await _db.Hospitals.ToListAsync();

            var results = hospitals
                .Select(h => new
                {
                    h.Id,
                    h.Name,
                    h.Address,
                    h.City,
                    h.PhoneNumber,
                    h.Latitude,
                    h.Longitude,
                    DistanceKm = Math.Round(HaversineDistanceKm(latitude.Value, longitude.Value, h.Latitude, h.Longitude), 2)
                })
                .OrderBy(h => h.DistanceKm)
                .ToList();

            return Ok(results);
        }

        /// <summary>
        /// Great-circle distance between two lat/long points in kilometers (Haversine formula).
        /// </summary>
        private static double HaversineDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371.0;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        private static double ToRadians(double deg) => deg * Math.PI / 180.0;
    }
}
