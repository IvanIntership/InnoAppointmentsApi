using InnoAppointmentsApi.Entities;
using MediatR;

namespace InnoAppointmentsApi.Features.Results;

public record ResultCreatedEvent(Result Result) : INotification;
public record ResultUpdatedEvent(Result Result) : INotification;
public record ResultDeletedEvent(Guid Id) : INotification;