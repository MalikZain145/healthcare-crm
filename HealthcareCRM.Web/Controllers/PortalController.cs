using System.Security.Claims;
using HealthcareCRM.Web.Models.ViewModels;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Web.Controllers
{
    // The patient-facing area. Anyone who self-registers (role = Patient) lands here.
    [Authorize(Roles = "Patient")]
    public class PortalController : Controller
    {
        private readonly IPatientService _patients;
        private readonly IAppointmentService _appointments;
        private readonly IBillingService _billing;

        public PortalController(IPatientService patients, IAppointmentService appointments, IBillingService billing)
        {
            _patients = patients;
            _appointments = appointments;
            _billing = billing;
        }

        private async Task<Models.Patient?> CurrentPatientAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return string.IsNullOrEmpty(email) ? null : await _patients.GetByEmailAsync(email);
        }

        public async Task<IActionResult> Index()
        {
            var patient = await CurrentPatientAsync();
            var vm = new PortalDashboardViewModel { Patient = patient };
            if (patient != null)
            {
                var appts = await _appointments.GetByPatientAsync(patient.Id);
                vm.UpcomingAppointments = appts
                    .Where(a => a.AppointmentDate >= DateTime.Now && a.Status == "Scheduled")
                    .OrderBy(a => a.AppointmentDate).Take(5).ToList();
                vm.Invoices = await _billing.GetByPatientAsync(patient.Id);
            }
            return View(vm);
        }

        public async Task<IActionResult> Appointments()
        {
            var patient = await CurrentPatientAsync();
            var appts = patient == null ? new() : await _appointments.GetByPatientAsync(patient.Id);
            return View(appts);
        }

        public async Task<IActionResult> Invoices()
        {
            var patient = await CurrentPatientAsync();
            var invoices = patient == null ? new() : await _billing.GetByPatientAsync(patient.Id);
            return View(invoices);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var p = await CurrentPatientAsync();
            if (p == null) return View(new PatientViewModel());
            return View(new PatientViewModel
            {
                Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, Email = p.Email,
                PhoneNumber = p.PhoneNumber, DateOfBirth = p.DateOfBirth, Gender = p.Gender,
                BloodType = p.BloodType, Address = p.Address
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(PatientViewModel model)
        {
            var current = await CurrentPatientAsync();
            if (current == null) return NotFound();
            // A patient may only update their own record.
            model.Id = current.Id;
            if (!ModelState.IsValid) return View(model);

            await _patients.UpdateAsync(current.Id, model);
            TempData["Success"] = "Profile updated.";
            return RedirectToAction(nameof(Profile));
        }
    }
}
