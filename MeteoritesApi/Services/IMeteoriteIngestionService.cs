namespace MeteoritesApi.Services;

public interface IMeteoriteIngestionService
{
    Task RunIngestionAsync(string? hangfireJobId = null, CancellationToken cancellationToken = default);
}

