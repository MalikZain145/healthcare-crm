# Billing & Invoices — Test Cases (Week 5)
 Healthcare CRM

---

## TEST CASE 1 — Invoice List Shows Correct Status Badges
Steps:
1. Log in as Admin or Doctor
2. Navigate to Billing & Invoices page

Expected Result: Each invoice shows correct status badge:
- Paid invoices show green "Paid" badge
- Unpaid invoices show amber "Unpaid" badge
- Overdue invoices show red "Overdue" badge
Status: Pass ✅

---

## TEST CASE 2 — Filter Invoices by Status (Paid)
Steps:
1. Navigate to Billing & Invoices
2. Select "Paid" from status filter dropdown

Expected Result: Only paid invoices are displayed in the list
Status: Pass ✅

---

## TEST CASE 3 — Filter Invoices by Status (Overdue)
Steps:
1. Navigate to Billing & Invoices
2. Select "Overdue" from status filter dropdown

Expected Result: Only unpaid invoices whose due date has passed are displayed
Status: Pass ✅

---

## TEST CASE 4 — Mark Invoice as Paid
Steps:
1. Navigate to Billing & Invoices
2. Click "Edit" on an Unpaid invoice
3. Click "Mark as Paid" button on Invoice Detail screen
4. Confirm the action

Expected Result:
- Invoice status changes to "Paid"
- Success message shown: "Invoice updated."
- "Mark as Paid" button is now disabled and shows "Already Paid"
Status: Pass ✅

---

## TEST CASE 5 — Mark as Paid Button Disabled for Paid Invoice
Steps:
1. Navigate to an invoice that is already Paid
2. Click "Edit" to open Invoice Detail screen

Expected Result: "Mark as Paid" button is disabled (grayed out) and shows "Already Paid"
Status: Pass ✅

---

## TEST CASE 6 — Loading Spinner on Invoice List
Steps:
1. Navigate to Billing & Invoices page

Expected Result: A loading spinner briefly appears while the invoice list loads
Status: Pass ✅

---

## TEST CASE 7 — Create New Invoice
Steps:
1. Navigate to Billing & Invoices
2. Click "+ New Invoice"
3. Select patient, enter amount, description, issued date
4. Click "Create invoice"

Expected Result: Invoice is created and appears in the list with correct status badge
Status: Pass ✅
