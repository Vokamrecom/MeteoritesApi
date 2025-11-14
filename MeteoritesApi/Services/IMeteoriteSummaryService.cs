using MeteoritesApi.Dtos;

namespace MeteoritesApi.Services;

public interface IMeteoriteSummaryService
{
    Task<MeteoriteSummaryResponse> GetSummaryAsync(MeteoriteSummaryQuery query, CancellationToken cancellationToken);
}

