using MeteoritesApi.Data;
using MeteoritesApi.Dtos;
using MeteoritesApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MeteoritesApi.Services;

public class MeteoriteSummaryService : IMeteoriteSummaryService
{
    private readonly MeteoritesDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly CachingOptions _options;
    private readonly ILogger<MeteoriteSummaryService> _logger;

    public MeteoriteSummaryService(
        MeteoritesDbContext dbContext,
        IMemoryCache cache,
        IOptions<CachingOptions> options,
        ILogger<MeteoriteSummaryService> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<MeteoriteSummaryResponse> GetSummaryAsync(MeteoriteSummaryQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(query);
        if (_cache.TryGetValue(cacheKey, out MeteoriteSummaryResponse? cachedResponse))
        {
            return cachedResponse!;
        }

        var dataQuery = _dbContext.Meteorites.AsNoTracking();

        if (query.YearFrom.HasValue)
        {
            dataQuery = dataQuery.Where(x => x.Year >= query.YearFrom);
        }

        if (query.YearTo.HasValue)
        {
            dataQuery = dataQuery.Where(x => x.Year <= query.YearTo);
        }

        if (!string.IsNullOrWhiteSpace(query.Recclass))
        {
            dataQuery = dataQuery.Where(x => x.Recclass != null &&
                                             EF.Functions.ILike(x.Recclass!, $"%{query.Recclass.Trim()}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.NamePart))
        {
            var namePart = query.NamePart.Trim();
            dataQuery = dataQuery.Where(x => EF.Functions.ILike(x.Name, $"%{namePart}%"));
        }

        var summaryQuery = dataQuery
            .Where(x => x.Year.HasValue)
            .GroupBy(x => x.Year!.Value)
            .Select(g => new MeteoriteSummaryItem
            {
                Year = g.Key,
                Count = g.Count(),
                TotalMass = g.Sum(m => m.MassGram ?? 0)
            });

        summaryQuery = ApplySorting(summaryQuery, query);

        var items = await summaryQuery.ToListAsync(cancellationToken);

        var response = new MeteoriteSummaryResponse
        {
            Items = items,
            TotalCount = items.Sum(x => x.Count),
            TotalMass = items.Sum(x => x.TotalMass)
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.SummaryTtlMinutes)
        };

        _cache.Set(cacheKey, response, cacheEntryOptions);
        _logger.LogInformation("Cached meteorite summary with key {CacheKey}", cacheKey);

        return response;
    }

    private static IQueryable<MeteoriteSummaryItem> ApplySorting(IQueryable<MeteoriteSummaryItem> query, MeteoriteSummaryQuery request)
    {
        var descending = request.IsSortDescending();

        return request.NormalizeSortField() switch
        {
            "count" => descending
                ? query.OrderByDescending(x => x.Count)
                : query.OrderBy(x => x.Count),
            "mass" => descending
                ? query.OrderByDescending(x => x.TotalMass)
                : query.OrderBy(x => x.TotalMass),
            _ => descending
                ? query.OrderByDescending(x => x.Year)
                : query.OrderBy(x => x.Year)
        };
    }

    private static string BuildCacheKey(MeteoriteSummaryQuery query)
    {
        return $"summary:{query.YearFrom}:{query.YearTo}:{query.Recclass}:{query.NamePart}:{query.NormalizeSortField()}:{(query.IsSortDescending() ? "desc" : "asc")}";
    }
}

