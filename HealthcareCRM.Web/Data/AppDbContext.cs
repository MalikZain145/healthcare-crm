using HealthcareCRM.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

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
            b.Entity<Invoice>()
                .HasOne(i => i.Appointment)
                .WithMany()
                .HasForeignKey(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
