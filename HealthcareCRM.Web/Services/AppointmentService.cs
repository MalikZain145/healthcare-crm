using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;
        public AppointmentService(AppDbContext db) => _db = db;

        public async Task<List<Appointment>> GetAllAsync(string? search = null, string? status = null, int? doctorId = null)
        {
            var query = _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.Patient!.FirstName + " " + a.Patient!.LastName).ToLower().Contains(t) ||
                    a.Doctor!.FullName.ToLower().Contains(t) ||
                    a.Reason.ToLower().Contains(t));
            }
            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id) =>
            await _db.Appointments
                .Include(a => a.Patient).Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<Appointment>> GetByPatientAsync(int patientId) =>
            await _db.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

        public async Task<Appointment> CreateAsync(AppointmentViewModel m)
        {
            var a = new Appointment
            {
                PatientId = m.PatientId, DoctorId = m.DoctorId,
                AppointmentDate = m.AppointmentDate, Reason = m.Reason,
                Status = m.Status, Notes = m.Notes, CreatedAt = DateTime.UtcNow
            };
            _db.Appointments.Add(a);
            await _db.SaveChangesAsync();
            return a;
        }

        public async Task<Appointment?> UpdateAsync(int id, AppointmentViewModel m)
        {
            var a = await _db.Appointments.FindAsync(id);
            if (a == null) return null;
            a.PatientId = m.PatientId; a.DoctorId = m.DoctorId;
            a.AppointmentDate = m.AppointmentDate; a.Reason = m.Reason;
            a.Status = m.Status; a.Notes = m.Notes;
            await _db.SaveChangesAsync();
            return a;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var a = await _db.Appointments.FindAsync(id);
            if (a == null) return false;
            _db.Appointments.Remove(a);
            await _db.SaveChangesAsync();
            return true;
        }

        public Task<int> CountAsync() => _db.Appointments.CountAsync();
    }
}
