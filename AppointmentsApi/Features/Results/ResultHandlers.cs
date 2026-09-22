using AutoMapper;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Exceptions;
using InnoAppointmentsApi.Interfaces;
using MediatR;

namespace InnoAppointmentsApi.Features.Results;

public sealed class ResultCommandHandlers : 
    IRequestHandler<CreateResultCommand, Guid>,
    IRequestHandler<UpdateResultCommand>,
    IRequestHandler<DeleteResultCommand>
{
    private readonly IResultRepository _resultRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public ResultCommandHandlers(
        IResultRepository resultRepository, 
        IAppointmentRepository appointmentRepository,
        IMapper mapper)
    {
        _resultRepository = resultRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateResultCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
        if (appointment == null)
            throw new NotFoundException("Appointment", request.AppointmentId);

        if (!appointment.IsApproved)
            throw new BusinessRuleException("Cannot create a result for an unapproved appointment.");
        
        if (appointment.Date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BusinessRuleException("Cannot create a result for a future appointment.");
        
        var existingResult = await _resultRepository.GetByAppointmentIdAsync(request.AppointmentId);
        if (existingResult != null)
            throw new ConflictException("Result for this appointment already exists.");

        var result = _mapper.Map<Result>(request);
        result.Id = Guid.NewGuid();
        
        await _resultRepository.AddAsync(result);
        
        return result.Id;
    }

    public async Task Handle(UpdateResultCommand request, CancellationToken cancellationToken)
    {
        var existingResult = await _resultRepository.GetByIdAsync(request.Id);
        if (existingResult == null)
            throw new NotFoundException("Result", request.Id);

        var result = _mapper.Map<Result>(request);

        await _resultRepository.UpdateAsync(result);
    }

    public async Task Handle(DeleteResultCommand request, CancellationToken cancellationToken)
    {
        await _resultRepository.DeleteAsync(request.Id);
    }
}

public sealed class ResultQueryHandlers : 
    IRequestHandler<GetResultByIdQuery, ResultDto?>,
    IRequestHandler<GetResultByAppointmentIdQuery, ResultDto?>
{
    private readonly IResultRepository _repository;
    private readonly IMapper _mapper;

    public ResultQueryHandlers(IResultRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResultDto?> Handle(GetResultByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<ResultDto?>(result);
    }

    public async Task<ResultDto?> Handle(GetResultByAppointmentIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByAppointmentIdAsync(request.AppointmentId);
        return _mapper.Map<ResultDto?>(result);
    }
}