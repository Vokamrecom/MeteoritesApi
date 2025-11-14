namespace MeteoritesApi.Options;

public class HangfireOptions
{
    public const string SectionName = "Hangfire";

    public string Queue { get; set; } = "meteorite-sync";

    public int WorkerCount { get; set; } = 1;
}

