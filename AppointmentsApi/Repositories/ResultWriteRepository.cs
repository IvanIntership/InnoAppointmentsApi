using Dapper;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Interfaces;

namespace InnoAppointmentsApi.Repositories;

public sealed class ResultWriteRepository : IResultWriteRepository
{
    private readonly IWriteDbConnectionFactory _connectionFactory;

    public ResultWriteRepository(IWriteDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<Result?> GetByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"SELECT * FROM Results WHERE Id = @Id";
        
        return await connection.QuerySingleOrDefaultAsync<Result>(sql, new { Id = id });
    }

    public async Task<Result?> GetByAppointmentIdAsync(Guid appointmentId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"SELECT * FROM Results WHERE AppointmentId = @AppointmentId";
        
        return await connection.QuerySingleOrDefaultAsync<Result>(sql, new { AppointmentId = appointmentId });
    }

    public async Task AddAsync(Result result)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            INSERT INTO Results (Id, AppointmentId, Complaints, Diagnosis, Recommendations)
            VALUES (@Id, @AppointmentId, @Complaints, @Diagnosis, @Recommendations)";
            
        await connection.ExecuteAsync(sql, result);
    }

    public async Task UpdateAsync(Result result)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            UPDATE Results 
            SET Complaints = @Complaints, 
                Diagnosis = @Diagnosis, 
                Recommendations = @Recommendations
            WHERE Id = @Id";
            
        await connection.ExecuteAsync(sql, result);
    }
    
    public async Task DeleteAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"DELETE FROM Results WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}