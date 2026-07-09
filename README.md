# HealthCare CRM

A full-stack healthcare clinic management system built with **ASP.NET Core (.NET 10)** and **SQL Server**.
The solution contains two projects:

| Project | What it is | Runs on |
|---|---|---|
| **HealthcareCRM.Web** | The complete application — Razor frontend **and** backend together (login, dashboard, patients, doctors, appointments, billing, reports). **This is the main app you run.** | one localhost (e.g. `https://localhost:7080`) |
| **HealthcareCRM.API** | An optional REST/JSON API over the **same database**, with interactive Swagger docs. Useful for testing, integrations, or a future mobile client. | a separate port (e.g. `https://localhost:7090/swagger`) |

Both projects connect to **one SQL Server database** (`HealthcareCRM`), so anything you add through the web UI is also visible through the API.

---

## Requirements
- **.NET 10 SDK**
- **SQL Server** — any of these works:
  - **SQL Server LocalDB** (installed automatically with Visual Studio) — this is the default.
  - **SQL Server Express** or a full **SQL Server** instance.

The database itself is **created and seeded automatically on first run** — you do not have to create it manually or run any SQL scripts.

---

## Quick start

### Option A — Visual Studio
1. Open `HealthcareCRM.sln`.
2. Set **HealthcareCRM.Web** as the startup project.
3. Press **F5** (or Ctrl+F5).
4. On first run the `HealthcareCRM` database is created in SQL Server, seeded with demo data, and the login page opens.

### Option B — Command line
```bash
cd HealthcareCRM.Web
dotnet run
```
Then open the URL shown in the console (e.g. `https://localhost:7080`).

### Default login
```
Email:    admin@healthcare.com
Password: Admin@123
```
You can also create a new account from the **Register** page.

---

## Database connection
The connection string lives in **`appsettings.json`** of each project. By default it uses LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HealthcareCRM;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

If you use a different SQL Server, change only the `Server=` part, for example:
- SQL Server Express:  `Server=.\\SQLEXPRESS;Database=HealthcareCRM;Trusted_Connection=True;TrustServerCertificate=True`
- Named instance:      `Server=DESKTOP-XYZ\\SQLSERVER;Database=HealthcareCRM;Trusted_Connection=True;TrustServerCertificate=True`
- SQL login:           `Server=localhost;Database=HealthcareCRM;User Id=sa;Password=YourPassword;TrustServerCertificate=True`

> Keep the connection string **the same in both projects** so the Web app and the API share one database.

The schema is created automatically via `EnsureCreated()` and seeded on first run — no manual migration step is required. (If you later want versioned migrations, you can add them with `dotnet ef migrations add InitialCreate` and call `db.Database.Migrate()` instead of `EnsureCreated()`.)

> **Tip — viewing the data:** open **SQL Server Object Explorer** in Visual Studio (View → SQL Server Object Explorer), expand `(localdb)\MSSQLLocalDB → Databases → HealthcareCRM → Tables` to see the `Patients`, `Doctors`, `Appointments`, `Invoices`, and `Users` tables.

---

## Running the REST API (optional)
```bash
cd HealthcareCRM.API
dotnet run
```
Then open **`https://localhost:7090/swagger`** to explore and test every endpoint.

Main endpoints:
- `POST /api/auth/register`, `POST /api/auth/login`
- `GET/POST/PUT/DELETE /api/patients`
- `GET/POST/PUT/DELETE /api/doctors`
- `GET/POST/PUT/DELETE /api/appointments`
- `GET/POST/PUT/DELETE /api/billing` + `PATCH /api/billing/{id}/pay`
- `GET /api/stats` (dashboard summary)

---

## Features
- **Authentication** — register / login / logout with cookie auth and PBKDF2 password hashing (no plain passwords stored). All pages require login.
- **Patients** — full CRUD, search, soft delete.
- **Doctors** — full CRUD, search, activate/deactivate.
- **Appointments** — book/edit/cancel, linked to patients & doctors, status filter (Scheduled / Completed / Cancelled).
- **Billing** — invoices per patient, mark-as-paid, paid/unpaid tracking.
- **Dashboard** — live stats (patients, doctors, today's appointments, revenue, outstanding), upcoming appointments, recent patients.
- **Reports** — appointment status breakdown and billing summary.
- Data **persists** to SQL Server and is retrieved back on every page — fully wired end to end.

---

## Tech stack
- ASP.NET Core MVC + Razor views (Web)
- ASP.NET Core Web API + Swagger (API)
- Entity Framework Core 8 (SQL Server provider)
- Bootstrap 5 + a custom clinical theme (`wwwroot/css/site.css`)
- Cookie authentication + PBKDF2 hashing (no third-party auth packages)

> **NuGet note:** EF Core packages are pinned to **8.0.0** (stable LTS, restores cleanly on .NET 10). To match .NET 10 exactly, change every `Microsoft.EntityFrameworkCore.*` version in both `.csproj` files to `10.0.0`.

---

## Project structure
```
HealthcareCRM.sln
├── HealthcareCRM.Web/          ← main app (frontend + backend, one localhost)
│   ├── Controllers/            ← Account, Dashboard, Patients, Doctors, Appointments, Billing, Reports
│   ├── Models/                 ← entities + ViewModels
│   ├── Data/                   ← AppDbContext + DbSeeder
│   ├── Services/               ← business logic + PasswordHasher
│   ├── Views/                  ← Razor views (the UI)
│   └── wwwroot/                ← css, js, bootstrap/jquery libs
└── HealthcareCRM.API/          ← REST API over the same database (+ Swagger)
    ├── Controllers/            ← Patients, Doctors, Appointments, Billing, Auth, Stats
    ├── Models/ + Models/Dtos/
    ├── Data/                   ← AppDbContext + DbSeeder
    └── Services/               ← PasswordHasher
```
