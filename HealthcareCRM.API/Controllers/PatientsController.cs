using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PatientsController(AppDbContext db) => _db = db;

        // GET: api/patients?search=ali
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var query = _db.Patients.Where(p => p.IsActive);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(p => (p.FirstName + " " + p.LastName).ToLower().Contains(t)
                                      || p.Email.ToLower().Contains(t));
            }
            var list = await query.OrderBy(p => p.FirstName).ToListAsync();
            return Ok(list);
        }

        // GET: api/patients/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
            return p == null ? NotFound(new { message = "Patient not found" }) : Ok(p);
        }

        // POST: api/patients
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var p = new Patient
            {
                FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email,
                PhoneNumber = dto.PhoneNumber, DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender, BloodType = dto.BloodType, DoctorId = dto.DoctorId, Address = dto.Address
            };
            _db.Patients.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = p.Id }, p);
        }

        // PUT: api/patients/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var p = await _db.Patients.FindAsync(id);
            if (p == null || !p.IsActive) return NotFound(new { message = "Patient not found" });
            p.FirstName = dto.FirstName; p.LastName = dto.LastName; p.Email = dto.Email;
            p.PhoneNumber = dto.PhoneNumber; p.DateOfBirth = dto.DateOfBirth;
            p.Gender = dto.Gender; p.BloodType = dto.BloodType; p.DoctorId = dto.DoctorId; p.Address = dto.Address;
            await _db.SaveChangesAsync();
            return Ok(p);
        }

        // DELETE: api/patients/5  (soft delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound(new { message = "Patient not found" });
            p.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Patient removed" });
        }
    }
}
