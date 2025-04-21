using System.Text.Json.Serialization;

namespace Core.Domain.ViewModels.Rota;
public class MarcadorResponse
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }
}