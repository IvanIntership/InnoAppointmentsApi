using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Interfaces;
using MediatR;

namespace InnoAppointmentsApi.Features.Schedules;

public sealed class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Guid>
{
    private readonly IScheduleRepository _repository;
    private readonly IMapper _mapper;
    private readonly IExternalValidationService _externalValidation;

    public CreateScheduleCommandHandler(
        IScheduleRepository repository, 
        IMapper mapper, 
        IExternalValidationService externalValidation)
    {
        _repository = repository;
        _mapper = mapper;
        _externalValidation = externalValidation;
    }

    public async Task<Guid> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.DoctorId), "Doctor not found") });

        var schedule = _mapper.Map<Schedule>(request);
        schedule.Id = Guid.NewGuid();
        
        await _repository.AddAsync(schedule);
        
        return schedule.Id;
    }
}

public sealed class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand>
{
    private readonly IScheduleRepository _repository;
    private readonly IMapper _mapper;
    private readonly IExternalValidationService _externalValidation;

    public UpdateScheduleCommandHandler(
        IScheduleRepository repository, 
        IMapper mapper, 
        IExternalValidationService externalValidation)
    {
        _repository = repository;
        _mapper = mapper;
        _externalValidation = externalValidation;
    }

    public async Task Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        if (!await _externalValidation.DoctorExistsAsync(request.DoctorId, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.DoctorId), "Doctor not found") });

        var schedule = _mapper.Map<Schedule>(request);
        await _repository.UpdateAsync(schedule);
    }
}

public sealed class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand>
{
    private readonly IScheduleRepository _repository;

    public DeleteScheduleCommandHandler(IScheduleRepository repository) => _repository = repository;

    public async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}

public sealed class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, ScheduleDto?>
{
    private readonly IScheduleRepository _repository;
    private readonly IMapper _mapper;

    public GetScheduleByIdQueryHandler(IScheduleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ScheduleDto?> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<ScheduleDto?>(schedule);
    }
}

public sealed class GetSchedulesByDoctorIdQueryHandler : IRequestHandler<GetSchedulesByDoctorIdQuery, IEnumerable<ScheduleDto>>
{
    private readonly IScheduleRepository _repository;
    private readonly IMapper _mapper;

    public GetSchedulesByDoctorIdQueryHandler(IScheduleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ScheduleDto>> Handle(GetSchedulesByDoctorIdQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _repository.GetByDoctorIdAsync(request.DoctorId);
        return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
    }
}