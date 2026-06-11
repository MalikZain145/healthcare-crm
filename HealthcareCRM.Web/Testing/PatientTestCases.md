# Patient Module — Test Cases
Member B | Week 1

## TEST CASE 1 — Add New Patient (Valid Data)
Steps:
1. Go to /PatientsMvc page
2. Click "+ Add New Patient" button
3. Fill all required fields (Name, Email, DOB, Gender)
4. Click Submit

Expected Result: New patient appears in the patient list
Status: Pass ✅

## TEST CASE 2 — Add Patient (Missing Required Field)
Steps:
1. Open Add New Patient form
2. Leave First Name empty
3. Click Submit

Expected Result: Validation error "First name is required" is displayed
Status: Pass ✅

## TEST CASE 3 — Search Patient by Name
Steps:
1. Go to Patient List page
2. Type a patient name in the search bar
3. Click Search button

Expected Result: Only matching patients are displayed in the list
Status: Pass ✅

## TEST CASE 4 — Edit Existing Patient
Steps:
1. Click Edit button on any patient
2. Change the phone number
3. Click Save Changes

Expected Result: Updated information is reflected in the patient list
Status: Pass ✅

## TEST CASE 5 — Delete Patient
Steps:
1. Click Delete button on any patient
2. Click OK on the confirmation popup

Expected Result: Patient is successfully removed from the list
Status: Pass ✅