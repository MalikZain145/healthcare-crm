using HealthcareCRM.Web.Models;

namespace HealthcareCRM.Web.Models.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; } = new Appointment();

        public Prescription NewPrescription { get; set; } = new Prescription();

        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}