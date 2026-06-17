using System.Security.Claims;
using HealthcareCRM.Web.Models.ViewModels;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointments;
        private readonly IPatientService _patients;
        private readonly IDoctorService _doctors;

        public AppointmentsController(IAppointmentService appointments, IPatientService patients, IDoctorService doctors)
        {
            _appointments = appointments;
            _patients = patients;
            _doctors = doctors;
        }

        private async Task<int?> CurrentDoctorIdAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var doc = string.IsNullOrEmpty(email) ? null : await _doctors.GetByEmailAsync(email);
            return doc?.Id;
        }

        public async Task<IActionResult> Index(string? search, string? status)
        {
            ViewBag.Search = search;
            ViewBag.Status = status;
            int? scope = User.IsInRole("Admin") ? null : (await CurrentDoctorIdAsync() ?? -1);
            return View(await _appointments.GetAllAsync(search, status, scope));
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            await PopulateDropdownsAsync();

            if (id == null)
            {
                var vm = new AppointmentViewModel();
                if (User.IsInRole("Doctor"))
                    vm.DoctorId = await CurrentDoctorIdAsync() ?? 0; // pre-select the doctor
                return View(vm);
            }

            var a = await _appointments.GetByIdAsync(id.Value);
            if (a == null) return NotFound();

            if (User.IsInRole("Doctor") && a.DoctorId != await CurrentDoctorIdAsync())
                return Forbid();

            return View(new AppointmentViewModel
            {
                Id = a.Id, PatientId = a.PatientId, DoctorId = a.DoctorId,
                AppointmentDate = a.AppointmentDate, Reason = a.Reason,
                Status = a.Status, Notes = a.Notes
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AppointmentViewModel model)
        {
            // A doctor always books under their own name.
            if (User.IsInRole("Doctor"))
                model.DoctorId = await CurrentDoctorIdAsync() ?? model.DoctorId;

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(model);
            }

            if (model.Id == 0) await _appointments.CreateAsync(model);
            else await _appointments.UpdateAsync(model.Id, model);

            TempData["Success"] = model.Id == 0 ? "Appointment booked." : "Appointment updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointments.DeleteAsync(id);
            TempData["Success"] = "Appointment deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync()
        {
            // Doctors only see their own patients in the dropdown; admin sees all.
            int? scope = User.IsInRole("Admin") ? null : (await CurrentDoctorIdAsync() ?? -1);
            var patients = await _patients.GetAllAsync(null, scope);
            var doctors = await _doctors.GetActiveAsync();

            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }), "Id", "Name");
            ViewBag.Doctors = new SelectList(
                doctors.Select(d => new { d.Id, Name = d.FullName + " (" + d.Specialization + ")" }), "Id", "Name");
        }
    }
}
