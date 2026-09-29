using System.Data;
using InnoAppointmentsApi.Interfaces;
using Npgsql;

namespace InnoAppointmentsApi.Data;

public sealed class WriteDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public WriteDbConnectionFactory(string connectionString) => _connectionString = connectionString;

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}

public sealed class ReadDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    public ReadDbConnectionFactory(string connectionString) => _connectionString = connectionString;

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}