using Dapper;
using FluentValidation;
using MongoDB.Driver;
using InnoAppointmentsApi.Data;
using InnoAppointmentsApi.TypeHandlers;
using InnoAppointmentsApi.Interfaces;
using InnoAppointmentsApi.Repositories;
using InnoAppointmentsApi.Behaviors;
using InnoAppointmentsApi.Services;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection") 
                               ?? throw new InvalidOperationException("Postgres connection string not found.");

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoLogging") 
                            ?? throw new InvalidOperationException("Mongo connection string not found.");

SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

DatabaseInitializer.Initialize(postgresConnectionString);

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(postgresConnectionString));

MongoClassMap.Register();

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var mongoUrl = MongoUrl.Create(mongoConnectionString);
    return client.GetDatabase(mongoUrl.DatabaseName);
});

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddHttpClient("GatewayClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000");
});

builder.Services.AddScoped<IExternalValidationService, ExternalValidationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();