using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Interfaces;
using MediatR;

namespace InnoAppointmentsApi.Features.Appointments;

public sealed class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IAppointmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly IExternalValidationService _externalValidation;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository repository, 
        IMapper mapper, 
        IExternalValidationService externalValidation)
    {
        _repository = repository;
        _mapper = mapper;
        _externalValidation = externalValidation;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (!await _externalValidation.PatientExistsAsync(request.PatientId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.PatientId), "Patient not found") });

        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.DoctorId), "Doctor not found") });

        if (!await _externalValidation.ServiceExistsAsync(request.ServiceId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.ServiceId), "Service not found") });

        var appointment = _mapper.Map<Appointment>(request);
        appointment.Id = Guid.NewGuid();
        
        await _repository.AddAsync(appointment);
        
        return appointment.Id;
    }
}

public sealed class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand>
{
    private readonly IAppointmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly IExternalValidationService _externalValidation;

    public UpdateAppointmentCommandHandler(
        IAppointmentRepository repository, 
        IMapper mapper, 
        IExternalValidationService externalValidation)
    {
        _repository = repository;
        _mapper = mapper;
        _externalValidation = externalValidation;
    }

    public async Task Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (!await _externalValidation.PatientExistsAsync(request.PatientId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.PatientId), "Patient not found") });

        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.DoctorId), "Doctor not found") });

        if (!await _externalValidation.ServiceExistsAsync(request.ServiceId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.ServiceId), "Service not found") });

        var appointment = _mapper.Map<Appointment>(request);
        await _repository.UpdateAsync(appointment);
    }
}

public sealed class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand>
{
    private readonly IAppointmentRepository _repository;

    public DeleteAppointmentCommandHandler(IAppointmentRepository repository) => _repository = repository;

    public async Task Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}

public sealed class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
{
    private readonly IAppointmentRepository _repository;
    private readonly IMapper _mapper;

    public GetAppointmentByIdQueryHandler(IAppointmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<AppointmentDto?>(appointment);
    }
}

public sealed class GetAppointmentsByDoctorIdQueryHandler : IRequestHandler<GetAppointmentsByDoctorIdQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;
    private readonly IMapper _mapper;

    public GetAppointmentsByDoctorIdQueryHandler(IAppointmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByDoctorIdQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetByDoctorIdAsync(request.DoctorId);
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }
}

public sealed class GetAppointmentsByPatientIdQueryHandler : IRequestHandler<GetAppointmentsByPatientIdQuery, IEnumerable<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;
    private readonly IMapper _mapper;

    public GetAppointmentsByPatientIdQueryHandler(IAppointmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByPatientIdQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetByPatientIdAsync(request.PatientId);
        return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
    }
}