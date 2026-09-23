using InnoAppointmentsApi.Entities;
using MediatR;

namespace InnoAppointmentsApi.Features.Appointments;

public record AppointmentCreatedEvent(Appointment Appointment) : INotification;
public record AppointmentUpdatedEvent(Appointment Appointment) : INotification;
public record AppointmentDeletedEvent(Guid Id) : INotification;