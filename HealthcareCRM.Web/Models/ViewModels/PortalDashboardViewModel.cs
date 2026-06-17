using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Models.ViewModels
{
    public class PortalDashboardViewModel
    {
        public Patient? Patient { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
    }
}
