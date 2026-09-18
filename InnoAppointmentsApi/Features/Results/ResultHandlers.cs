using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Interfaces;
using MediatR;

namespace InnoAppointmentsApi.Features.Results;

public sealed class CreateResultCommandHandler : IRequestHandler<CreateResultCommand, Guid>
{
    private readonly IResultRepository _resultRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public CreateResultCommandHandler(
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
            throw new ValidationException(new[] { new ValidationFailure(nameof(request.AppointmentId), "Appointment not found") });

        var result = _mapper.Map<Result>(request);
        result.Id = Guid.NewGuid();
        
        await _resultRepository.AddAsync(result);
        
        return result.Id;
    }
}

public sealed class UpdateResultCommandHandler : IRequestHandler<UpdateResultCommand>
{
    private readonly IResultRepository _repository;
    private readonly IMapper _mapper;

    public UpdateResultCommandHandler(IResultRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateResultCommand request, CancellationToken cancellationToken)
    {
        var result = _mapper.Map<Result>(request);
        await _repository.UpdateAsync(result);
    }
}

public sealed class DeleteResultCommandHandler : IRequestHandler<DeleteResultCommand>
{
    private readonly IResultRepository _repository;

    public DeleteResultCommandHandler(IResultRepository repository) => _repository = repository;

    public async Task Handle(DeleteResultCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id);
    }
}

public sealed class GetResultByIdQueryHandler : IRequestHandler<GetResultByIdQuery, ResultDto?>
{
    private readonly IResultRepository _repository;
    private readonly IMapper _mapper;

    public GetResultByIdQueryHandler(IResultRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResultDto?> Handle(GetResultByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<ResultDto?>(result);
    }
}

public sealed class GetResultByAppointmentIdQueryHandler : IRequestHandler<GetResultByAppointmentIdQuery, ResultDto?>
{
    private readonly IResultRepository _repository;
    private readonly IMapper _mapper;

    public GetResultByAppointmentIdQueryHandler(IResultRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResultDto?> Handle(GetResultByAppointmentIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByAppointmentIdAsync(request.AppointmentId);
        return _mapper.Map<ResultDto?>(result);
    }
}