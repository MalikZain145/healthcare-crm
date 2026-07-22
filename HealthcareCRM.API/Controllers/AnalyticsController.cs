using HealthcareCRM.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    /// <summary>
    /// Aggregate analytics/reporting endpoints for patients, appointments, and doctors.
    /// Read-only — does not modify any data.
    /// </summary>
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AnalyticsController(AppDbContext db) => _db = db;

        /// <summary>
        /// Patient analytics: total patients, new patients registered this calendar month,
        /// and a breakdown of patients by gender.
        /// </summary>
        /// <response code="200">Analytics returned successfully</response>
        [HttpGet("patients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientAnalytics()
        {
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var totalPatients = await _db.Patients.CountAsync();

            var newPatientsThisMonth = await _db.Patients
                .CountAsync(p => p.CreatedAt >= monthStart);

            var genderBreakdown = await _db.Patients
                .GroupBy(p => p.Gender == null || p.Gender == "" ? "Unspecified" : p.Gender)
                .Select(g => new { gender = g.Key, count = g.Count() })
                .OrderByDescending(g => g.count)
                .ToListAsync();

            return Ok(new
            {
                totalPatients,
                newPatientsThisMonth,
                monthStart = DateOnly.FromDateTime(monthStart),
                genderBreakdown
            });
        }

        /// <summary>
        /// Appointment analytics: number of appointments booked on each of the last 30 days
        /// (including today). Days with zero appointments are included with a count of 0.
        /// </summary>
        /// <response code="200">Analytics returned successfully</response>
        [HttpGet("appointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAppointmentAnalytics()
        {
            var today = DateTime.Today;
            var rangeStart = today.AddDays(-29); // last 30 days inclusive of today
            var rangeEndExclusive = today.AddDays(1);

            var rows = await _db.Appointments
                .Where(a => a.AppointmentDate >= rangeStart && a.AppointmentDate < rangeEndExclusive)
                .GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new { date = g.Key, count = g.Count() })
                .ToListAsync();

            var countsByDate = rows.ToDictionary(r => r.date, r => r.count);

            // Fill in every day of the 30-day window, even days with no appointments.
            var dailyCounts = Enumerable.Range(0, 30)
                .Select(offset => rangeStart.AddDays(offset))
                .Select(date => new
                {
                    date = DateOnly.FromDateTime(date),
                    count = countsByDate.TryGetValue(date, out var c) ? c : 0
                })
                .ToList();

            return Ok(new
            {
                rangeStart = DateOnly.FromDateTime(rangeStart),
                rangeEnd = DateOnly.FromDateTime(today),
                totalAppointments = dailyCounts.Sum(d => d.count),
                dailyCounts
            });
        }

        /// <summary>
        /// Doctor analytics: number of appointments handled by each active doctor
        /// within the current calendar month.
        /// </summary>
        /// <response code="200">Analytics returned successfully</response>
        [HttpGet("doctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorAnalytics()
        {
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var monthEndExclusive = monthStart.AddMonths(1);

            var perDoctor = await _db.Doctors
                .Select(d => new
                {
                    doctorId = d.Id,
                    doctorName = d.FullName,
                    specialization = d.Specialization,
                    isActive = d.IsActive,
                    appointmentCount = d.Appointments
                        .Count(a => a.AppointmentDate >= monthStart && a.AppointmentDate < monthEndExclusive)
                })
                .OrderByDescending(d => d.appointmentCount)
                .ToListAsync();

            return Ok(new
            {
                monthStart = DateOnly.FromDateTime(monthStart),
                monthEnd = DateOnly.FromDateTime(monthEndExclusive.AddDays(-1)),
                totalAppointmentsThisMonth = perDoctor.Sum(d => d.appointmentCount),
                doctors = perDoctor
            });
        }
    }
}
