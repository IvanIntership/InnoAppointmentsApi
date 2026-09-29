using InnoAppointmentsApi.Entities;
using MediatR;

namespace InnoAppointmentsApi.Features.Schedules;

public sealed record ScheduleCreatedEvent(Schedule Schedule) : INotification;
public sealed record ScheduleUpdatedEvent(Schedule Schedule) : INotification;
public sealed record ScheduleDeletedEvent(Guid Id) : INotification;