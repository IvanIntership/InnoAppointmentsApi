CREATE INDEX IF NOT EXISTS idx_appointments_patient_id ON Appointments(PatientId);

CREATE INDEX IF NOT EXISTS idx_appointments_doctor_id ON Appointments(DoctorId);

CREATE INDEX IF NOT EXISTS idx_appointments_doctor_date ON Appointments(DoctorId, Date);

CREATE UNIQUE INDEX IF NOT EXISTS idx_results_appointment_id ON Results(AppointmentId);