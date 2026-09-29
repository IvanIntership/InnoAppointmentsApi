using AutoMapper;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Exceptions;
using InnoAppointmentsApi.Interfaces;
using MediatR;

namespace InnoAppointmentsApi.Features.Schedules;

public sealed class ScheduleCommandHandlers : 
    IRequestHandler<CreateScheduleCommand, Guid>,
    IRequestHandler<UpdateScheduleCommand>,
    IRequestHandler<DeleteScheduleCommand>
{
    private readonly IScheduleWriteRepository _repository;
    private readonly IAppointmentWriteRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly IExternalValidationService _externalValidation;
    private readonly IPublisher _publisher;

    public ScheduleCommandHandlers(
        IScheduleWriteRepository repository, 
        IAppointmentWriteRepository appointmentRepository,
        IMapper mapper, 
        IExternalValidationService externalValidation,
        IPublisher publisher)
    {
        _repository = repository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _externalValidation = externalValidation;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new NotFoundException("Doctor", request.DoctorId);
        
        if (await _repository.ExistsAsync(request.DoctorId, request.Year, request.Month))
            throw new ConflictException("A schedule for this doctor in this month already exists.");

        var schedule = _mapper.Map<Schedule>(request);
        schedule.Id = Guid.NewGuid();
        
        await _repository.AddAsync(schedule);
        
        await _publisher.Publish(new ScheduleCreatedEvent(schedule), cancellationToken);
        
        return schedule.Id;
    }

    public async Task Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var existingSchedule = await _repository.GetByIdAsync(request.Id);
        if (existingSchedule == null)
            throw new NotFoundException("Schedule", request.Id);

        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new NotFoundException("Doctor", request.DoctorId);
        
        if (existingSchedule.Year != request.Year || existingSchedule.Month != request.Month)
        {
            if (await _repository.ExistsAsync(request.DoctorId, request.Year, request.Month))
                throw new ConflictException("A schedule for this new month already exists.");
        }

        var monthAppointments = (await _appointmentRepository.GetByDoctorIdAsync(request.DoctorId))
            .Where(a => a.Date.Year == request.Year && a.Date.Month == request.Month)
            .ToList();
        
        foreach (var appointment in monthAppointments)
        {
            var newWorkDay = request.WorkDays.FirstOrDefault(w => w.Date == appointment.Date);
            
            if (newWorkDay == null)
                throw new BusinessRuleException($"Cannot remove work day {appointment.Date} because there are existing appointments.");

            var serviceDuration = await _externalValidation.GetServiceDurationAsync(appointment.ServiceId, cancellationToken);
            var appointmentEndTime = appointment.Time.Add(serviceDuration);

            if (appointment.Time < newWorkDay.StartTime || appointmentEndTime > newWorkDay.EndTime)
                throw new BusinessRuleException($"Appointment on {appointment.Date} at {appointment.Time} falls outside the new working hours.");
        }

        var schedule = _mapper.Map<Schedule>(request);
        await _repository.UpdateAsync(schedule);

        await _publisher.Publish(new ScheduleUpdatedEvent(schedule), cancellationToken);
    }

    public async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
        
        await _publisher.Publish(new ScheduleDeletedEvent(request.Id), cancellationToken);
    }
}

public sealed class ScheduleQueryHandlers : 
    IRequestHandler<GetScheduleByIdQuery, ScheduleDto?>,
    IRequestHandler<GetSchedulesByDoctorIdQuery, IEnumerable<ScheduleDto>>
{
    private readonly IScheduleReadRepository _repository;
    private readonly IMapper _mapper;

    public ScheduleQueryHandlers(IScheduleReadRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ScheduleDto?> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<ScheduleDto?>(schedule);
    }

    public async Task<IEnumerable<ScheduleDto>> Handle(GetSchedulesByDoctorIdQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _repository.GetByDoctorIdAsync(request.DoctorId);
        return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
    }
}