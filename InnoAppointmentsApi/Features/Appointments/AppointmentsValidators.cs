using FluentValidation;

namespace InnoAppointmentsApi.Features.Appointments;

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.Date).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}

public sealed class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.ServiceId).NotEmpty();
    }
}

public sealed class DeleteAppointmentCommandValidator : AbstractValidator<DeleteAppointmentCommand>
{
    public DeleteAppointmentCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class GetAppointmentByIdQueryValidator : AbstractValidator<GetAppointmentByIdQuery>
{
    public GetAppointmentByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}

public sealed class GetAppointmentsByDoctorIdQueryValidator : AbstractValidator<GetAppointmentsByDoctorIdQuery>
{
    public GetAppointmentsByDoctorIdQueryValidator() => RuleFor(x => x.DoctorId).NotEmpty();
}

public sealed class GetAppointmentsByPatientIdQueryValidator : AbstractValidator<GetAppointmentsByPatientIdQuery>
{
    public GetAppointmentsByPatientIdQueryValidator() => RuleFor(x => x.PatientId).NotEmpty();
}