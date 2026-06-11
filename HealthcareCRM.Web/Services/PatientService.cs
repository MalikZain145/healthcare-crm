using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        public PatientService(AppDbContext context) { _context = context; }

        public async Task<List<Patient>> GetAllPatientsAsync(string? searchTerm = null)
        {
            var query = _context.Patients.Where(p => p.IsActive);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    p.Email.ToLower().Contains(term));
            }
            return await query.OrderBy(p => p.LastName).ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id) =>
            await _context.Patients.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        public async Task<Patient> CreatePatientAsync(PatientViewModel model)
        {
            var patient = new Patient
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient?> UpdatePatientAsync(int id, PatientViewModel model)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null || !patient.IsActive) return null;
            patient.FirstName = model.FirstName; patient.LastName = model.LastName;
            patient.Email = model.Email; patient.PhoneNumber = model.PhoneNumber;
            patient.DateOfBirth = model.DateOfBirth; patient.Gender = model.Gender;
            patient.Address = model.Address;
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;
            patient.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}