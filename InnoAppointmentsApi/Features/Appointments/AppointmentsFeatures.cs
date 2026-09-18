using MediatR;
using InnoAppointmentsApi.Dtos;

namespace InnoAppointmentsApi.Features.Appointments;

public sealed record CreateAppointmentCommand(
    Guid PatientId, 
    Guid DoctorId, 
    Guid ServiceId, 
    DateOnly Date, 
    TimeOnly Time) : IRequest<Guid>;

public sealed record UpdateAppointmentCommand(
    Guid Id, 
    Guid PatientId, 
    Guid DoctorId, 
    Guid ServiceId, 
    DateOnly Date, 
    TimeOnly Time, 
    bool IsApproved) : IRequest;

public sealed record DeleteAppointmentCommand(Guid Id) : IRequest;

public sealed record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentDto?>;
public sealed record GetAppointmentsByDoctorIdQuery(Guid DoctorId) : IRequest<IEnumerable<AppointmentDto>>;
public sealed record GetAppointmentsByPatientIdQuery(Guid PatientId) : IRequest<IEnumerable<AppointmentDto>>;