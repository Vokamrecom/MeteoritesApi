using System.Net;
using System.Text.Json;
using MeteoritesApi.Models.Nasa;
using MeteoritesApi.Options;
using Microsoft.Extensions.Options;

namespace MeteoritesApi.Clients;

public class NasaDatasetClient : INasaDatasetClient
{
    private readonly HttpClient _httpClient;
    private readonly NasaDataOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public NasaDatasetClient(HttpClient httpClient, IOptions<NasaDataOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.Timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds);
    }

    public async Task<IReadOnlyCollection<NasaMeteoriteRecord>> FetchAsync(CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(_options.SourceUrl, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            return Array.Empty<NasaMeteoriteRecord>();
        }

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var data = await JsonSerializer.DeserializeAsync<List<NasaMeteoriteRecord>>(stream, _serializerOptions, cancellationToken);

        return data as IReadOnlyCollection<NasaMeteoriteRecord> ?? Array.Empty<NasaMeteoriteRecord>();
    }
}

