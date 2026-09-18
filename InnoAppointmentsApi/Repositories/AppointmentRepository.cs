using Dapper;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Interfaces;

namespace InnoAppointmentsApi.Repositories;

public sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AppointmentRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"SELECT * FROM ""Appointments"" WHERE ""Id"" = @Id";
        
        return await connection.QuerySingleOrDefaultAsync<Appointment>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"SELECT * FROM ""Appointments"" WHERE ""DoctorId"" = @DoctorId";
        
        return await connection.QueryAsync<Appointment>(sql, new { DoctorId = doctorId });
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"SELECT * FROM ""Appointments"" WHERE ""PatientId"" = @PatientId";
        
        return await connection.QueryAsync<Appointment>(sql, new { PatientId = patientId });
    }

    public async Task AddAsync(Appointment appointment)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            INSERT INTO ""Appointments"" (""Id"", ""PatientId"", ""DoctorId"", ""ServiceId"", ""Date"", ""Time"", ""IsApproved"")
            VALUES (@Id, @PatientId, @DoctorId, @ServiceId, @Date, @Time, @IsApproved)";
            
        await connection.ExecuteAsync(sql, appointment);
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"
            UPDATE ""Appointments"" 
            SET ""PatientId"" = @PatientId, 
                ""DoctorId"" = @DoctorId, 
                ""ServiceId"" = @ServiceId, 
                ""Date"" = @Date, 
                ""Time"" = @Time, 
                ""IsApproved"" = @IsApproved
            WHERE ""Id"" = @Id";
            
        await connection.ExecuteAsync(sql, appointment);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        const string sql = @"DELETE FROM ""Appointments"" WHERE ""Id"" = @Id";
        
        await connection.ExecuteAsync(sql, new { Id = id });
    }
}