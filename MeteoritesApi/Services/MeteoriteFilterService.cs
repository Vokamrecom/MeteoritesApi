using MeteoritesApi.Data;
using MeteoritesApi.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MeteoritesApi.Services;

public class MeteoriteFilterService : IMeteoriteFilterService
{
    internal const string CacheKey = "meteorite-filters";

    private readonly MeteoritesDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MeteoriteFilterService> _logger;

    public MeteoriteFilterService(MeteoritesDbContext dbContext, IMemoryCache cache, ILogger<MeteoriteFilterService> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _logger = logger;
    }

    public async Task<MeteoriteFiltersResponse> GetFiltersAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKey, out MeteoriteFiltersResponse? cached) && cached is not null)
        {
            return cached;
        }

        var yearQuery = _dbContext.Meteorites.Where(x => x.Year.HasValue);

        var minYear = await yearQuery.MinAsync(x => (int?)x.Year, cancellationToken) ?? 0;
        var maxYear = await yearQuery.MaxAsync(x => (int?)x.Year, cancellationToken) ?? DateTime.UtcNow.Year;

        var recclasses = await _dbContext.Meteorites
            .Where(x => x.Recclass != null)
            .Select(x => x.Recclass!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var response = new MeteoriteFiltersResponse
        {
            MinYear = minYear,
            MaxYear = maxYear,
            Recclasses = recclasses
        };

        _cache.Set(CacheKey, response, TimeSpan.FromHours(1));
        _logger.LogInformation("Cached meteorite filter metadata");

        return response;
    }
}

