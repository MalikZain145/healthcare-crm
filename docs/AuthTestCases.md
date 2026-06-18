# Auth Hardening — Test Cases (Week 2)
Member B | Healthcare CRM

---

## TEST CASE 1 — Register New User (Valid Data)
Steps:
1. Go to /Account/Register
2. Fill Full Name, Email, Password, Confirm Password
3. Select a Role from dropdown
4. Click "Create account"

Expected Result: Account is created, success message shown, redirected to Login page
Status: Pass

---

## TEST CASE 2 — Register with Duplicate Email
Steps:
1. Go to /Account/Register
2. Enter an email that is already registered
3. Fill remaining fields and submit

Expected Result: Error message "An account with this email already exists" is shown, no new account created
Status: Pass

---

## TEST CASE 3 — Login with Correct Credentials
Steps:
1. Go to /Account/Login
2. Enter a registered email and correct password
3. Click "Sign in"

Expected Result: User is authenticated and redirected to the correct dashboard based on their role
Status: Pass

---

## TEST CASE 4 — Login with Wrong Password
Steps:
1. Go to /Account/Login
2. Enter a registered email with an incorrect password
3. Click "Sign in"

Expected Result: Error message "Invalid email or password" is shown, user is not logged in
Status: Pass

---

## TEST CASE 5 — Unauthorized Access to Restricted Route
Steps:
1. Log in as a user with the "Patient" role
2. Manually navigate to /Patients (a Doctor/Admin only route)

Expected Result: User is redirected to the Access Denied / Unauthorized page, Patient list is not shown
Status: Pass

---

## TEST CASE 6 — JWT Token Generation on Login (API)
Steps:
1. Open Swagger UI for HealthcareCRM.API
2. Send POST request to /api/Auth/login with valid credentials

Expected Result: Response returns status 200 with a valid JWT token, user id, full name, email, and role
Status: Pass

---

## TEST CASE 7 — Role Selector Visible on Register Form
Steps:
1. Go to /Account/Register
2. Check the form fields

Expected Result: A "Role" dropdown is visible with options Patient, Doctor, and Receptionist
Status: Pass
