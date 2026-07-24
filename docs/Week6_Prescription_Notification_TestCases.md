# Prescription & Notification — Test Cases (Week 6)
Healthcare CRM — Track A
Tested by: Member B / QA

---

## Prescription Module

## TEST CASE 1 — Add Prescription from Appointment Details
Steps:
1. Log in as Admin or Doctor
2. Open an appointment's Details page
3. Fill in Medicine Name, Dosage, Duration, and Instructions
4. Click "Save Prescription"

Expected Result: The prescription is saved and appears immediately in the Prescription History table below the form, linked to the correct appointment.
Status: Pass ✅

---

## TEST CASE 2 — Prescription History Shows Only the Correct Appointment's Records
Steps:
1. Open Appointment Details for Patient A's appointment
2. Add a prescription
3. Open a different appointment (Patient B's) Details page

Expected Result: Patient B's appointment does not show the prescription added under Patient A's appointment — history is correctly scoped by AppointmentId.
Status: Pass ✅

---

## Notification Module

## TEST CASE 3 — Notification Bell Shows Correct Unread Count
Steps:
1. Insert an unread notification for the logged-in user directly in the database
2. Refresh any page in the app
3. Observe the bell icon in the top bar

Expected Result: A red badge appears on the bell showing the correct unread count (e.g. "1").
Status: Pass ✅

---

## TEST CASE 4 — Clicking a Notification Marks It as Read
Steps:
1. Click the bell icon to open the notification panel
2. Click on an unread notification in the list

Expected Result: The notification is marked as read in the database (IsRead = 1), the badge count decreases, and the user is navigated to the relevant screen (or the full Notifications page as a fallback).
Status: Pass ✅

---

## TEST CASE 5 — Full Notifications Page Lists Unread Notifications
Steps:
1. Navigate to the Notifications page from the bell panel's "View all" link
2. Compare the list shown against unread rows in the database for the current user

Expected Result: Every unread notification for the current user is listed with correct title, message, and date. When there are none, an empty state ("You're all caught up.") is shown instead of a blank page.
Status: Pass ✅

---

Summary: 5 / 5 test cases passed.
