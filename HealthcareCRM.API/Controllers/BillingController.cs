using HealthcareCRM.API.Data;
using HealthcareCRM.API.Models;
using HealthcareCRM.API.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillingController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BillingController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _db.Invoices.Include(i => i.Patient).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status)) query = query.Where(i => i.Status == status);

            var list = await query.OrderByDescending(i => i.IssuedDate)
                .Select(i => new
                {
                    i.Id, i.PatientId,
                    PatientName = i.Patient!.FirstName + " " + i.Patient!.LastName,
                    i.AppointmentId, i.Amount, i.Description, i.Status, i.IssuedDate
                }).ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var i = await _db.Invoices.Include(x => x.Patient).FirstOrDefaultAsync(x => x.Id == id);
            if (i == null) return NotFound(new { message = "Invoice not found" });
            return Ok(new
            {
                i.Id, i.PatientId, PatientName = i.Patient!.FullName,
                i.AppointmentId, i.Amount, i.Description, i.Status, i.IssuedDate
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _db.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { message = "Invalid PatientId" });

            var i = new Invoice
            {
                PatientId = dto.PatientId, AppointmentId = dto.AppointmentId,
                Amount = dto.Amount, Description = dto.Description,
                Status = dto.Status, IssuedDate = dto.IssuedDate
            };
            _db.Invoices.Add(i);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = i.Id }, new { i.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] InvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var i = await _db.Invoices.FindAsync(id);
            if (i == null) return NotFound(new { message = "Invoice not found" });
            i.PatientId = dto.PatientId; i.AppointmentId = dto.AppointmentId;
            i.Amount = dto.Amount; i.Description = dto.Description;
            i.Status = dto.Status; i.IssuedDate = dto.IssuedDate;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Invoice updated" });
        }

        // PATCH: api/billing/5/pay
        [HttpPatch("{id:int}/pay")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var i = await _db.Invoices.FindAsync(id);
            if (i == null) return NotFound(new { message = "Invoice not found" });
            i.Status = "Paid";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Invoice marked as paid" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var i = await _db.Invoices.FindAsync(id);
            if (i == null) return NotFound(new { message = "Invoice not found" });
            _db.Invoices.Remove(i);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Invoice deleted" });
        }
    }
}
