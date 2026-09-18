using System.Net.Http.Headers;
using InnoAppointmentsApi.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace InnoAppointmentsApi.Services;

public sealed class ExternalValidationService : IExternalValidationService
{
    private readonly HttpClient _gatewayClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExternalValidationService(
        IHttpClientFactory httpClientFactory, 
        IHttpContextAccessor httpContextAccessor)
    {
        _gatewayClient = httpClientFactory.CreateClient("GatewayClient");
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> DoctorExistsAsync(Guid doctorId, CancellationToken cancellationToken)
    {
        return await CheckEndpointAsync(_gatewayClient, $"/doctors/{doctorId}", cancellationToken);
    }

    public async Task<bool> PatientExistsAsync(Guid patientId, CancellationToken cancellationToken)
    {
        return await CheckEndpointAsync(_gatewayClient, $"/patients/{patientId}", cancellationToken);
    }

    public async Task<bool> ServiceExistsAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        return await CheckEndpointAsync(_gatewayClient, $"/services/{serviceId}", cancellationToken);
    }

    private async Task<bool> CheckEndpointAsync(HttpClient client, string url, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            var token = await context.GetTokenAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}