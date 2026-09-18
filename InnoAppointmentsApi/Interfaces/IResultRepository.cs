using InnoAppointmentsApi.Entities;

namespace InnoAppointmentsApi.Interfaces;

public interface IResultRepository
{
    Task<Result?> GetByIdAsync(Guid id);
    Task<Result?> GetByAppointmentIdAsync(Guid appointmentId);
    Task AddAsync(Result result);
    Task UpdateAsync(Result result);
    Task DeleteAsync(Guid id);
}