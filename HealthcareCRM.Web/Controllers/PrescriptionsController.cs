using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class PrescriptionsController : Controller
    {
        private readonly IPrescriptionService _prescriptions;

        public PrescriptionsController(IPrescriptionService prescriptions)
        {
            _prescriptions = prescriptions;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(Prescription prescription)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                TempData["Success"] = errors;

                return RedirectToAction(
                    "Details",
                    "Appointments",
                    new { id = prescription.AppointmentId });
            }

            await _prescriptions.CreateAsync(prescription);

            TempData["Success"] = "Prescription saved successfully.";

            return RedirectToAction(
                "Details",
                "Appointments",
                new { id = prescription.AppointmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int appointmentId)
        {
            await _prescriptions.DeleteAsync(id);

            TempData["Success"] = "Prescription deleted successfully.";

            return RedirectToAction(
                "Details",
                "Appointments",
                new { id = appointmentId });
        }
    }
}