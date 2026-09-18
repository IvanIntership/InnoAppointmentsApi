namespace InnoAppointmentsApi.Entities;

public sealed class WorkDay
{
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
