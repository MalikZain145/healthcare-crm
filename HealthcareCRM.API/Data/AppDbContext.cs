using HealthcareCRM.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Hospital> Hospitals => Set<Hospital>();

        // ---- Week 6: Notifications & Reminders / Emergency Contacts - Prescriptions - SOS Alert Flow ----
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
        public DbSet<EmergencyAlert> EmergencyAlerts => Set<EmergencyAlert>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>().ToTable("Users");
            b.Entity<User>().HasIndex(u => u.Email).IsUnique();

            b.Entity<Patient>().ToTable("Patients");
            b.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany()
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            b.Entity<Doctor>().ToTable("Doctors");

            b.Entity<Appointment>().ToTable("Appointments");
            b.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Invoice>().ToTable("Invoices");
            b.Entity<Invoice>().Property(i => i.Amount).HasPrecision(18, 2);
            b.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            // Required FK — every invoice must reference a real, existing appointment.
            // Restrict delete so an appointment with invoices can't be deleted and leave an orphan.
            b.Entity<Invoice>()
                .HasOne(i => i.Appointment)
                .WithMany()
                .HasForeignKey(i => i.AppointmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Payment>().ToTable("Payments");
            b.Entity<Payment>().Property(p => p.AmountPaid).HasPrecision(18, 2);
            b.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Hospital>().ToTable("Hospitals");

            // ---- Week 6 entities ----
            b.Entity<Prescription>().ToTable("Prescriptions");
            b.Entity<Prescription>()
                .HasOne(p => p.Appointment)
                .WithMany()
                .HasForeignKey(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Notification>().ToTable("Notifications");
            b.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<EmergencyContact>().ToTable("EmergencyContacts");
            b.Entity<EmergencyContact>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<EmergencyAlert>().ToTable("EmergencyAlerts");
            b.Entity<EmergencyAlert>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
