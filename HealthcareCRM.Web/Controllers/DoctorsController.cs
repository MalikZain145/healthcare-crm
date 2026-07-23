using HealthcareCRM.Web.Models.ViewModels;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctors;
        public DoctorsController(IDoctorService doctors) => _doctors = doctors;

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            try
            {
                return View(await _doctors.GetAllAsync(search));
            }
            catch (Exception)
            {
                ViewBag.LoadError = true;
                return View(new List<Doctor>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null) return View(new DoctorViewModel());

            var d = await _doctors.GetByIdAsync(id.Value);
            if (d == null) return NotFound();

            return View(new DoctorViewModel
            {
                Id = d.Id, FullName = d.FullName, Specialization = d.Specialization,
                Email = d.Email, PhoneNumber = d.PhoneNumber, IsActive = d.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(DoctorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Id == 0) await _doctors.CreateAsync(model);
            else await _doctors.UpdateAsync(model.Id, model);

            TempData["Success"] = model.Id == 0 ? "Doctor added." : "Doctor updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _doctors.DeleteAsync(id);
            TempData["Success"] = "Doctor deactivated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
