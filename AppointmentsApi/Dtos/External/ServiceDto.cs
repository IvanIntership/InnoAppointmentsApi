using System.Text.Json.Serialization;

namespace InnoAppointmentsApi.Dtos.External;

public sealed class ServiceDto
{
    public Guid Id { get; set; }

    [JsonPropertyName("serviceCategoryId")]
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid SpecializationId { get; set; }
}