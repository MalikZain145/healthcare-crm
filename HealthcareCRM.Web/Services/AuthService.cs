using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        public AuthService(AppDbContext db) => _db = db;

        public async Task<(bool Success, string? Error)> RegisterAsync(RegisterViewModel m)
        {
            var email = m.Email.Trim().ToLower();
            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == email))
                return (false, "An account with this email already exists.");

            // Self-registration ALWAYS creates a Patient account (Admin/Doctor are seeded, never self-register).
            _db.Users.Add(new User
            {
                FullName = m.FullName.Trim(),
                Email = m.Email.Trim(),
                PasswordHash = PasswordHasher.Hash(m.Password),
                Role = "Patient",
                CreatedAt = DateTime.UtcNow
            });

            // Make sure a matching Patient record exists so the user shows up in the patient module.
            var existing = await _db.Patients.FirstOrDefaultAsync(p => p.Email.ToLower() == email);
            if (existing == null)
            {
                var parts = m.FullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                _db.Patients.Add(new Patient
                {
                    FirstName = parts.Length > 0 ? parts[0] : m.FullName.Trim(),
                    LastName = parts.Length > 1 ? parts[1] : string.Empty,
                    Email = m.Email.Trim(),
                    DateOfBirth = DateTime.Today,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else if (!existing.IsActive)
            {
                existing.IsActive = true; // reactivate a doctor-created record if needed
            }

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<User?> ValidateAsync(string email, string password)
        {
            var e = email.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == e);
            if (user == null) return null;
            return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
        }
    }
}
