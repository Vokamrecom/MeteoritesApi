using MeteoritesApi.Clients;
using MeteoritesApi.Data;
using MeteoritesApi.Entities;
using MeteoritesApi.Models.Nasa;
using MeteoritesApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MeteoritesApi.Services;

public class MeteoriteIngestionService : IMeteoriteIngestionService
{
    private readonly MeteoritesDbContext _dbContext;
    private readonly INasaDatasetClient _nasaDatasetClient;
    private readonly ILogger<MeteoriteIngestionService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly SyncJobOptions _options;

    public MeteoriteIngestionService(
        MeteoritesDbContext dbContext,
        INasaDatasetClient nasaDatasetClient,
        IOptions<SyncJobOptions> options,
        ILogger<MeteoriteIngestionService> logger,
        IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _nasaDatasetClient = nasaDatasetClient;
        _logger = logger;
        _options = options.Value;
        _memoryCache = memoryCache;
    }

    public async Task RunIngestionAsync(string? hangfireJobId = null, CancellationToken cancellationToken = default)
    {
        var run = new IngestionRun
        {
            HangfireJobId = hangfireJobId,
            Status = "Running",
            StartedAt = DateTime.UtcNow
        };

        _dbContext.IngestionRuns.Add(run);
        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var records = await _nasaDatasetClient.FetchAsync(cancellationToken);
            _logger.LogInformation("Fetched {Count} records from NASA dataset", records.Count);

            if (records.Count == 0)
            {
                run.Status = "NoChanges";
                run.FinishedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return;
            }

            var existing = await _dbContext.Meteorites
                .AsTracking()
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var processedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var batchSize = Math.Max(1, _options.BatchSize);
            var processed = 0;

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.Id) || string.IsNullOrWhiteSpace(record.Name))
                {
                    continue;
                }

                var normalizedId = record.Id.Trim();
                processedIds.Add(normalizedId);

                if (existing.TryGetValue(normalizedId, out var entity))
                {
                    var updated = ApplyRecord(entity, record);
                    if (updated)
                    {
                        run.Updated++;
                    }
                }
                else
                {
                    var newEntity = CreateEntity(record);
                    if (newEntity is null)
                    {
                        continue;
                    }

                    await _dbContext.Meteorites.AddAsync(newEntity, cancellationToken);
                    run.Inserted++;
                }

                processed++;
                if (processed % batchSize == 0)
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }

            var toDelete = existing
                .Where(pair => !processedIds.Contains(pair.Key))
                .Select(pair => pair.Value)
                .ToList();

            if (toDelete.Count > 0)
            {
                _dbContext.Meteorites.RemoveRange(toDelete);
                run.Deleted += toDelete.Count;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            run.Status = "Succeeded";
            run.FinishedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            InvalidateCaches();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ingestion job failed");
            run.Status = "Failed";
            run.ErrorMessage = ex.Message;
            run.FinishedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            throw;
        }
    }

    private void InvalidateCaches()
    {
        _memoryCache.Remove(MeteoriteFilterService.CacheKey);
        if (_memoryCache is MemoryCache memoryCache)
        {
            memoryCache.Compact(1.0);
        }
    }

    private static Meteorite? CreateEntity(NasaMeteoriteRecord record)
    {
        if (string.IsNullOrWhiteSpace(record.Id) || string.IsNullOrWhiteSpace(record.Name))
        {
            return null;
        }

        var entity = new Meteorite
        {
            Id = record.Id.Trim(),
            Name = record.Name.Trim()
        };

        ApplyRecord(entity, record);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        return entity;
    }

    private static bool ApplyRecord(Meteorite entity, NasaMeteoriteRecord record)
    {
        var changed = false;

        var name = record.Name?.Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            changed |= SetValue(entity.Name, name, v => entity.Name = v);
        }

        changed |= SetValue(entity.NameType, record.NameType?.Trim(), v => entity.NameType = v);
        changed |= SetValue(entity.Recclass, record.Recclass?.Trim(), v => entity.Recclass = v);
        changed |= SetValue(entity.Fall, record.Fall?.Trim(), v => entity.Fall = v);
        changed |= SetValue(entity.GeoLocationType, record.Geolocation?.Type?.Trim(), v => entity.GeoLocationType = v);

        var mass = ParseDecimal(record.Mass);
        changed |= SetValue(entity.MassGram, mass, v => entity.MassGram = v);

        var year = ParseYear(record.Year);
        changed |= SetValue(entity.Year, year, v => entity.Year = v);

        var lat = ParseDouble(record.Reclat);
        var lon = ParseDouble(record.Reclong);

        if (record.Geolocation?.Coordinates is { Count: >= 2 })
        {
            lon ??= record.Geolocation.Coordinates[0];
            lat ??= record.Geolocation.Coordinates[1];
        }

        changed |= SetValue(entity.Latitude, lat, v => entity.Latitude = v);
        changed |= SetValue(entity.Longitude, lon, v => entity.Longitude = v);

        if (changed)
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return changed;
    }

    private static bool SetValue<T>(T currentValue, T newValue, Action<T> setter)
    {
        if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
        {
            return false;
        }

        setter(newValue);
        return true;
    }

    private static decimal? ParseDecimal(string? input)
    {
        if (decimal.TryParse(input, out var value))
        {
            return value;
        }

        return null;
    }

    private static int? ParseYear(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (DateTime.TryParse(input, out var date))
        {
            return date.Year;
        }

        if (int.TryParse(input.AsSpan(0, Math.Min(4, input.Length)), out var year))
        {
            return year;
        }

        return null;
    }

    private static double? ParseDouble(string? input)
    {
        if (double.TryParse(input, out var value))
        {
            return value;
        }

        return null;
    }
}

