namespace MeteoritesApi.Dtos;

public class MeteoriteSummaryResponse
{
    public IReadOnlyCollection<MeteoriteSummaryItem> Items { get; set; } = Array.Empty<MeteoriteSummaryItem>();

    public int TotalCount { get; set; }

    public decimal TotalMass { get; set; }
}

public class MeteoriteSummaryItem
{
    public int Year { get; set; }

    public int Count { get; set; }

    public decimal TotalMass { get; set; }
}

