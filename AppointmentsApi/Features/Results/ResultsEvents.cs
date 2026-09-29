using InnoAppointmentsApi.Entities;
using MediatR;

namespace InnoAppointmentsApi.Features.Results;

public sealed record ResultCreatedEvent(Result Result) : INotification;
public sealed record ResultUpdatedEvent(Result Result) : INotification;
public sealed record ResultDeletedEvent(Guid Id) : INotification;