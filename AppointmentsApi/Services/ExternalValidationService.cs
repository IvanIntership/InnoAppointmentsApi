using System.Net.Http.Headers;
using InnoAppointmentsApi.Dtos.External;
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
    
    public async Task<TimeSpan> GetServiceDurationAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        var token = context != null ? await context.GetTokenAsync("access_token") : null;
        
        var serviceRequest = new HttpRequestMessage(HttpMethod.Get, $"/services/{serviceId}");
        if (!string.IsNullOrEmpty(token)) serviceRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var serviceResponse = await _gatewayClient.SendAsync(serviceRequest, cancellationToken);
        if (!serviceResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to fetch service {serviceId}");
            
        var service = await serviceResponse.Content.ReadFromJsonAsync<ServiceDto>(cancellationToken: cancellationToken);
        
        var categoryRequest = new HttpRequestMessage(HttpMethod.Get, $"/serviceCategories/{service!.CategoryId}");
        if (!string.IsNullOrEmpty(token)) categoryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var categoryResponse = await _gatewayClient.SendAsync(categoryRequest, cancellationToken);
        if (!categoryResponse.IsSuccessStatusCode)
            throw new Exception($"Failed to fetch category {service.CategoryId}");

        var category = await categoryResponse.Content.ReadFromJsonAsync<ServiceCategoryDto>(cancellationToken: cancellationToken);
        
        return category!.Duration;
    }
    
    public async Task<bool> DoctorProvidesServiceAsync(Guid doctorId, Guid serviceId, CancellationToken cancellationToken = default)
    {
        var doctorResponse = await _gatewayClient.GetAsync($"/doctors/{doctorId}", cancellationToken);
        if (!doctorResponse.IsSuccessStatusCode)
            return false;

        var doctor = await doctorResponse.Content.ReadFromJsonAsync<DoctorDto>(cancellationToken: cancellationToken);
        if (doctor == null)
            return false;

        var serviceResponse = await _gatewayClient.GetAsync($"/services/{serviceId}", cancellationToken);
        if (!serviceResponse.IsSuccessStatusCode)
            return false;

        var service = await serviceResponse.Content.ReadFromJsonAsync<ServiceDto>(cancellationToken: cancellationToken);
        if (service == null)
            return false;
        
        return doctor.SpecializationId == service.SpecializationId;
    }
}