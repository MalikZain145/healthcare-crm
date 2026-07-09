namespace HealthcareCRM.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TodaysAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal OutstandingAmount { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public List<Patient> RecentPatients { get; set; } = new();
    }
}
