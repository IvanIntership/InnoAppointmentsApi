using MediatR;
using InnoAppointmentsApi.Interfaces;

namespace InnoAppointmentsApi.Features.Appointments;

public sealed class AppointmentSyncHandlers :
    INotificationHandler<AppointmentCreatedEvent>,
    INotificationHandler<AppointmentUpdatedEvent>,
    INotificationHandler<AppointmentDeletedEvent>
{
    private readonly IAppointmentReadRepository _readRepository;

    public AppointmentSyncHandlers(IAppointmentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task Handle(AppointmentCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.AddAsync(notification.Appointment);
    }

    public async Task Handle(AppointmentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.UpdateAsync(notification.Appointment);
    }

    public async Task Handle(AppointmentDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.DeleteAsync(notification.Id);
    }
}