CREATE TABLE IF NOT EXISTS "Appointments" (
    "Id" UUID PRIMARY KEY,
    "PatientId" UUID NOT NULL,
    "DoctorId" UUID NOT NULL,
    "ServiceId" UUID NOT NULL,
    "Date" DATE NOT NULL,
    "Time" TIME NOT NULL,
    "IsApproved" BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS "Results" (
    "Id" UUID PRIMARY KEY,
    "AppointmentId" UUID NOT NULL UNIQUE REFERENCES "Appointments"("Id") ON DELETE CASCADE,
    "Complaints" TEXT NOT NULL,
    "Diagnosis" TEXT NOT NULL,
    "Recommendations" TEXT NOT NULL
);