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
        }
    }
}
