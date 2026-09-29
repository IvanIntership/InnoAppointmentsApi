namespace InnoAppointmentsApi.Entities;

public sealed class Schedule
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<WorkDay> WorkDays { get; set; } = new();
}