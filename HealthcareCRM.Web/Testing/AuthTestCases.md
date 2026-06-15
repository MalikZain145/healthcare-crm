# Auth & Onboarding Module — Test Cases
Member A | Week 1

## TEST CASE 1 — Register New User (Valid Data)
Steps:
1. Go to /Account/Register page
2. Fill First Name, Last Name, Email, Password, and Confirm Password with valid data
3. Click "Create Account"

Expected Result: Account is created, success message is shown, JWT token + user info are saved in localStorage, and the user is redirected to /PatientsMvc/Index
Status: Pass ✅

## TEST CASE 2 — Register with Already Registered Email
Steps:
1. Go to /Account/Register page
2. Enter an email address that already exists in the system
3. Fill the remaining fields correctly
4. Click "Create Account"

Expected Result: Error message "An account with this email already exists." is displayed and no token is stored
Status: Pass ✅

## TEST CASE 3 — Register with Mismatched Passwords
Steps:
1. Go to /Account/Register page
2. Fill all fields, but enter different values in Password and Confirm Password
3. Click "Create Account"

Expected Result: Validation error "Passwords must match." is shown and the form is not submitted
Status: Pass ✅

## TEST CASE 4 — Login with Valid Credentials
Steps:
1. Go to /Account/Login page
2. Enter the email and correct password of a registered user
3. Click "Sign In"

Expected Result: JWT token + user info are saved in localStorage and the user is redirected to /PatientsMvc/Index (or the returnUrl)
Status: Pass ✅

## TEST CASE 5 — Login with Invalid Credentials
Steps:
1. Go to /Account/Login page
2. Enter a registered email with an incorrect password
3. Click "Sign In"

Expected Result: Error message "Invalid email or password." is displayed, no token is stored, and the user stays on the login page
Status: Pass ✅