using HealthcareCRM.API.Models;
using HealthcareCRM.API.Services;
using Microsoft.EntityFrameworkCore;

namespace HealthcareCRM.API.Data
{
    // Creates the database (if missing) and seeds demo data once.
    // Admin and Doctor accounts are HARDCODED here (they never sign up).
    // Anyone who registers through the app becomes a Patient.
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            db.Database.EnsureCreated();

            // ---- Doctors (hardcoded) ----
            if (!db.Doctors.Any())
            {
                db.Doctors.AddRange(
                    new Doctor { FullName = "Dr. Sara Ahmed",   Specialization = "Cardiology",  Email = "sara@healthcare.com",   PhoneNumber = "0300-1112233" },
                    new Doctor { FullName = "Dr. Bilal Khan",   Specialization = "Dermatology", Email = "bilal@healthcare.com",  PhoneNumber = "0301-2223344" },
                    new Doctor { FullName = "Dr. Ayesha Malik", Specialization = "Pediatrics",  Email = "ayesha@healthcare.com", PhoneNumber = "0302-3334455" },
                    new Doctor { FullName = "Dr. Usman Tariq",  Specialization = "Orthopedics", Email = "usman@healthcare.com",  PhoneNumber = "0303-4445566" }
                );
                db.SaveChanges();
            }

            // ---- User accounts: Admin + a login for every Doctor (hardcoded, no signup) ----
            if (!db.Users.Any())
            {
                // Admin
                db.Users.Add(new User
                {
                    FullName = "System Admin",
                    Email = "admin@healthcare.com",
                    PasswordHash = PasswordHasher.Hash("Admin@123"),
                    Role = "Admin"
                });

                // One login per doctor (role = Doctor), matched to the Doctor record by email
                foreach (var doc in db.Doctors.ToList())
                {
                    db.Users.Add(new User
                    {
                        FullName = doc.FullName,
                        Email = doc.Email,
                        PasswordHash = PasswordHasher.Hash("Doctor@123"),
                        Role = "Doctor"
                    });
                }
                db.SaveChanges();
            }

            // ---- Patients (each assigned to a doctor) ----
            if (!db.Patients.Any())
            {
                var docs = db.Doctors.OrderBy(d => d.Id).ToList();
                db.Patients.AddRange(
                    new Patient { FirstName = "Ali",    LastName = "Raza",   Email = "ali.raza@example.com",    PhoneNumber = "0311-1234567", DateOfBirth = new DateTime(1990, 5, 14), Gender = "Male",   Address = "Rawalpindi", BloodType = "O+",  DoctorId = docs[0].Id },
                    new Patient { FirstName = "Fatima", LastName = "Noor",   Email = "fatima.noor@example.com", PhoneNumber = "0312-2345678", DateOfBirth = new DateTime(1985, 9, 2),  Gender = "Female", Address = "Islamabad",  BloodType = "A+",  DoctorId = docs[1].Id },
                    new Patient { FirstName = "Hamza",  LastName = "Sheikh", Email = "hamza.sheikh@example.com",PhoneNumber = "0313-3456789", DateOfBirth = new DateTime(2001, 1, 23), Gender = "Male",   Address = "Lahore",     BloodType = "B+",  DoctorId = docs[2].Id },
                    new Patient { FirstName = "Zoya",   LastName = "Iqbal",  Email = "zoya.iqbal@example.com",  PhoneNumber = "0314-4567890", DateOfBirth = new DateTime(1995, 12, 9), Gender = "Female", Address = "Karachi",    BloodType = "AB-", DoctorId = docs[0].Id }
                );
                db.SaveChanges();

                // A demo Patient login (role = Patient), linked by email to Ali Raza
                if (!db.Users.Any(u => u.Email == "ali.raza@example.com"))
                {
                    db.Users.Add(new User
                    {
                        FullName = "Ali Raza",
                        Email = "ali.raza@example.com",
                        PasswordHash = PasswordHasher.Hash("Patient@123"),
                        Role = "Patient"
                    });
                    db.SaveChanges();
                }
            }

