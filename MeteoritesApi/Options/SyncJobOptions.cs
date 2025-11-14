namespace MeteoritesApi.Options;

public class SyncJobOptions
{
    public const string SectionName = "SyncJob";

    public string CronExpression { get; set; } = "0 */1 * * *";

    public int BatchSize { get; set; } = 500;
}

