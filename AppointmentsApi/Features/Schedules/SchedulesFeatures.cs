using MediatR;
using InnoAppointmentsApi.Dtos;

namespace InnoAppointmentsApi.Features.Schedules;

public record CreateScheduleCommand(
    Guid DoctorId, 
    int Year, 
    int Month, 
    List<WorkDayDto> WorkDays) : IRequest<Guid>;

public record UpdateScheduleCommand(
    Guid Id, 
    Guid DoctorId, 
    int Year, 
    int Month, 
    List<WorkDayDto> WorkDays) : IRequest;

public record DeleteScheduleCommand(Guid Id) : IRequest;

public record GetScheduleByIdQuery(Guid Id) : IRequest<ScheduleDto?>;
public record GetSchedulesByDoctorIdQuery(Guid DoctorId) : IRequest<IEnumerable<ScheduleDto>>;