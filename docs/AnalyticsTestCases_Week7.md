# Analytics — Test Cases (Week 7)
Healthcare CRM — Track A

---

## TEST CASE 1 — GET /api/analytics/patients Returns Correct Gender Breakdown
Steps:
1. Note the total patient count and gender split directly from the Patients table in the database
2. Call `GET /api/analytics/patients` (via Swagger or Postman)

Expected Result: `totalPatients` matches the database count, and `genderBreakdown` lists each gender with a count that matches the database exactly (unspecified/blank genders grouped under "Unspecified")
Status: Pass ✅

---

## TEST CASE 2 — GET /api/analytics/appointments Returns 30 Days, Zero-Filled
Steps:
1. Call `GET /api/analytics/appointments`
2. Count the entries in `dailyCounts`
3. Compare a day with no appointments booked against the database

Expected Result: Exactly 30 entries are returned (today + previous 29 days), every calendar day is present even with zero appointments, and `totalAppointments` equals the sum of all `count` values
Status: Pass ✅

---

## TEST CASE 3 — GET /api/analytics/doctors Returns Correct Per-Doctor Counts
Steps:
1. Pick a doctor with known appointments in the current month
2. Call `GET /api/analytics/doctors`
3. Find that doctor's entry in the `doctors` array

Expected Result: `appointmentCount` for that doctor matches the number of appointments in the database for the current calendar month; `totalAppointmentsThisMonth` equals the sum across all doctors
Status: Pass ✅

---

## TEST CASE 4 — Reports Page: Patients-by-Gender Chart Matches API
Steps:
1. Call `GET /api/analytics/patients` and note the `genderBreakdown` values
2. Open the Reports page and view the "Patients by gender" bar chart

Expected Result: Bar heights/labels match the API response exactly (same genders, same counts)
Status: Pass ✅

---

## TEST CASE 5 — Reports Page: Appointments Line Chart Matches API
Steps:
1. Call `GET /api/analytics/appointments` and note a few `dailyCounts` values
2. Open the Reports page and view the "Appointments — last 30 days" line chart
3. Hover/compare the same dates

Expected Result: Line chart values match the API's `dailyCounts` for the same dates
Status: Pass ✅

---

## TEST CASE 6 — Reports Page: Doctors Pie Chart Matches API
Steps:
1. Call `GET /api/analytics/doctors` and note each doctor's `appointmentCount`
2. Open the Reports page and view the "Appointments per doctor — this month" pie chart

Expected Result: Each pie segment's label and proportion matches the corresponding doctor's `appointmentCount` from the API
Status: Pass ✅

---

## TEST CASE 7 — Analytics Error Handling When API Is Unreachable
Steps:
1. Stop the `HealthcareCRM.API` project (leave `HealthcareCRM.Web` running)
2. Open the Reports page

Expected Result: Charts do not crash the page; a friendly banner ("Could not load analytics data. Please try again later.") is shown instead of a blank chart or unhandled JS error
Status: Pass ✅
