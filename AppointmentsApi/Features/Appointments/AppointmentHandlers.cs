using AutoMapper;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Exceptions;
using InnoAppointmentsApi.Interfaces;
using MediatR;

namespace InnoAppointmentsApi.Features.Appointments;

public sealed class AppointmentCommandHandlers : 
    IRequestHandler<CreateAppointmentCommand, Guid>,
    IRequestHandler<UpdateAppointmentCommand>,
    IRequestHandler<DeleteAppointmentCommand>
{
    private readonly IAppointmentWriteRepository _appointmentRepository;
    private readonly IScheduleWriteRepository _scheduleRepository;
    private readonly IMapper _mapper;
    private readonly IExternalValidationService _externalValidation;
    private readonly IPublisher _publisher;

    public AppointmentCommandHandlers(
        IAppointmentWriteRepository appointmentRepository,
        IScheduleWriteRepository scheduleRepository,
        IMapper mapper, 
        IExternalValidationService externalValidation,
        IPublisher publisher)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
        _mapper = mapper;
        _externalValidation = externalValidation;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (!await _externalValidation.PatientExistsAsync(request.PatientId, cancellationToken))
            throw new NotFoundException("Patient", request.PatientId);

        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new NotFoundException("Doctor", request.DoctorId);

        if (!await _externalValidation.ServiceExistsAsync(request.ServiceId, cancellationToken))
            throw new NotFoundException("Service", request.ServiceId);
        
        if (!await _externalValidation.DoctorProvidesServiceAsync(request.DoctorId, request.ServiceId, cancellationToken))
            throw new BusinessRuleException("The selected doctor does not specialize in the requested service.");
        
        var serviceDuration = await _externalValidation.GetServiceDurationAsync(request.ServiceId, cancellationToken);
        var appointmentEndTime = request.Time.Add(serviceDuration);
        
        var doctorSchedules = await _scheduleRepository.GetByDoctorIdAsync(request.DoctorId);
        var currentMonthSchedule = doctorSchedules.FirstOrDefault(s => s.Year == request.Date.Year && s.Month == request.Date.Month);
        
        if (currentMonthSchedule == null)
            throw new BusinessRuleException("Doctor does not have a schedule for this month.");

        var workDay = currentMonthSchedule.WorkDays.FirstOrDefault(w => w.Date == request.Date);
        if (workDay == null)
            throw new BusinessRuleException("Doctor does not work on this date.");

        if (request.Time < workDay.StartTime || appointmentEndTime > workDay.EndTime)
            throw new BusinessRuleException("The appointment falls outside of the doctor's working hours.");
        
        var existingAppointments = (await _appointmentRepository.GetByDoctorIdAsync(request.DoctorId))
            .Where(a => a.Date == request.Date)
            .ToList();

        foreach (var existing in existingAppointments)
        {
            var existingDuration = await _externalValidation.GetServiceDurationAsync(existing.ServiceId, cancellationToken);
            var existingEndTime = existing.Time.Add(existingDuration);
            
            if (request.Time < existingEndTime && appointmentEndTime > existing.Time)
            {
                throw new ConflictException("This time slot overlaps with an existing appointment.");
            }
        }
        
        var appointment = _mapper.Map<Appointment>(request);
        appointment.Id = Guid.NewGuid();
        
        await _appointmentRepository.AddAsync(appointment);
        await _publisher.Publish(new AppointmentCreatedEvent(appointment), cancellationToken);
        
        return appointment.Id;
    }

    public async Task Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var existingAppointment = await _appointmentRepository.GetByIdAsync(request.Id);
        if (existingAppointment == null)
            throw new NotFoundException("Appointment", request.Id);

        if (!await _externalValidation.PatientExistsAsync(request.PatientId, cancellationToken))
            throw new NotFoundException("Patient", request.PatientId);

        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new NotFoundException("Doctor", request.DoctorId);

        if (!await _externalValidation.ServiceExistsAsync(request.ServiceId, cancellationToken))
            throw new NotFoundException("Service", request.ServiceId);

        if (!await _externalValidation.DoctorProvidesServiceAsync(request.DoctorId, request.ServiceId, cancellationToken))
            throw new BusinessRuleException("The selected doctor does not specialize in the requested service.");

        var serviceDuration = await _externalValidation.GetServiceDurationAsync(request.ServiceId, cancellationToken);
        var appointmentEndTime = request.Time.Add(serviceDuration);
        
        var doctorSchedules = await _scheduleRepository.GetByDoctorIdAsync(request.DoctorId);
        var currentMonthSchedule = doctorSchedules.FirstOrDefault(s => s.Year == request.Date.Year && s.Month == request.Date.Month);

        if (currentMonthSchedule == null) throw new BusinessRuleException("Doctor does not have a schedule for this month.");
        var workDay = currentMonthSchedule.WorkDays.FirstOrDefault(w => w.Date == request.Date);
        if (workDay == null) throw new BusinessRuleException("Doctor does not work on this date.");
        if (request.Time < workDay.StartTime || appointmentEndTime > workDay.EndTime) throw new BusinessRuleException("The appointment falls outside of the doctor's working hours.");

        var existingAppointments = (await _appointmentRepository.GetByDoctorIdAsync(request.DoctorId))
            .Where(a => a.Date == request.Date && a.Id != request.Id)
            .ToList();

        foreach (var existing in existingAppointments)
        {
            var existingDuration = await _externalValidation.GetServiceDurationAsync(existing.ServiceId, cancellationToken);
            var existingEndTime = existing.Time.Add(existingDuration);
            
            if (request.Time < existingEndTime && appointmentEndTime > existing.Time)
            {
                throw new ConflictException("This time slot overlaps with an existing appointment.");
            }
        }

        var appointment = _mapper.Map<Appointment>(request);
        await _appointmentRepository.UpdateAsync(appointment);

        await _publisher.Publish(new AppointmentUpdatedEvent(appointment), cancellationToken);
    }

    public async Task Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        await _appointmentRepository.DeleteAsync(request.Id);

        await _publisher.Publish(new AppointmentDeletedEvent(request.Id), cancellationToken);
    }
}

public sealed class AppointmentQueryHandlers : 
    IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>,
    IRequestHandler<GetAppointmentsByDoctorIdQuery, IEnumerable<AppointmentDto>>,
    IRequestHandler<GetAppointmentsByPatientIdQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentReadRepository _repository;
    private readonly IMapper _mapper;

    public AppointmentQueryHandlers(IAppointmentReadRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<AppointmentDto?>(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByDoctorIdQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetByDoctorIdAsync(request.DoctorId);
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByPatientIdQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetByPatientIdAsync(request.PatientId);
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }
}