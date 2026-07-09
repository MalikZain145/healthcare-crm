using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DoctorsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false)
        {
            var query = _db.Doctors.AsQueryable();
            if (activeOnly) query = query.Where(d => d.IsActive);
            return Ok(await query.OrderBy(d => d.FullName).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var d = await _db.Doctors.FindAsync(id);
            return d == null ? NotFound(new { message = "Doctor not found" }) : Ok(d);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var d = new Doctor
            {
                FullName = dto.FullName, Specialization = dto.Specialization,
                Email = dto.Email, PhoneNumber = dto.PhoneNumber, IsActive = dto.IsActive
            };
            _db.Doctors.Add(d);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = d.Id }, d);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DoctorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var d = await _db.Doctors.FindAsync(id);
            if (d == null) return NotFound(new { message = "Doctor not found" });
            d.FullName = dto.FullName; d.Specialization = dto.Specialization;
            d.Email = dto.Email; d.PhoneNumber = dto.PhoneNumber; d.IsActive = dto.IsActive;
            await _db.SaveChangesAsync();
            return Ok(d);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _db.Doctors.FindAsync(id);
            if (d == null) return NotFound(new { message = "Doctor not found" });
            d.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Doctor deactivated" });
        }
    }
}
