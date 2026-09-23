using System.Security.Claims;
using System.Text.Json;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using InnoAppointmentsApi.Data;
using InnoAppointmentsApi.TypeHandlers;
using InnoAppointmentsApi.Interfaces;
using InnoAppointmentsApi.Repositories;
using InnoAppointmentsApi.Behaviors;
using InnoAppointmentsApi.Constants;
using InnoAppointmentsApi.Middleware;
using InnoAppointmentsApi.Services;
using Microsoft.OpenApi;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

var postgresWriteConnection = builder.Configuration.GetConnectionString("PostgresWriteConnection") 
                               ?? throw new InvalidOperationException("PostgresWriteConnection not found.");
var postgresReadConnection = builder.Configuration.GetConnectionString("PostgresReadConnection") 
                               ?? throw new InvalidOperationException("PostgresReadConnection not found.");
var mongoWriteConnection = builder.Configuration.GetConnectionString("MongoWriteConnection") 
                            ?? throw new InvalidOperationException("MongoWriteConnection not found.");
var mongoReadConnection = builder.Configuration.GetConnectionString("MongoReadConnection") 
                            ?? throw new InvalidOperationException("MongoReadConnection not found.");

SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

DatabaseInitializer.Initialize(postgresWriteConnection, postgresReadConnection);

builder.Services.AddSingleton<IWriteDbConnectionFactory>(_ => new WriteDbConnectionFactory(postgresWriteConnection));
builder.Services.AddSingleton<IReadDbConnectionFactory>(_ => new ReadDbConnectionFactory(postgresReadConnection));

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
MongoClassMap.Register();

builder.Services.AddKeyedSingleton<IMongoDatabase>("MongoWrite", (sp, key) =>
{
    var client = new MongoClient(mongoWriteConnection);
    return client.GetDatabase(MongoUrl.Create(mongoWriteConnection).DatabaseName);
});

builder.Services.AddKeyedSingleton<IMongoDatabase>("MongoRead", (sp, key) =>
{
    var client = new MongoClient(mongoReadConnection);
    return client.GetDatabase(MongoUrl.Create(mongoReadConnection).DatabaseName);
});

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));

builder.Services.AddScoped<IAppointmentWriteRepository, AppointmentWriteRepository>();
builder.Services.AddScoped<IAppointmentReadRepository, AppointmentReadRepository>();
builder.Services.AddScoped<IResultWriteRepository, ResultWriteRepository>();
builder.Services.AddScoped<IResultReadRepository, ResultReadRepository>();
builder.Services.AddScoped<IScheduleWriteRepository, ScheduleWriteRepository>();
builder.Services.AddScoped<IScheduleReadRepository, ScheduleReadRepository>();

builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = builder.Environment.ApplicationName, 
        Version = "v1" 
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",          
        BearerFormat = "JWT",
        Description = "Enter JWT token (without Bearer prefix)"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement((doc) =>
    {
        var requirement = new OpenApiSecurityRequirement();
        var reference = new OpenApiSecuritySchemeReference("Bearer", doc);
        requirement[reference] = new List<string>(); 
    
        return requirement;
    });
});

builder.Services.AddHttpClient("GatewayClient", client =>
{
    var gatewayBaseUrl = builder.Configuration["Gateway:BaseUrl"] ?? throw new InvalidOperationException("Gateway:BaseUrl configuration is missing.");
                         
    var clientId = builder.Configuration["Gateway:ClientId"] ?? "AppointmentsMicroservice";

    client.BaseAddress = new Uri(gatewayBaseUrl);
    client.DefaultRequestHeaders.Add("ClientId", clientId);
});

builder.Services.AddScoped<IExternalValidationService, ExternalValidationService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keycloakBaseUrl = builder.Configuration["Keycloak:BaseUrl"];
    var keycloakRealm = builder.Configuration["Keycloak:Realm"];

    if (string.IsNullOrWhiteSpace(keycloakBaseUrl) || string.IsNullOrWhiteSpace(keycloakRealm))
        throw new InvalidOperationException("Keycloak configuration is missing.");

    var authority = $"{keycloakBaseUrl.TrimEnd('/')}/realms/{keycloakRealm}";

    options.Authority = authority;
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = authority,
        ValidateAudience = false
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
            {
                var realmAccessClaim = claimsIdentity.FindFirst("realm_access");
                if (realmAccessClaim != null)
                {
                    using var doc = JsonDocument.Parse(realmAccessClaim.Value);
                    if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                    {
                        foreach (var role in rolesElement.EnumerateArray())
                        {
                            var roleName = role.GetString();
                            if (!string.IsNullOrEmpty(roleName))
                            {
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicies.RequireAdmin, policy => 
        policy.RequireRole("Administrator"));
        
    options.AddPolicy(AuthPolicies.RequireStaff, policy => 
        policy.RequireRole("Administrator", "Doctor"));
        
    options.AddPolicy(AuthPolicies.RequirePatientOrAdmin, policy => 
        policy.RequireRole("Administrator", "Patient"));
        
    options.AddPolicy(AuthPolicies.RequireAllRoles, policy => 
        policy.RequireRole("Administrator", "Doctor", "Patient"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();