using HealthcareCRM.Web.Data;
using HealthcareCRM.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _db;
        public DashboardService(AppDbContext db) => _db = db;

        public async Task<DashboardViewModel> GetStatsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Revenue/outstanding totals.
            var paidAmounts     = await _db.Invoices.Where(i => i.Status == "Paid").Select(i => i.Amount).ToListAsync();
            var unpaidAmounts   = await _db.Invoices.Where(i => i.Status == "Unpaid").Select(i => i.Amount).ToListAsync();

            return new DashboardViewModel
            {
                TotalPatients      = await _db.Patients.CountAsync(p => p.IsActive),
                TotalDoctors       = await _db.Doctors.CountAsync(d => d.IsActive),
                TotalAppointments  = await _db.Appointments.CountAsync(),
                TodaysAppointments = await _db.Appointments.CountAsync(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow),
                TotalRevenue       = paidAmounts.Sum(),
                OutstandingAmount  = unpaidAmounts.Sum(),
                UpcomingAppointments = await _db.Appointments
                    .Include(a => a.Patient).Include(a => a.Doctor)
                    .Where(a => a.AppointmentDate >= DateTime.Now && a.Status == "Scheduled")
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5).ToListAsync(),
                RecentPatients = await _db.Patients
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.Id)
                    .Take(5).ToListAsync()
            };
        }

        public async Task<DashboardViewModel> GetStatsForDoctorAsync(int doctorId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return new DashboardViewModel
            {
                TotalPatients      = await _db.Patients.CountAsync(p => p.IsActive && p.DoctorId == doctorId),
                TotalDoctors       = 0,
                TotalAppointments  = await _db.Appointments.CountAsync(a => a.DoctorId == doctorId),
                TodaysAppointments = await _db.Appointments.CountAsync(a => a.DoctorId == doctorId && a.AppointmentDate >= today && a.AppointmentDate < tomorrow),
                TotalRevenue       = 0m,
                OutstandingAmount  = 0m,
                UpcomingAppointments = await _db.Appointments
                    .Include(a => a.Patient).Include(a => a.Doctor)
                    .Where(a => a.DoctorId == doctorId && a.AppointmentDate >= DateTime.Now && a.Status == "Scheduled")
                    .OrderBy(a => a.AppointmentDate)
                    .Take(5).ToListAsync(),
                RecentPatients = await _db.Patients
                    .Where(p => p.IsActive && p.DoctorId == doctorId)
                    .OrderByDescending(p => p.Id)
                    .Take(5).ToListAsync()
            };
        }

    }
}
