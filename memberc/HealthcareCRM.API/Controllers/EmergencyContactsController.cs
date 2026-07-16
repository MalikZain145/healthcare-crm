using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EmergencyContactsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Get emergency contacts. Optionally filter by UserId.
        /// </summary>
        /// <param name="userId">Filter contacts belonging to a specific user</param>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? userId)
        {
            var query = _db.EmergencyContacts.AsQueryable();

            if (userId.HasValue)
                query = query.Where(c => c.UserId == userId.Value);

            var list = await query
                .OrderBy(c => c.ContactName)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.ContactName,
                    c.PhoneNumber,
                    c.Relationship,
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Get a single emergency contact by ID.
        /// </summary>
        /// <param name="id">Emergency contact ID</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _db.EmergencyContacts.FindAsync(id);
            if (c == null) return NotFound(new { message = "Emergency contact not found" });
            return Ok(c);
        }

        /// <summary>
        /// Add a new emergency contact for a user. Returns 400 if the user does not exist.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmergencyContactDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _db.Users.AnyAsync(u => u.Id == dto.UserId))
                return BadRequest(new { message = "Invalid UserId" });

            var contact = new EmergencyContact
            {
                UserId = dto.UserId,
                ContactName = dto.ContactName,
                PhoneNumber = dto.PhoneNumber,
                Relationship = dto.Relationship
            };

            _db.EmergencyContacts.Add(contact);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
        }

        /// <summary>
        /// Delete an emergency contact by ID.
        /// </summary>
        /// <param name="id">Emergency contact ID</param>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.EmergencyContacts.FindAsync(id);
            if (c == null) return NotFound(new { message = "Emergency contact not found" });

            _db.EmergencyContacts.Remove(c);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Emergency contact deleted" });
        }
    }
}
