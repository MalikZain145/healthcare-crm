using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Web.Controllers
{
    public class PatientsMvcController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientsMvcController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var patients = await _patientService.GetAllPatientsAsync(search);
            ViewBag.Search = search;
            return View(patients);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null) return View(new PatientViewModel());

            var patient = await _patientService.GetPatientByIdAsync(id.Value);
            if (patient == null) return NotFound();

            var model = new PatientViewModel
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(PatientViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Id == 0)
                await _patientService.CreatePatientAsync(model);
            else
                await _patientService.UpdatePatientAsync(model.Id, model);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _patientService.DeletePatientAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}