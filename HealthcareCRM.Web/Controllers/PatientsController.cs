using System.Security.Claims;
using HealthcareCRM.Web.Models.ViewModels;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class PatientsController : Controller
    {
        private readonly IPatientService _patients;
        private readonly IDoctorService _doctors;

        public PatientsController(IPatientService patients, IDoctorService doctors)
        {
            _patients = patients;
            _doctors = doctors;
        }

        // The logged-in doctor's id (null if the user isn't a doctor / has no doctor record).
        private async Task<int?> CurrentDoctorIdAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var doc = string.IsNullOrEmpty(email) ? null : await _doctors.GetByEmailAsync(email);
            return doc?.Id;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            // Admin sees all; a doctor sees only their own patients.
            int? scope = User.IsInRole("Admin") ? null : (await CurrentDoctorIdAsync() ?? -1);
            return View(await _patients.GetAllAsync(search, scope));
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null) return View(new PatientViewModel());

            var p = await _patients.GetByIdAsync(id.Value);
            if (p == null) return NotFound();

            // A doctor may only edit their own patients.
            if (User.IsInRole("Doctor") && p.DoctorId != await CurrentDoctorIdAsync())
                return Forbid();

            return View(new PatientViewModel
            {
                Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, Email = p.Email,
                PhoneNumber = p.PhoneNumber, DateOfBirth = p.DateOfBirth, Gender = p.Gender,
                BloodType = p.BloodType, Address = p.Address
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(PatientViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Id == 0)
            {
                // Doctor-created patients are owned by that doctor; admin-created stay unassigned.
                int? owner = User.IsInRole("Doctor") ? await CurrentDoctorIdAsync() : null;
                await _patients.CreateAsync(model, owner);
            }
            else
            {
                await _patients.UpdateAsync(model.Id, model);
            }

            TempData["Success"] = model.Id == 0 ? "Patient added." : "Patient updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _patients.DeleteAsync(id);
            TempData["Success"] = "Patient removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
