using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _db;
        public DoctorService(AppDbContext db) => _db = db;

        public async Task<List<Doctor>> GetAllAsync(string? search = null)
        {
            var query = _db.Doctors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(d =>
                    d.FullName.ToLower().Contains(t) ||
                    d.Specialization.ToLower().Contains(t) ||
                    d.Email.ToLower().Contains(t));
            }
            return await query.OrderBy(d => d.FullName).ToListAsync();
        }

        public async Task<List<Doctor>> GetActiveAsync() =>
            await _db.Doctors.Where(d => d.IsActive).OrderBy(d => d.FullName).ToListAsync();

        public async Task<Doctor?> GetByIdAsync(int id) =>
            await _db.Doctors.FirstOrDefaultAsync(d => d.Id == id);

        public async Task<Doctor?> GetByEmailAsync(string email)
        {
            var e = email.Trim().ToLower();
            return await _db.Doctors.FirstOrDefaultAsync(d => d.Email.ToLower() == e);
        }

        public async Task<Doctor> CreateAsync(DoctorViewModel m)
        {
            var d = new Doctor
            {
                FullName = m.FullName, Specialization = m.Specialization,
                Email = m.Email, PhoneNumber = m.PhoneNumber, IsActive = m.IsActive,
                CreatedAt = DateTime.UtcNow
            };
            _db.Doctors.Add(d);
            await _db.SaveChangesAsync();
            return d;
        }

        public async Task<Doctor?> UpdateAsync(int id, DoctorViewModel m)
        {
            var d = await _db.Doctors.FindAsync(id);
            if (d == null) return null;
            d.FullName = m.FullName; d.Specialization = m.Specialization;
            d.Email = m.Email; d.PhoneNumber = m.PhoneNumber; d.IsActive = m.IsActive;
            await _db.SaveChangesAsync();
            return d;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var d = await _db.Doctors.FindAsync(id);
            if (d == null) return false;
            d.IsActive = false; // soft delete (doctors may be referenced by appointments)
            await _db.SaveChangesAsync();
            return true;
        }

        public Task<int> CountAsync() => _db.Doctors.CountAsync(d => d.IsActive);
    }
}
