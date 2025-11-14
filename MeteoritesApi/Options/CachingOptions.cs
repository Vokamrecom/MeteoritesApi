namespace MeteoritesApi.Options;

public class CachingOptions
{
    public const string SectionName = "Caching";

    public int SummaryTtlMinutes { get; set; } = 10;
}

