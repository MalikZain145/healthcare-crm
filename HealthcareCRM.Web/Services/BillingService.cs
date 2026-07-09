using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class BillingService : IBillingService
    {
        private readonly AppDbContext _db;
        public BillingService(AppDbContext db) => _db = db;

        public async Task<List<Invoice>> GetAllAsync(string? search = null, string? status = null)
        {
            var query = _db.Invoices.Include(i => i.Patient).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(i => i.Status == status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var t = search.Trim().ToLower();
                query = query.Where(i =>
                    (i.Patient!.FirstName + " " + i.Patient!.LastName).ToLower().Contains(t) ||
                    i.Description.ToLower().Contains(t));
            }
            return await query.OrderByDescending(i => i.IssuedDate).ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id) =>
            await _db.Invoices.Include(i => i.Patient).FirstOrDefaultAsync(i => i.Id == id);

        public async Task<List<Invoice>> GetByPatientAsync(int patientId) =>
            await _db.Invoices
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.IssuedDate)
                .ToListAsync();

        public async Task<Invoice> CreateAsync(InvoiceViewModel m)
        {
            var i = new Invoice
            {
                PatientId = m.PatientId, AppointmentId = m.AppointmentId,
                Amount = m.Amount, Description = m.Description, Status = m.Status,
                IssuedDate = m.IssuedDate, CreatedAt = DateTime.UtcNow
            };
            _db.Invoices.Add(i);
            await _db.SaveChangesAsync();
            return i;
        }

        public async Task<Invoice?> UpdateAsync(int id, InvoiceViewModel m)
        {
            var i = await _db.Invoices.FindAsync(id);
            if (i == null) return null;
            i.PatientId = m.PatientId; i.AppointmentId = m.AppointmentId;
            i.Amount = m.Amount; i.Description = m.Description;
            i.Status = m.Status; i.IssuedDate = m.IssuedDate;
            await _db.SaveChangesAsync();
            return i;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var i = await _db.Invoices.FindAsync(id);
            if (i == null) return false;
            _db.Invoices.Remove(i);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkPaidAsync(int id)
        {
            var i = await _db.Invoices.FindAsync(id);
            if (i == null) return false;
            i.Status = "Paid";
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
