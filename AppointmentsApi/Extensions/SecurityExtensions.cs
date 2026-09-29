using System.Security.Claims;
using System.Text.Json;
using InnoAppointmentsApi.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace InnoAppointmentsApi.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var keycloakBaseUrl = configuration["Keycloak:BaseUrl"];
            var keycloakRealm = configuration["Keycloak:Realm"];

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

        services.AddAuthorization(options =>
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

        return services;
    }
}