using HealthcareCRM.Web.Models.ViewModels;
using HealthcareCRM.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareCRM.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IDashboardService _dashboard;
        private readonly IAppointmentService _appointments;
        private readonly IBillingService _billing;

        public ReportsController(IDashboardService dashboard, IAppointmentService appointments, IBillingService billing)
        {
            _dashboard = dashboard;
            _appointments = appointments;
            _billing = billing;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = await _dashboard.GetStatsAsync();

                var allAppts = await _appointments.GetAllAsync();
                ViewBag.Scheduled = allAppts.Count(a => a.Status == "Scheduled");
                ViewBag.Completed = allAppts.Count(a => a.Status == "Completed");
                ViewBag.Cancelled = allAppts.Count(a => a.Status == "Cancelled");

                var allInvoices = await _billing.GetAllAsync();
                ViewBag.PaidCount = allInvoices.Count(i => i.Status == "Paid");
                ViewBag.UnpaidCount = allInvoices.Count(i => i.Status == "Unpaid");

                return View(stats);
            }
            catch (Exception)
            {
                ViewBag.LoadError = true;
                return View(new DashboardViewModel());
            }
        }
    }
}
