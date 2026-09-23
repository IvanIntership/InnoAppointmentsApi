using InnoAppointmentsApi.Entities;

namespace InnoAppointmentsApi.Interfaces;

public interface IResultWriteRepository
{
    Task AddAsync(Result result);
    Task UpdateAsync(Result result);
    Task DeleteAsync(Guid id);
    
    Task<Result?> GetByIdAsync(Guid id);
    Task<Result?> GetByAppointmentIdAsync(Guid appointmentId);
}

public interface IResultReadRepository
{
    Task<Result?> GetByIdAsync(Guid id);
    Task<Result?> GetByAppointmentIdAsync(Guid appointmentId);

    Task AddAsync(Result result);
    Task UpdateAsync(Result result);
    Task DeleteAsync(Guid id);
}