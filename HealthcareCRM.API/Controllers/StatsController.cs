using HealthcareCRM.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StatsController(AppDbContext db) => _db = db;

        // GET: api/stats  -> dashboard summary
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Revenue/outstanding totals.
            var paid   = await _db.Invoices.Where(i => i.Status == "Paid").Select(i => i.Amount).ToListAsync();
            var unpaid = await _db.Invoices.Where(i => i.Status == "Unpaid").Select(i => i.Amount).ToListAsync();

            return Ok(new
            {
                totalPatients      = await _db.Patients.CountAsync(p => p.IsActive),
                totalDoctors       = await _db.Doctors.CountAsync(d => d.IsActive),
                totalAppointments  = await _db.Appointments.CountAsync(),
                todaysAppointments = await _db.Appointments.CountAsync(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow),
                totalRevenue       = paid.Sum(),
                outstandingAmount  = unpaid.Sum()
            });
        }
    }
}
