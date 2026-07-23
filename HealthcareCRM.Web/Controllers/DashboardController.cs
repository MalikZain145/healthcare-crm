using System.Diagnostics;
using System.Security.Claims;
using HealthcareCRM.Web.Models;
using HealthcareCRM.Web.Models.ViewModels;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize(Roles = "Admin,Doctor")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboard;
        private readonly IDoctorService _doctors;

        public DashboardController(IDashboardService dashboard, IDoctorService doctors)
        {
            _dashboard = dashboard;
            _doctors = doctors;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (User.IsInRole("Admin"))
                    return View(await _dashboard.GetStatsAsync());

                // Doctor: scoped to their own patients & appointments.
                var email = User.FindFirstValue(ClaimTypes.Email);
                var doc = string.IsNullOrEmpty(email) ? null : await _doctors.GetByEmailAsync(email);
                var model = doc == null
                    ? new DashboardViewModel()
                    : await _dashboard.GetStatsForDoctorAsync(doc.Id);
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.LoadError = true;
                return View(new DashboardViewModel());
            }
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
