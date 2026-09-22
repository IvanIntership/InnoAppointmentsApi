using InnoAppointmentsApi.Entities;

namespace InnoAppointmentsApi.Interfaces;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task<IEnumerable<Schedule>> GetByDoctorIdAsync(Guid doctorId);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid doctorId, int year, int month);
}