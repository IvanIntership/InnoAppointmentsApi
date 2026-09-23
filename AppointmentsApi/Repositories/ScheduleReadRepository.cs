using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Interfaces;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

namespace InnoAppointmentsApi.Repositories;

public sealed class ScheduleReadRepository : IScheduleReadRepository
{
    private readonly IMongoCollection<Schedule> _collection;
    
    public ScheduleReadRepository([FromKeyedServices("MongoRead")] IMongoDatabase database)
    {
        _collection = database.GetCollection<Schedule>("schedules");
    }

    public async Task<Schedule?> GetByIdAsync(Guid id)
    {
        var filter = Builders<Schedule>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByDoctorIdAsync(Guid doctorId)
    {
        var filter = Builders<Schedule>.Filter.Eq(x => x.DoctorId, doctorId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task AddAsync(Schedule schedule)
    {
        await _collection.InsertOneAsync(schedule);
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        var filter = Builders<Schedule>.Filter.Eq(x => x.Id, schedule.Id);
        await _collection.ReplaceOneAsync(filter, schedule);
    }
    
    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<Schedule>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }
}