using InnoAppointmentsApi.Entities;
using MediatR;

namespace InnoAppointmentsApi.Features.Schedules;

public record ScheduleCreatedEvent(Schedule Schedule) : INotification;
public record ScheduleUpdatedEvent(Schedule Schedule) : INotification;
public record ScheduleDeletedEvent(Guid Id) : INotification;