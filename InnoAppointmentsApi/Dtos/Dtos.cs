namespace InnoAppointmentsApi.Dtos;

public sealed record AppointmentDto(
    Guid Id, 
    Guid PatientId, 
    Guid DoctorId, 
    Guid ServiceId, 
    DateOnly Date, 
    TimeOnly Time, 
    bool IsApproved);

public sealed record ResultDto(
    Guid Id, 
    Guid AppointmentId, 
    string Complaints, 
    string Diagnosis, 
    string Recommendations);

public sealed record WorkDayDto(
    DateOnly Date, 
    TimeOnly StartTime, 
    TimeOnly EndTime);

public sealed record ScheduleDto(
    Guid Id, 
    Guid DoctorId, 
    int Year, 
    int Month, 
    List<WorkDayDto> WorkDays);