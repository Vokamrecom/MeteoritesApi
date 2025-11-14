using MeteoritesApi.Services;

namespace MeteoritesApi.Jobs;

public class MeteoriteSyncJob
{
    private readonly IMeteoriteIngestionService _ingestionService;

    public MeteoriteSyncJob(IMeteoriteIngestionService ingestionService)
    {
        _ingestionService = ingestionService;
    }

    public Task Execute(CancellationToken cancellationToken)
        => _ingestionService.RunIngestionAsync(null, cancellationToken);
}

