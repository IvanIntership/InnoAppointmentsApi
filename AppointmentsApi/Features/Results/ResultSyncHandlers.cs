using MediatR;
using InnoAppointmentsApi.Interfaces;

namespace InnoAppointmentsApi.Features.Results;

public sealed class ResultSyncHandlers :
    INotificationHandler<ResultCreatedEvent>,
    INotificationHandler<ResultUpdatedEvent>,
    INotificationHandler<ResultDeletedEvent>
{
    private readonly IResultReadRepository _readRepository;

    public ResultSyncHandlers(IResultReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task Handle(ResultCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.AddAsync(notification.Result);
    }

    public async Task Handle(ResultUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.UpdateAsync(notification.Result);
    }

    public async Task Handle(ResultDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _readRepository.DeleteAsync(notification.Id);
    }
}