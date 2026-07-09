# Patient CRUD & Search — Security / Auth Test Cases
Member A| Healthcare CRM

## TEST CASE 1 — Unauthorized User Blocked from Patient CRUD

**Scope:** API — HealthcareCRM.API

**Steps:**
1. Open Swagger UI at `/swagger`
2. Do NOT authenticate (no JWT token)
3. Send GET request to `api/patients`

**Expected Result:** API returns HTTP **401 Unauthorized**. No patient data is returned. The `[Authorize]` attribute on `PatientsController` rejects the unauthenticated request before it reaches any business logic.

**Status:** Pass

---

## TEST CASE 2 — Patient Search Requires Login

**Scope:** API — HealthcareCRM.API

**Steps:**
1. Open Swagger UI at `/swagger`
2. Do NOT authenticate (no Bearer token set)
3. Send GET request to `api/patients?search=ali`

**Expected Result:** API returns HTTP **401 Unauthorized**. The search query parameter is ignored because authentication is enforced at the controller level before any filtering logic runs. Authenticating with a valid token and repeating the request should return matching results.

**Status:** Pass

---

## TEST CASE 3 — Invalid / Expired Token Rejected

**Scope:** API — HealthcareCRM.API

**Steps:**
1. Open Swagger UI and click **Authorize**
2. Enter a tampered or expired Bearer token (e.g. modify one character of a real token, or use a token issued more than 60 minutes ago)
3. Send GET request to `api/patients`

**Expected Result:** API returns HTTP **401 Unauthorized**. The JWT middleware validates the signature and expiry; an invalid or expired token fails validation and the request is rejected before reaching the controller. A fresh, correctly signed token produces a 200 response.

**Status:** Pass

---

## TEST CASE 4 — Doctor Sees Only Their Own Patients

**Scope:** Web — HealthcareCRM.Web & API

**Steps:**
1. Log in as **Doctor A** (e.g. `dr.smith@clinic.com`)
2. Navigate to `/Patients`
3. Observe the patient list
4. Log out and log in as **Doctor B** (e.g. `dr.jones@clinic.com`)
5. Navigate to `/Patients` again

**Expected Result:** Each doctor's patient list contains only the patients whose `DoctorId` matches their own record. Patients assigned to Doctor B are not visible when logged in as Doctor A, and vice versa. The scoping logic in `PatientsController.Index` (`int? scope = User.IsInRole("Admin") ? null : (await CurrentDoctorIdAsync() ?? -1)`) enforces this. An Admin sees all patients.

**Status:** Pass

---

## TEST CASE 5 — Logout Clears Session / Token

**Scope:** Web — HealthcareCRM.Web

**Steps:**
1. Log in as any user (Doctor or Admin)
2. Navigate to `/Patients` — confirm the page loads successfully
3. Click **Logout** (navigates to `/Account/Logout`)
4. Press the browser **Back** button to return to `/Patients`
5. Alternatively, copy the `/Patients` URL and open it in the same tab after logout

**Expected Result:** After logout the authentication cookie is cleared. Navigating to any `[Authorize]`-protected route (e.g. `/Patients`, `/Doctors`, `/Appointments`) redirects to `/Account/Login` instead of showing protected content. The back-button cache may briefly show a stale render, but any server-side request returns a redirect to login, not live data.

**Status:** Pass

---
