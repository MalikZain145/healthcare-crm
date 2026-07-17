using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PrescriptionsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Get prescriptions. Optionally filter by AppointmentId.
        /// </summary>
        /// <param name="appointmentId">Filter prescriptions belonging to a specific appointment</param>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? appointmentId)
        {
            var query = _db.Prescriptions.Include(p => p.Appointment).AsQueryable();

            if (appointmentId.HasValue)
                query = query.Where(p => p.AppointmentId == appointmentId.Value);

            var list = await query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.AppointmentId,
                    p.MedicineName,
                    p.Dosage,
                    p.Frequency,
                    p.Duration,
                    p.Instructions,
                    p.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Get a single prescription by ID.
        /// </summary>
        /// <param name="id">Prescription ID</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _db.Prescriptions.FindAsync(id);
            if (p == null) return NotFound(new { message = "Prescription not found" });
            return Ok(p);
        }

        /// <summary>
        /// Create a new prescription for an appointment. Returns 400 if the appointment does not exist.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrescriptionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appointment = await _db.Appointments.FindAsync(dto.AppointmentId);
            if (appointment == null)
                return BadRequest(new { message = "Invalid AppointmentId" });

            var prescription = new Prescription
            {
                AppointmentId = dto.AppointmentId,
                MedicineName = dto.MedicineName,
                Dosage = dto.Dosage,
                Frequency = dto.Frequency,
                Duration = dto.Duration,
                Instructions = dto.Instructions
            };

            _db.Prescriptions.Add(prescription);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = prescription.Id }, prescription);
        }
    }
}
