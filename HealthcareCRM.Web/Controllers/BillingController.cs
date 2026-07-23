using HealthcareCRM.Web.Models.ViewModels;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BillingController : Controller
    {
        private readonly IBillingService _billing;
        private readonly IPatientService _patients;

        public BillingController(IBillingService billing, IPatientService patients)
        {
            _billing = billing;
            _patients = patients;
        }

        public async Task<IActionResult> Index(string? search, string? status)
        {
            ViewBag.Search = search;
            ViewBag.Status = status;
            try
            {
                return View(await _billing.GetAllAsync(search, status));
            }
            catch (Exception)
            {
                ViewBag.LoadError = true;
                return View(new List<Invoice>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            await PopulatePatientsAsync();
            if (id == null) return View(new InvoiceViewModel());

            var i = await _billing.GetByIdAsync(id.Value);
            if (i == null) return NotFound();

            return View(new InvoiceViewModel
            {
                Id = i.Id, PatientId = i.PatientId, AppointmentId = i.AppointmentId,
                Amount = i.Amount, Description = i.Description, Status = i.Status, IssuedDate = i.IssuedDate
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(InvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulatePatientsAsync();
                return View(model);
            }

            if (model.Id == 0) await _billing.CreateAsync(model);
            else await _billing.UpdateAsync(model.Id, model);

            TempData["Success"] = model.Id == 0 ? "Invoice created." : "Invoice updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            await _billing.MarkPaidAsync(id);
            TempData["Success"] = "Invoice marked as paid.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _billing.DeleteAsync(id);
            TempData["Success"] = "Invoice deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulatePatientsAsync()
        {
            var patients = await _patients.GetAllAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }), "Id", "Name");
        }
    }
}
