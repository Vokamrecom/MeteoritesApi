using MeteoritesApi.Models.Nasa;

namespace MeteoritesApi.Clients;

public interface INasaDatasetClient
{
    Task<IReadOnlyCollection<NasaMeteoriteRecord>> FetchAsync(CancellationToken cancellationToken);
}

