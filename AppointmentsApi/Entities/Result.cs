namespace InnoAppointmentsApi.Entities;

public sealed class Result
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public string Complaints { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Recommendations { get; set; } = string.Empty;
}