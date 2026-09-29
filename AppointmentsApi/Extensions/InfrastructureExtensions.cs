using Dapper;
using InnoAppointmentsApi.Data;
using InnoAppointmentsApi.Interfaces;
using InnoAppointmentsApi.Repositories;
using InnoAppointmentsApi.TypeHandlers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace InnoAppointmentsApi.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddDatabaseInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresWriteConnection = configuration.GetConnectionString("PostgresWriteConnection") 
                                      ?? throw new InvalidOperationException("PostgresWriteConnection not found.");
        var postgresReadConnection = configuration.GetConnectionString("PostgresReadConnection") 
                                     ?? throw new InvalidOperationException("PostgresReadConnection not found.");
        var mongoWriteConnection = configuration.GetConnectionString("MongoWriteConnection") 
                                   ?? throw new InvalidOperationException("MongoWriteConnection not found.");
        var mongoReadConnection = configuration.GetConnectionString("MongoReadConnection") 
                                  ?? throw new InvalidOperationException("MongoReadConnection not found.");

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

        DatabaseInitializer.Initialize(postgresWriteConnection, postgresReadConnection);

        services.AddKeyedSingleton<IDbConnectionFactory>("PostgresWrite", (sp, key) => 
            new WriteDbConnectionFactory(postgresWriteConnection));

        services.AddKeyedSingleton<IDbConnectionFactory>("PostgresRead", (sp, key) => 
            new ReadDbConnectionFactory(postgresReadConnection));

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        MongoClassMap.Register();

        services.AddKeyedSingleton<IMongoDatabase>("MongoWrite", (sp, key) =>
        {
            var client = new MongoClient(mongoWriteConnection);
            return client.GetDatabase(MongoUrl.Create(mongoWriteConnection).DatabaseName);
        });

        services.AddKeyedSingleton<IMongoDatabase>("MongoRead", (sp, key) =>
        {
            var client = new MongoClient(mongoReadConnection);
            return client.GetDatabase(MongoUrl.Create(mongoReadConnection).DatabaseName);
        });

        services.AddScoped<IAppointmentWriteRepository, AppointmentWriteRepository>();
        services.AddScoped<IAppointmentReadRepository, AppointmentReadRepository>();
        services.AddScoped<IResultWriteRepository, ResultWriteRepository>();
        services.AddScoped<IResultReadRepository, ResultReadRepository>();
        services.AddScoped<IScheduleWriteRepository, ScheduleWriteRepository>();
        services.AddScoped<IScheduleReadRepository, ScheduleReadRepository>();

        return services;
    }
}