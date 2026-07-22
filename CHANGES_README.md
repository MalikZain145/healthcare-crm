# HealthcareCRM.API — Analytics + Push Notification Trigger (patch)

This zip contains **only the new/changed files** — drop them into your existing
`healthcare-crm-main` project at the matching paths (overwrite when prompted).

## Files in this zip

```
HealthcareCRM.API/
├── Controllers/
│   ├── AnalyticsController.cs        ← NEW
│   └── EmergencyController.cs        ← MODIFIED (added 1 new endpoint at the bottom)
└── Models/
    └── Dtos/
        └── EmergencyStatusUpdateDto.cs  ← NEW
HealthcareCRM_Postman_Collection.json ← MODIFIED (2 new items added, nothing removed)
```

**Where to copy them:**
- `HealthcareCRM.API/Controllers/AnalyticsController.cs` → `HealthcareCRM.API/Controllers/`
- `HealthcareCRM.API/Controllers/EmergencyController.cs` → **overwrite** `HealthcareCRM.API/Controllers/EmergencyController.cs`
- `HealthcareCRM.API/Models/Dtos/EmergencyStatusUpdateDto.cs` → `HealthcareCRM.API/Models/Dtos/`
- `HealthcareCRM_Postman_Collection.json` → **overwrite** the one in your project root

No changes were needed to `Program.cs`, `AppDbContext.cs`, or any existing model — all
new code reuses tables/entities that already exist in your project.

---

## 1–3. New Analytics endpoints (`AnalyticsController.cs`)

All 3 are read-only `GET` endpoints, no request body, no DB schema changes.

| Endpoint | Returns |
|---|---|
| `GET /api/analytics/patients` | `totalPatients`, `newPatientsThisMonth`, `genderBreakdown` (array of `{ gender, count }`) |
| `GET /api/analytics/appointments` | `dailyCounts` — one entry per day for the last 30 days (`{ date, count }`), zero-filled for days with no appointments |
| `GET /api/analytics/doctors` | `doctors` — one entry per doctor (`{ doctorId, doctorName, specialization, appointmentCount }`) for the **current calendar month**, sorted busiest-first |

## 4. Push Notification Trigger — Track B (`EmergencyController.cs`)

New endpoint:

```
PATCH /api/emergency/alerts/{alertId}/status
Body: { "status": "Dispatched" }   // SOS | Dispatched | Resolved
```

- Looks up the `EmergencyAlert` by `alertId` (created earlier via `POST /api/emergency/{userId}/notify`).
- If the status **actually changes**, it automatically creates a row in the existing
  `Notifications` table (`Type = "Emergency"`) for that alert's user — this is the
  "push notification" trigger, using your existing in-app notification system
  (bell icon / `GET /api/notifications?userId=`). Calling it again with the same
  status does **not** create a duplicate notification.
- Setting status to `"Resolved"` also stamps `ResolvedAt`.

## 5. Testing (manual, since I couldn't build in the sandbox — see note below)

Once you run the API (`dotnet run` inside `HealthcareCRM.API`), test in this order:

1. `GET /api/analytics/patients` → check counts against your seeded/added patients.
2. `GET /api/analytics/appointments` → check the last 30 days add up to your total appointment count in that window.
3. `GET /api/analytics/doctors` → check counts match appointments dated in the current month.
4. `POST /api/emergency/{userId}/notify` → note the returned `alert.Id` (e.g. `1`).
5. `PATCH /api/emergency/alerts/1/status` with `{ "status": "Dispatched" }` → response should say `notificationTriggered: true`.
6. `GET /api/notifications?userId={userId}` → confirm the new "Emergency Status Update" notification appears.
7. Repeat step 5 with the **same** status → response should say `notificationTriggered: false` (no duplicate).

## 6. Swagger / Postman

- **Swagger**: nothing to configure — your `Program.cs` already auto-includes XML doc
  comments for every controller, so all 4 new endpoints appear under `/swagger` automatically
  with full descriptions, once you rebuild.
- **Postman**: the updated `HealthcareCRM_Postman_Collection.json` now has a new
  **"Analytics [Track A/B - Week 7]"** folder (3 requests) and one new request added
  inside your existing **"Track B - Emergency"** folder (the `PATCH .../status` call).
  Re-import the collection (or replace the file) to get them.

---

## ⚠️ Important note on verification

I reviewed every line of new/changed C# against your existing code patterns (DbContext,
DTOs, controller style) and the JSON was validated as well-formed. However, this sandbox
can only reach `github.com`/`npmjs.com`/`pypi.org`-style package registries — **not
`nuget.org`** — so I was not able to run an actual `dotnet restore` / `dotnet build`
against your project to get a compiler-verified "zero errors" guarantee. Please run:

```bash
cd HealthcareCRM.API
dotnet build
```

after copying the files in, just to confirm on your machine (which has real NuGet
access). If anything doesn't compile, send me the exact error and I'll fix it immediately.