            // ---- Appointments + Invoices ----
            if (!db.Appointments.Any())
            {
                var patients = db.Patients.OrderBy(p => p.Id).ToList();
                var doctors  = db.Doctors.OrderBy(d => d.Id).ToList();

                var appts = new List<Appointment>
                {
                    new Appointment { PatientId = patients[0].Id, DoctorId = doctors[0].Id, AppointmentDate = DateTime.Today.AddDays(1).AddHours(10), Reason = "Routine heart checkup", Status = "Scheduled" },
                    new Appointment { PatientId = patients[1].Id, DoctorId = doctors[1].Id, AppointmentDate = DateTime.Today.AddDays(2).AddHours(12), Reason = "Skin allergy",          Status = "Scheduled" },
                    new Appointment { PatientId = patients[2].Id, DoctorId = doctors[2].Id, AppointmentDate = DateTime.Today.AddDays(-3).AddHours(9), Reason = "Fever & flu",          Status = "Completed" },
                    new Appointment { PatientId = patients[3].Id, DoctorId = doctors[0].Id, AppointmentDate = DateTime.Today.AddHours(15),            Reason = "Knee pain",            Status = "Scheduled" }
                };
                db.Appointments.AddRange(appts);
                db.SaveChanges();

                db.Invoices.AddRange(
                    new Invoice { PatientId = patients[2].Id, AppointmentId = appts[2].Id, Amount = 3500m, Description = "Consultation + medicines", Status = "Paid",   IssuedDate = DateTime.Today.AddDays(-3), DueDate = DateTime.Today.AddDays(4),  PaidAt = DateTime.Today.AddDays(-2) },
                    new Invoice { PatientId = patients[0].Id, AppointmentId = appts[0].Id, Amount = 2000m, Description = "Consultation fee",          Status = "Unpaid", IssuedDate = DateTime.Today,             DueDate = DateTime.Today.AddDays(7) },
                    new Invoice { PatientId = patients[1].Id, AppointmentId = appts[1].Id, Amount = 1500m, Description = "Lab tests",                  Status = "Unpaid", IssuedDate = DateTime.Today.AddDays(-10), DueDate = DateTime.Today.AddDays(-3) } // overdue, for testing the Overdue filter
                );
                db.SaveChanges();
            }

            // ---- Hospitals (sample data for the nearby-hospital distance API) ----
            if (!db.Hospitals.Any())
            {
                db.Hospitals.AddRange(
                    new Hospital { Name = "Lahore General Hospital",      Address = "Ferozepur Road, Lahore",         City = "Lahore",     PhoneNumber = "042-99230113", Latitude = 31.5204, Longitude = 74.3587 },
                    new Hospital { Name = "Services Hospital Lahore",     Address = "Jail Road, Lahore",              City = "Lahore",     PhoneNumber = "042-99203402", Latitude = 31.5546, Longitude = 74.3221 },
                    new Hospital { Name = "Shaukat Khanum Memorial",      Address = "Johar Town, Lahore",             City = "Lahore",     PhoneNumber = "042-35945100", Latitude = 31.4697, Longitude = 74.2728 },
                    new Hospital { Name = "PIMS Hospital",                Address = "G-8/3, Islamabad",               City = "Islamabad",  PhoneNumber = "051-9261170",  Latitude = 33.7294, Longitude = 73.0931 },
                    new Hospital { Name = "Shifa International Hospital", Address = "Sector H-8/4, Islamabad",        City = "Islamabad",  PhoneNumber = "051-8464646",  Latitude = 33.6789, Longitude = 73.0426 },
                    new Hospital { Name = "Aga Khan University Hospital", Address = "Stadium Road, Karachi",          City = "Karachi",    PhoneNumber = "021-34930051", Latitude = 24.8929, Longitude = 67.0817 },
                    new Hospital { Name = "Jinnah Postgraduate Medical Centre", Address = "Rafiqui Shaheed Road, Karachi", City = "Karachi", PhoneNumber = "021-99201300", Latitude = 24.8546, Longitude = 67.0298 },
                    new Hospital { Name = "Nishtar Hospital",             Address = "Nishtar Road, Multan",           City = "Multan",     PhoneNumber = "061-9201133",  Latitude = 30.1798, Longitude = 71.4498 }
                );
                db.SaveChanges();
            }
        }
    }
}
