using FluentValidation;
using InnoAppointmentsApi.Behaviors;
using InnoAppointmentsApi.Interfaces;
using InnoAppointmentsApi.Middleware;
using InnoAppointmentsApi.Services;

namespace InnoAppointmentsApi.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddHttpClient("GatewayClient", client =>
        {
            var gatewayBaseUrl = configuration["Gateway:BaseUrl"] 
                                 ?? throw new InvalidOperationException("Gateway:BaseUrl configuration is missing.");
                                 
            var clientId = configuration["Gateway:ClientId"] ?? "AppointmentsMicroservice";

            client.BaseAddress = new Uri(gatewayBaseUrl);
            client.DefaultRequestHeaders.Add("ClientId", clientId);
        });

        services.AddScoped<IExternalValidationService, ExternalValidationService>();

        return services;
    }
}