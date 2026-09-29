using MediatR;
using InnoAppointmentsApi.Dtos;

namespace InnoAppointmentsApi.Features.Results;

public sealed record CreateResultCommand(
    Guid AppointmentId, 
    string Complaints, 
    string Diagnosis, 
    string Recommendations) : IRequest<Guid>;

public sealed record UpdateResultCommand(
    Guid Id, 
    string Complaints, 
    string Diagnosis, 
    string Recommendations) : IRequest;

public sealed record DeleteResultCommand(Guid Id) : IRequest;

public sealed record GetResultByIdQuery(Guid Id) : IRequest<ResultDto?>;
public sealed record GetResultByAppointmentIdQuery(Guid AppointmentId) : IRequest<ResultDto?>;