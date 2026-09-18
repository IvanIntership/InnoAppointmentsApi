using FluentValidation;
using InnoAppointmentsApi.Dtos;

namespace InnoAppointmentsApi.Features.Schedules;

public sealed class WorkDayDtoValidator : AbstractValidator<WorkDayDto>
{
    public WorkDayDtoValidator()
    {
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime)
            .WithMessage("EndTime must be after StartTime");
    }
}

public sealed class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(2024, 2100);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleForEach(x => x.WorkDays).SetValidator(new WorkDayDtoValidator());
    }
}

public sealed class UpdateScheduleCommandValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Year).InclusiveBetween(2024, 2100);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleForEach(x => x.WorkDays).SetValidator(new WorkDayDtoValidator());
    }
}

public sealed class DeleteScheduleCommandValidator : AbstractValidator<DeleteScheduleCommand>
{
    public DeleteScheduleCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class GetScheduleByIdQueryValidator : AbstractValidator<GetScheduleByIdQuery>
{
    public GetScheduleByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class GetSchedulesByDoctorIdQueryValidator : AbstractValidator<GetSchedulesByDoctorIdQuery>
{
    public GetSchedulesByDoctorIdQueryValidator() => RuleFor(x => x.DoctorId).NotEmpty();
}