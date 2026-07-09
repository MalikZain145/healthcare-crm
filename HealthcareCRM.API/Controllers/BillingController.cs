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

        /// <summary>
        /// Get all invoices. Optional filter by status: Paid, Unpaid, or Overdue
        /// (Overdue = Unpaid invoices whose DueDate has already passed).
        /// </summary>
        /// <param name="status">Paid | Unpaid | Overdue</param>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _db.Invoices.Include(i => i.Patient).Include(i => i.Appointment).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var now = DateTime.UtcNow;
                query = status.Trim().ToLower() switch
                {
                    "paid" => query.Where(i => i.Status == "Paid"),
                    "unpaid" => query.Where(i => i.Status == "Unpaid"),
                    "overdue" => query.Where(i => i.Status == "Unpaid" && i.DueDate < now),
                    _ => query.Where(i => i.Status == status)
                };
            }

            var list = await query.OrderByDescending(i => i.IssuedDate)
                .Select(i => new
                {
                    i.Id,
                    i.PatientId,
                    PatientName = i.Patient!.FirstName + " " + i.Patient!.LastName,
                    i.AppointmentId,
                    i.Amount,
                    i.Description,
                    i.Status,
                    i.IssuedDate,
                    i.DueDate,
                    i.PaidAt,
                    IsOverdue = i.Status == "Unpaid" && i.DueDate < DateTime.UtcNow
                }).ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get a single invoice by ID, including its payment history.
        /// </summary>
        /// <param name="id">Invoice ID</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var i = await _db.Invoices
                .Include(x => x.Patient)
                .Include(x => x.Appointment)
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (i == null) return NotFound(new { message = "Invoice not found" });

            return Ok(new
            {
                i.Id,
                i.PatientId,
                PatientName = i.Patient!.FullName,
                i.AppointmentId,
                i.Amount,
                i.Description,
                i.Status,
                i.IssuedDate,
                i.DueDate,
                i.PaidAt,
                Payments = i.Payments.Select(p => new { p.Id, p.AmountPaid, p.PaymentMethod, p.Status, p.PaidAt })
            });
        }

        /// <summary>
        /// Create an invoice directly (manual PatientId + AppointmentId). Prefer POST /api/billing/generate
        /// when generating an invoice from an appointment, since it derives PatientId automatically.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!await _db.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { message = "Invalid PatientId" });
            if (!await _db.Appointments.AnyAsync(a => a.Id == dto.AppointmentId))
                return BadRequest(new { message = "Invalid AppointmentId — invoice must be linked to a real appointment" });

            var i = new Invoice
            {
                PatientId = dto.PatientId,
                AppointmentId = dto.AppointmentId,
                Amount = dto.Amount,
                Description = dto.Description,
                Status = dto.Status,
                IssuedDate = dto.IssuedDate,
                DueDate = dto.DueDate
            };
            _db.Invoices.Add(i);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = i.Id }, new { i.Id });
        }

        /// <summary>
        /// Generate a new invoice from an existing appointment. PatientId is derived automatically
        /// from the appointment, so the invoice can never be orphaned (no AppointmentId = 400 Bad Request,
        /// unknown AppointmentId = 404 Not Found).
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] GenerateInvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == dto.AppointmentId);
            if (appointment == null)
                return NotFound(new { message = $"Appointment with ID {dto.AppointmentId} not found" });

            var invoice = new Invoice
            {
                PatientId = appointment.PatientId,
                AppointmentId = appointment.Id,
                Amount = dto.Amount,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? $"Invoice for appointment #{appointment.Id}" : dto.Description,
                Status = "Unpaid",
                IssuedDate = DateTime.UtcNow,
                DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(7)
            };

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = invoice.Id }, new
            {
                invoice.Id,
                invoice.PatientId,
                invoice.AppointmentId,
                invoice.Amount,
                invoice.Description,
                invoice.Status,
                invoice.IssuedDate,
                invoice.DueDate
            });
        }

        /// <summary>
        /// Update an existing invoice's details.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] InvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var i = await _db.Invoices.FindAsync(id);
            if (i == null) return NotFound(new { message = "Invoice not found" });
            if (!await _db.Appointments.AnyAsync(a => a.Id == dto.AppointmentId))
                return BadRequest(new { message = "Invalid AppointmentId — invoice must be linked to a real appointment" });

            i.PatientId = dto.PatientId;
            i.AppointmentId = dto.AppointmentId;
            i.Amount = dto.Amount;
            i.Description = dto.Description;
            i.Status = dto.Status;
            i.IssuedDate = dto.IssuedDate;
            i.DueDate = dto.DueDate;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Invoice updated" });
        }

        /// <summary>
        /// Mark an invoice as Paid. Sets the PaidAt timestamp and creates (or updates) the
        /// associated Payment record for this invoice.
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <param name="dto">Optional payment details (amount paid, payment method)</param>
        [HttpPatch("{id:int}/pay")]
        public async Task<IActionResult> MarkPaid(int id, [FromBody] MarkPaidDto? dto)
        {
            var invoice = await _db.Invoices.Include(i => i.Payments).FirstOrDefaultAsync(i => i.Id == id);
            if (invoice == null) return NotFound(new { message = "Invoice not found" });

            if (invoice.Status == "Paid")
                return BadRequest(new { message = "Invoice is already marked as paid" });

            var now = DateTime.UtcNow;
            var amountPaid = dto?.AmountPaid ?? invoice.Amount;
            var method = string.IsNullOrWhiteSpace(dto?.PaymentMethod) ? "Cash" : dto!.PaymentMethod;

            invoice.Status = "Paid";
            invoice.PaidAt = now;

            // Create or update the Payment record tied to this invoice.
            var payment = invoice.Payments.FirstOrDefault();
            if (payment == null)
            {
                payment = new Payment
                {
                    InvoiceId = invoice.Id,
                    AmountPaid = amountPaid,
                    PaymentMethod = method,
                    Status = "Completed",
                    PaidAt = now
                };
                _db.Payments.Add(payment);
            }
            else
            {
                payment.AmountPaid = amountPaid;
                payment.PaymentMethod = method;
                payment.Status = "Completed";
                payment.PaidAt = now;
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Invoice marked as paid",
                invoice.Id,
                invoice.Status,
                invoice.PaidAt,
                Payment = new { payment.Id, payment.AmountPaid, payment.PaymentMethod, payment.Status, payment.PaidAt }
            });
        }

        /// <summary>
        /// Delete an invoice by ID.
        /// </summary>
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
