namespace InnoAppointmentsApi.Interfaces;

public interface IExternalValidationService
{
    Task<bool> DoctorExistsAsync(Guid doctorId, CancellationToken cancellationToken);
    Task<bool> PatientExistsAsync(Guid patientId, CancellationToken cancellationToken);
    Task<bool> ServiceExistsAsync(Guid serviceId, CancellationToken cancellationToken);
    Task<TimeSpan> GetServiceDurationAsync(Guid serviceId, CancellationToken cancellationToken);
    Task<bool> DoctorProvidesServiceAsync(Guid doctorId, Guid serviceId, CancellationToken cancellationToken = default);
}