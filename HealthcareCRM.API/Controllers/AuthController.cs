using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using HealthcareCRM.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = dto.Email.Trim().ToLower();
            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == email))
                return Conflict(new { message = "An account with this email already exists." });

            // Self-registration always creates a Patient account (Admin/Doctor are seeded).
            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = "Patient"
            };
            _db.Users.Add(user);

            // Ensure a matching Patient record exists.
            var existing = await _db.Patients.FirstOrDefaultAsync(p => p.Email.ToLower() == email);
            if (existing == null)
            {
                var parts = dto.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                _db.Patients.Add(new Patient
                {
                    FirstName = parts.Length > 0 ? parts[0] : dto.FullName.Trim(),
                    LastName = parts.Length > 1 ? parts[1] : string.Empty,
                    Email = dto.Email.Trim(),
                    DateOfBirth = DateTime.Today,
                    IsActive = true
                });
            }
            else if (!existing.IsActive)
            {
                existing.IsActive = true;
            }

            await _db.SaveChangesAsync();

            return Ok(new { user.Id, user.FullName, user.Email, user.Role, message = "Registration successful" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = dto.Email.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            // NOTE: For a production API, issue a JWT here. Kept simple for this project.
            return Ok(new { user.Id, user.FullName, user.Email, user.Role, message = "Login successful" });
        }
    }
}
