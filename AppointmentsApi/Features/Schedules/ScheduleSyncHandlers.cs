using MediatR;
using InnoAppointmentsApi.Interfaces;

namespace InnoAppointmentsApi.Features.Schedules;

public sealed class ScheduleSyncHandlers :
    INotificationHandler<ScheduleCreatedEvent>,
    INotificationHandler<ScheduleUpdatedEvent>,
    INotificationHandler<ScheduleDeletedEvent>
{
    private readonly IScheduleReadRepository _readRepository;

    public ScheduleSyncHandlers(IScheduleReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task Handle(ScheduleCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.AddAsync(notification.Schedule);
    }

    public async Task Handle(ScheduleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.UpdateAsync(notification.Schedule);
    }

    public async Task Handle(ScheduleDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.DeleteAsync(notification.Id);
    }
}