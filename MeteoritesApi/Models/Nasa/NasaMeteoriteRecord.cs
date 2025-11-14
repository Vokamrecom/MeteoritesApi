using System.Text.Json.Serialization;

namespace MeteoritesApi.Models.Nasa;

public class NasaMeteoriteRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("nametype")]
    public string? NameType { get; set; }

    [JsonPropertyName("recclass")]
    public string? Recclass { get; set; }

    [JsonPropertyName("mass")]
    public string? Mass { get; set; }

    [JsonPropertyName("fall")]
    public string? Fall { get; set; }

    [JsonPropertyName("year")]
    public string? Year { get; set; }

    [JsonPropertyName("reclat")]
    public string? Reclat { get; set; }

    [JsonPropertyName("reclong")]
    public string? Reclong { get; set; }

    [JsonPropertyName("geolocation")]
    public NasaGeoLocation? Geolocation { get; set; }
}

public class NasaGeoLocation
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("coordinates")]
    public List<double>? Coordinates { get; set; }
}

