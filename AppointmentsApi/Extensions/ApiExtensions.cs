using Microsoft.OpenApi;

namespace InnoAppointmentsApi.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services, string applicationName)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            options.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = applicationName, 
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

        return services;
    }
}