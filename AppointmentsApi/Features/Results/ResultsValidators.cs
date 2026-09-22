using FluentValidation;

namespace InnoAppointmentsApi.Features.Results;

public sealed class CreateResultCommandValidator : AbstractValidator<CreateResultCommand>
{
    public CreateResultCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.Complaints).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Diagnosis).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Recommendations).MaximumLength(2000);
    }
}

public sealed class UpdateResultCommandValidator : AbstractValidator<UpdateResultCommand>
{
    public UpdateResultCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Complaints).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Diagnosis).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Recommendations).MaximumLength(2000);
    }
}

public sealed class DeleteResultCommandValidator : AbstractValidator<DeleteResultCommand>
{
    public DeleteResultCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class GetResultByIdQueryValidator : AbstractValidator<GetResultByIdQuery>
{
    public GetResultByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class GetResultByAppointmentIdQueryValidator : AbstractValidator<GetResultByAppointmentIdQuery>
{
    public GetResultByAppointmentIdQueryValidator() => RuleFor(x => x.AppointmentId).NotEmpty();
}