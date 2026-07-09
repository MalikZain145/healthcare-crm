using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _db;
        public PatientService(AppDbContext db) => _db = db;

        public async Task<List<Patient>> GetAllAsync(string? search = null, int? doctorId = null)
        {
            var query = _db.Patients.Where(p => p.IsActive);

            if (doctorId.HasValue)
                query = query.Where(p => p.DoctorId == doctorId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(t) ||
                    p.LastName.ToLower().Contains(t) ||
                    p.Email.ToLower().Contains(t) ||
                    p.PhoneNumber.Contains(t) ||
                    (p.FirstName.ToLower() + " " + p.LastName.ToLower()).Contains(t));
            }
            return await query.OrderBy(p => p.FirstName).ThenBy(p => p.LastName).ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id) =>
            await _db.Patients.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        public async Task<Patient?> GetByEmailAsync(string email)
        {
            var e = email.Trim().ToLower();
            return await _db.Patients.FirstOrDefaultAsync(p => p.Email.ToLower() == e && p.IsActive);
        }

        public async Task<Patient> CreateAsync(PatientViewModel m, int? doctorId = null)
        {
            var p = new Patient
            {
                FirstName = m.FirstName, LastName = m.LastName, Email = m.Email,
                PhoneNumber = m.PhoneNumber, DateOfBirth = m.DateOfBirth, Gender = m.Gender,
                Address = m.Address, BloodType = m.BloodType, DoctorId = doctorId,
                CreatedAt = DateTime.UtcNow, IsActive = true
            };
            _db.Patients.Add(p);
            await _db.SaveChangesAsync();
            return p;
        }

        public async Task<Patient?> UpdateAsync(int id, PatientViewModel m)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null || !p.IsActive) return null;
            p.FirstName = m.FirstName; p.LastName = m.LastName; p.Email = m.Email;
            p.PhoneNumber = m.PhoneNumber; p.DateOfBirth = m.DateOfBirth; p.Gender = m.Gender;
            p.Address = m.Address; p.BloodType = m.BloodType;
            await _db.SaveChangesAsync();
            return p;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return false;
            p.IsActive = false; // soft delete
            await _db.SaveChangesAsync();
            return true;
        }

        public Task<int> CountAsync(int? doctorId = null) =>
            doctorId.HasValue
                ? _db.Patients.CountAsync(p => p.IsActive && p.DoctorId == doctorId.Value)
                : _db.Patients.CountAsync(p => p.IsActive);
    }
}
