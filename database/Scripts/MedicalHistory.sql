
-- MedicalHistory table -- linked to Patients

USE HealthcareCRM_App;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalHistory')
BEGIN
    CREATE TABLE MedicalHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        DoctorId INT NULL,
        Diagnosis NVARCHAR(255) NOT NULL,
        Treatment NVARCHAR(500) NULL,
        VisitDate DATETIME NOT NULL DEFAULT GETDATE(),
        Notes NVARCHAR(1000) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT FK_MedicalHistory_Patients FOREIGN KEY (PatientId)
            REFERENCES Patients(Id) ON DELETE CASCADE
    );
END
GO

-- Seed sample data
INSERT INTO MedicalHistory (PatientId, DoctorId, Diagnosis, Treatment, VisitDate, Notes)
VALUES
(1, 1, 'Common Cold', 'Rest and fluids, Paracetamol 500mg', '2026-06-10', 'Patient advised to follow up if symptoms persist'),
(1, 1, 'Routine Checkup', 'No treatment needed', '2026-05-01', 'All vitals normal'),
(2, 1, 'Hypertension', 'Prescribed Amlodipine 5mg daily', '2026-06-15', 'Blood pressure monitored weekly');
GO
