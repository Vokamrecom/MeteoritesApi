using MeteoritesApi.Dtos;

namespace MeteoritesApi.Services;

public interface IMeteoriteFilterService
{
    Task<MeteoriteFiltersResponse> GetFiltersAsync(CancellationToken cancellationToken);
}

