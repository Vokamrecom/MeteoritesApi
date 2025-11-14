namespace MeteoritesApi.Options;

public class NasaDataOptions
{
    public const string SectionName = "NASAData";

    public string SourceUrl { get; set; } = string.Empty;

    public int RequestTimeoutSeconds { get; set; } = 30;
}

