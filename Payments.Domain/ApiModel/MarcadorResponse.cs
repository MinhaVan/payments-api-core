using System.Text.Json.Serialization;

namespace Payments.Domain.ViewModels.Rota;

public class MarcadorResponse
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }
}