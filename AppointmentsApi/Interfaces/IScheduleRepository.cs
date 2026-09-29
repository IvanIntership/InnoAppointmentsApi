using InnoAppointmentsApi.Entities;

namespace InnoAppointmentsApi.Interfaces;

public interface IScheduleWriteRepository
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetByDoctorIdAsync(Guid doctorId);
    Task<bool> ExistsAsync(Guid doctorId, int year, int month);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(Guid id);
}

public interface IScheduleReadRepository
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetByDoctorIdAsync(Guid doctorId);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(Guid id);
}