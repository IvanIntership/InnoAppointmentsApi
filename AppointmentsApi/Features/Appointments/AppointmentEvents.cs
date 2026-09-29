using InnoAppointmentsApi.Entities;
using MediatR;

namespace InnoAppointmentsApi.Features.Appointments;

public sealed record AppointmentCreatedEvent(Appointment Appointment) : INotification;
public sealed record AppointmentUpdatedEvent(Appointment Appointment) : INotification;
public sealed record AppointmentDeletedEvent(Guid Id) : INotification;