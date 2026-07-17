using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly AppDbContext _db;

        public PrescriptionService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Prescription>> GetByAppointmentAsync(int appointmentId)
        {
            return await _db.Prescriptions
                .Where(p => p.AppointmentId == appointmentId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Prescription?> GetByIdAsync(int id)
        {
            return await _db.Prescriptions.FindAsync(id);
        }

        public async Task<Prescription> CreateAsync(Prescription prescription)
        {
            _db.Prescriptions.Add(prescription);
            await _db.SaveChangesAsync();
            return prescription;
        }

        public async Task<Prescription?> UpdateAsync(int id, Prescription prescription)
        {
            var p = await _db.Prescriptions.FindAsync(id);

            if (p == null)
                return null;

            p.MedicineName = prescription.MedicineName;
            p.Dosage = prescription.Dosage;
            p.Duration = prescription.Duration;
            p.Instructions = prescription.Instructions;

            await _db.SaveChangesAsync();

            return p;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _db.Prescriptions.FindAsync(id);

            if (p == null)
                return false;

            _db.Prescriptions.Remove(p);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}