# Member C — Week 6 Backend Deliverables (Track A + Track B)

Ye zip sirf **Member C** ke Week 6 backend endpoints par mushtamil hai, jo
`healthcare-crm/HealthcareCRM.API` project ke sath merge honi hain.

## Is zip mein kya hai

### Naye files (project mein add karni hain)
| File | Purpose |
|---|---|
| `Models/Prescription.cs` | Prescription table (Entity) |
| `Models/Notification.cs` | Notification table (Entity) |
| `Models/EmergencyContact.cs` | EmergencyContact table (Entity) |
| `Models/EmergencyAlert.cs` | SOS alert record (timestamp + status) |
| `Models/Dtos/PrescriptionDto.cs` | Create-prescription request body |
| `Models/Dtos/NotificationDto.cs` | Create-notification request body |
| `Models/Dtos/EmergencyContactDto.cs` | Create-contact request body |
| `Controllers/PrescriptionsController.cs` | `GET /api/prescriptions?appointmentId={id}`, `POST /api/prescriptions` |
| `Controllers/NotificationsController.cs` | `GET /api/notifications?userId={id}`, `POST /api/notifications`, `PATCH /api/notifications/{id}/read` |
| `Controllers/EmergencyContactsController.cs` | `GET /api/emergencycontacts`, `POST /api/emergencycontacts`, `DELETE /api/emergencycontacts/{id}` |
| `HealthcareCRM_Postman_Collection.json` | Full Postman collection, **replace your existing one** — includes all Week 6 endpoints in new folders (Prescriptions, Notifications, Emergency Contacts) plus the reminders/notify requests added to existing folders |

### Modified files (in se apni existing files ko REPLACE karein)
| File | What changed |
|---|---|
| `Controllers/AppointmentsController.cs` | Added `GET /api/appointments/reminders` — appointments within next 24 hours |
| `Controllers/EmergencyController.cs` | Added `POST /api/emergency/{id}/notify` — triggers SOS, records alert timestamp |
| `Data/AppDbContext.cs` | Added `DbSet`s + relationship config for Prescription, Notification, EmergencyContact, EmergencyAlert |

## Setup steps

1. In sab files ko apne cloned repo ke `HealthcareCRM.API/` folder mein isi
   folder structure ke saath copy/replace karein (paths already match).
2. Terminal mein API project ke andar jaake migration + DB update karein:
   ```bash
   cd HealthcareCRM.API
   dotnet ef migrations add Week6_Notifications_Prescriptions_Emergency
   dotnet ef database update
   ```
   (Agar `dotnet-ef` tool installed nahi hai: `dotnet tool install --global dotnet-ef`)
3. `dotnet build` chala kar confirm karein koi error nahi.
4. `dotnet run` se API start karein aur Swagger (`/swagger`) mein naye
   endpoints check karein.

## Endpoints Summary (Member C ka kaam)

**Track A**
- `GET /api/appointments/reminders` → appointments due in next 24 hrs
- `GET /api/prescriptions?appointmentId={id}` / `POST /api/prescriptions`
- `POST /api/notifications` / `PATCH /api/notifications/{id}/read`
- `GET /api/notifications?userId={id}` → unread notifications only

**Track B**
- `GET /api/emergencycontacts?userId={id}` / `POST /api/emergencycontacts` / `DELETE /api/emergencycontacts/{id}`
- `POST /api/emergency/{id}/notify` → marks SOS, records timestamp, returns contacts-notified count

## Deliverables checklist status (Member C)
- [x] Prescriptions table + POST & GET endpoints live
- [x] Notifications: create, get, and mark-as-read endpoints live
- [x] Reminder logic flagging appointments within 24 hours
- [x] EmergencyContacts table + GET/POST/DELETE endpoints live
- [x] POST /api/emergency/{id}/notify endpoint live
- [x] Swagger — auto-generated from XML doc comments already in the code
- [x] Postman collection updated (`HealthcareCRM_Postman_Collection.json` included)
- [ ] Friday demo — presenting the work is on you, not a file deliverable 🙂

## Notes
- Code existing project ke pattern (models, DTOs, EF Core conventions) follow
  karta hai, isliye baaki team members ke code ke sath directly compatible
  hai — koi naya package install karne ki zaroorat nahi.
- `EmergencyAlert` ek naya table hai jo SOS timestamp record karne ke liye
  banaya gaya hai (checklist ki requirement "record the alert timestamp"
  ko poora karne ke liye).
- Migration command chalana zaroori hai warna naye tables database mein
  nahi banenge.
