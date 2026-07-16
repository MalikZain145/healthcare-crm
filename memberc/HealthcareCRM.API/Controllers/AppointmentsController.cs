using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AppointmentsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Get all appointments. Optionally filter by status, date, and/or doctorId.
        /// </summary>
        /// <param name="status">Filter by appointment status (Scheduled, Completed, Cancelled)</param>
        /// <param name="date">Filter by appointment date (yyyy-MM-dd)</param>
        /// <param name="doctorId">Filter by Doctor ID</param>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] DateTime? date,
            [FromQuery] int? doctorId)
        {
            var query = _db.Appointments.Include(a => a.Patient).Include(a => a.Doctor).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            var list = await query.OrderByDescending(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.Id, a.PatientId,
                    PatientName = a.Patient!.FirstName + " " + a.Patient!.LastName,
                    a.DoctorId,
                    DoctorName = a.Doctor!.FullName,
                    a.AppointmentDate, a.Reason, a.Status, a.Notes
                }).ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get appointments that fall within the next 24 hours and are still Scheduled.
        /// Used to power reminder notifications.
        /// </summary>
        [HttpGet("reminders")]
        public async Task<IActionResult> GetReminders()
        {
            var now = DateTime.UtcNow;
            var windowEnd = now.AddHours(24);

            var list = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Status == "Scheduled"
                    && a.AppointmentDate >= now
                    && a.AppointmentDate <= windowEnd)
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.Id,
                    a.PatientId,
                    PatientName = a.Patient!.FirstName + " " + a.Patient!.LastName,
                    a.DoctorId,
                    DoctorName = a.Doctor!.FullName,
                    a.AppointmentDate,
                    a.Reason,
                    a.Status,
                    HoursUntil = Math.Round((a.AppointmentDate - now).TotalHours, 1)
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Get a single appointment by ID.
        /// </summary>
        /// <param name="id">Appointment ID</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var a = await _db.Appointments.Include(x => x.Patient).Include(x => x.Doctor)
                        .FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound(new { message = "Appointment not found" });
            return Ok(new
            {
                a.Id, a.PatientId, PatientName = a.Patient!.FullName,
                a.DoctorId, DoctorName = a.Doctor!.FullName,
                a.AppointmentDate, a.Reason, a.Status, a.Notes
            });
        }

        /// <summary>
        /// Create a new appointment. Returns 409 Conflict if the doctor already has an appointment at the same date and time.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _db.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { message = "Invalid PatientId" });
            if (!await _db.Doctors.AnyAsync(d => d.Id == dto.DoctorId))
                return BadRequest(new { message = "Invalid DoctorId" });

            // Double-booking conflict check
            bool conflict = await _db.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.AppointmentDate == dto.AppointmentDate &&
                a.Status != "Cancelled");

            if (conflict)
                return Conflict(new { message = "Doctor already has an appointment at this date and time." });

            var a = new Appointment
            {
                PatientId = dto.PatientId, DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate, Reason = dto.Reason,
                Status = dto.Status, Notes = dto.Notes
            };
            _db.Appointments.Add(a);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = a.Id }, new { a.Id });
        }

        /// <summary>
        /// Update an existing appointment. Returns 409 Conflict if the doctor already has another appointment at the same date and time.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var a = await _db.Appointments.FindAsync(id);
            if (a == null) return NotFound(new { message = "Appointment not found" });

            // Double-booking conflict check (exclude this appointment itself)
            bool conflict = await _db.Appointments.AnyAsync(x =>
                x.Id != id &&
                x.DoctorId == dto.DoctorId &&
                x.AppointmentDate == dto.AppointmentDate &&
                x.Status != "Cancelled");

            if (conflict)
                return Conflict(new { message = "Doctor already has an appointment at this date and time." });

            a.PatientId = dto.PatientId; a.DoctorId = dto.DoctorId;
            a.AppointmentDate = dto.AppointmentDate; a.Reason = dto.Reason;
            a.Status = dto.Status; a.Notes = dto.Notes;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Appointment updated" });
        }

        /// <summary>
        /// Delete an appointment by ID.
        /// </summary>
        /// <param name="id">Appointment ID</param>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _db.Appointments.FindAsync(id);
            if (a == null) return NotFound(new { message = "Appointment not found" });
            _db.Appointments.Remove(a);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Appointment deleted" });
        }
    }
}
