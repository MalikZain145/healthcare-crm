# Appointment Module — Test Cases (Week 4)
 Healthcare CRM

---

## TEST CASE 1 — Book New Appointment (Valid Data)
Steps:
1. Log in as Doctor or Admin
2. Navigate to Appointments
3. Click "+ Book Appointment"
4. Select a patient, doctor, date and time slot
5. Enter a reason
6. Click "Book appointment"

Expected Result: Button shows "Saving...", appointment is created and appears in the list with correct date and time slot
Status: Pass ✅

---

## TEST CASE 2 — Book Appointment (Notes Left Empty)
Steps:
1. Navigate to Appointments → Book Appointment
2. Fill all required fields (patient, doctor, date, time slot, reason)
3. Leave Notes field empty
4. Click "Book appointment"

Expected Result: Appointment is created successfully — Notes is optional, no validation error shown
Status: Pass ✅

---

## TEST CASE 3 — Double Booking Conflict Check
Steps:
1. Book an appointment for Dr. Sumaira at 9:30 AM on a specific date
2. Try to book another appointment for the same doctor at the same date and time
3. Click "Book appointment"

Expected Result: Inline error message appears on the form: "This doctor already has an appointment at this date and time. Please choose a different time slot." — No duplicate appointment is created
Status: Pass ✅

---

## TEST CASE 4 — Filter Appointments by Date
Steps:
1. Navigate to Appointments list
2. Select a specific date from the date filter
3. Observe the results

Expected Result: Only appointments on the selected date are displayed in the list
Status: Pass ✅

---

## TEST CASE 5 — Filter Appointments by Doctor (Admin only)
Steps:
1. Log in as Admin
2. Navigate to Appointments list
3. Select a specific doctor from the doctor filter dropdown

Expected Result: Only appointments assigned to the selected doctor are displayed
Status: Pass ✅

---

## TEST CASE 6 — Link from Appointment to Patient Profile
Steps:
1. Navigate to Appointments list
2. Click on a patient's name in the list

Expected Result: Patient's profile/edit page opens directly — doctor can view full patient details without searching manually
Status: Pass ✅

---

## TEST CASE 7 — Time Slot Saved Correctly
Steps:
1. Navigate to Book Appointment
2. Select date and choose "09:30 AM" from time slot dropdown
3. Submit the form

Expected Result: Appointment is saved with exactly 9:30 AM — correct time appears in the appointments list
Status: Pass ✅
