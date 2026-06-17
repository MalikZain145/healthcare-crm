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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _db.Appointments.Include(a => a.Patient).Include(a => a.Doctor).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(a => a.Status == status);

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _db.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { message = "Invalid PatientId" });
            if (!await _db.Doctors.AnyAsync(d => d.Id == dto.DoctorId))
                return BadRequest(new { message = "Invalid DoctorId" });

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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var a = await _db.Appointments.FindAsync(id);
            if (a == null) return NotFound(new { message = "Appointment not found" });
            a.PatientId = dto.PatientId; a.DoctorId = dto.DoctorId;
            a.AppointmentDate = dto.AppointmentDate; a.Reason = dto.Reason;
            a.Status = dto.Status; a.Notes = dto.Notes;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Appointment updated" });
        }

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
