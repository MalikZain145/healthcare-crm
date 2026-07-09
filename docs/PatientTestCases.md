# Patient Module — Test Cases (Week 2)
Member B | Healthcare CRM

---

## TEST CASE 1 — Add New Patient (Valid Data)
Steps:
1. Log in as Doctor or Admin
2. Navigate to Patients
3. Click "Add patient"
4. Fill all required fields (First Name, Last Name, Email, Date of Birth, Gender)
5. Click "Add patient" to submit

Expected Result: Button shows "Saving...", patient is created, and appears in the patient list
Status: Pass

---

## TEST CASE 2 — Add Patient with Missing Required Field
Steps:
1. Navigate to Patients → Add patient
2. Leave First Name empty
3. Click "Add patient"

Expected Result: Validation error is displayed for First Name, form is not submitted
Status: Pass

---

## TEST CASE 3 — Search Patient by Name
Steps:
1. Navigate to Patients
2. Type a known patient's name in the search box
3. Press search

Expected Result: Only matching patients are displayed in the list
Status: Pass

---

## TEST CASE 4 — Empty State for No Search Results
Steps:
1. Navigate to Patients
2. Search for a name that does not exist (e.g. "zzzqq")

Expected Result: "No patients found" message is shown with guidance to try a different search term
Status: Pass

---

## TEST CASE 5 — Edit Existing Patient
Steps:
1. Navigate to Patients
2. Click "Edit" on an existing patient
3. Update the phone number
4. Click "Save changes"

Expected Result: Button shows "Saving...", patient record is updated, and new phone number reflects in the list
Status: Pass

---

## TEST CASE 6 — Delete Patient
Steps:
1. Navigate to Patients
2. Click "Delete" on an existing patient
3. Confirm the deletion

Expected Result: Patient is removed from the list
Status: Pass

---

## TEST CASE 7 — Doctor Sees Only Their Own Patients
Steps:
1. Log in as a Doctor account
2. Navigate to Patients

Expected Result: Only patients assigned to the logged-in Doctor are shown, not all patients in the system
Status: Pass
